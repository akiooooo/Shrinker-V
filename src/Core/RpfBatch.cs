using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GameFormats.Files;

namespace ShrinkerV.Core
{
    internal sealed class RpfBatch : IDisposable
    {

        private struct Part { public long Start, Bytes; public int Row, Patched; }

        private static uint Pack(FileStream original, FileStream patches, FileStream output,
            byte[] table, int namesLength, Part[] parts)
        {
            long next = (16L + table.Length + namesLength + 511) & ~511L;
            byte[] buffer = new byte[256 * 1024];
            foreach (Part part in parts)
            {
                FileStream source = part.Patched == 0 ? original : patches;
                if (part.Start < 0 || part.Bytes < 0 || part.Start > source.Length - part.Bytes)
                    throw new InvalidDataException("Entry outside its source stream.");
                long sector = next / 512;
                if (sector >= 0x800000)
                    throw new InvalidDataException("RPF7 sector exceeds 23 bits.");
                int row = checked(part.Row * 16);
                table[row + 5] = (byte)sector;
                table[row + 6] = (byte)(sector >> 8);
                table[row + 7] = (byte)((table[row + 7] & 0x80) | (int)(sector >> 16));
                output.Position = next;
                source.Position = part.Start;
                long remaining = part.Bytes;
                while (remaining > 0)
                {
                    int read = source.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                    if (read == 0) throw new EndOfStreamException();
                    output.Write(buffer, 0, read);
                    remaining -= read;
                }
                next = checked(next + ((part.Bytes + 511) & ~511L));
            }
            uint size = checked((uint)next);
            output.SetLength(size);
            return size;
        }
        private sealed class Patch
        {
            public long Start;
            public long Size;
            public uint StoredSize, System, Graphics;
            public bool Resource;
        }
        private readonly string input, directory, spoolPath;
        private readonly FileStream spool;
        private readonly Dictionary<RpfFileEntry,Patch> replacements = new Dictionary<RpfFileEntry,Patch>();
        private readonly HashSet<RpfFile> dirty = new HashSet<RpfFile>();
        // A failed GameFormats resource decode can mutate FileSize. Snapshot physical lengths before extraction.
        private readonly Dictionary<RpfFileEntry,long> originalSizes = new Dictionary<RpfFileEntry,long>();

        public bool Verify(RpfFile original, string output)
        {
            var packed = new RpfFile(output, original.Name);
            packed.ScanStructure(null, null);
            return SameTree(original, packed);
        }

        private bool SameTree(RpfFile original, RpfFile packed)
        {
            if (packed.LastException != null || packed.AllEntries == null || packed.Children == null ||
                original.AllEntries.Count != packed.AllEntries.Count || original.Children.Count != packed.Children.Count)
                return false;
            for (int i = 0; i < original.AllEntries.Count; i++)
            {
                var before = original.AllEntries[i];
                var after = packed.AllEntries[i];
                if (before.Path != after.Path || before.GetType() != after.GetType()) return false;
                var file = before as RpfFileEntry;
                if (file != null && originalSizes[file] > 0 && ((RpfFileEntry)after).GetFileSize() == 0)
                    return false;
            }
            for (int i = 0; i < original.Children.Count; i++)
                if (original.Children[i].Name != packed.Children[i].Name ||
                    !SameTree(original.Children[i], packed.Children[i])) return false;
            return true;
        }

        private void Snapshot(RpfFile archive)
        {
            foreach(var e in archive.AllEntries.OfType<RpfFileEntry>()) originalSizes.Add(e,e.GetFileSize());
            foreach(var child in archive.Children) Snapshot(child);
        }
        public RpfBatch(string path,string folder,RpfFile root)
        {
            Snapshot(root);
            input=path; directory=folder; spoolPath=Path.Combine(folder,Guid.NewGuid().ToString("N")+".patch");
            spool=new FileStream(spoolPath,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
        }
        public void Dispose(){spool.Dispose();File.Delete(spoolPath);}
        public void Add(RpfFileEntry entry,byte[] data)
        {
            var p=new Patch();
            if ((entry is RpfResourceFileEntry) != (data.Length >= 16 && BitConverter.ToUInt32(data,0) == 0x37435352)) throw new InvalidDataException("RPF replacement type mismatch: "+entry.Path);
            if(data.Length>=16 && BitConverter.ToUInt32(data,0)==0x37435352)
            {
                p.Resource=true; p.System=BitConverter.ToUInt32(data,8); p.Graphics=BitConverter.ToUInt32(data,12);
                p.StoredSize=(uint)Math.Min(data.Length,0xffffff);
                if(data.Length>=0xffffff){
                    data[7]=(byte)data.Length; data[14]=(byte)(data.Length>>8);
                    data[5]=(byte)(data.Length>>16); data[2]=(byte)(data.Length>>24);
                }
            }
            else
            {
                p.System=(uint)data.Length;
                if(!entry.Name.EndsWith(".awc",StringComparison.OrdinalIgnoreCase)){
                    var compressed=RpfFile.CompressBytes(data);
                    if(compressed.Length<=0xffffff){data=compressed;p.StoredSize=(uint)data.Length;}
                }
            }
            Store(entry,data,p);
        }

        internal void RestoreResource(RpfFileEntry target, RpfResourceFileEntry original, byte[] stored)
        {
            if (stored.LongLength != original.GetFileSize()) throw new InvalidDataException(original.Path);
            Store(target,stored,new Patch { Resource=true, StoredSize=Math.Min(original.FileSize,0xffffffu),
                System=original.SystemFlags, Graphics=original.GraphicsFlags });
        }

        private void Store(RpfFileEntry entry, byte[] data, Patch p)
        {
            p.Start=spool.Position;p.Size=data.Length;
            spool.Write(data,0,data.Length);replacements.Add(entry,p);
            for(var node=entry.File;node!=null;node=node.Parent) dirty.Add(node);
        }

        private static byte[] Crypt(RpfFile file,byte[] data,uint size,bool encode)
        {
            switch(file.Encryption){
                case RpfEncryption.NONE: case RpfEncryption.OPEN: return data;
                case RpfEncryption.AES:return encode?GTACrypto.EncryptAES(data):GTACrypto.DecryptAES(data);
                case RpfEncryption.NG:return encode?GTACrypto.EncryptNG(data,file.Name,size):GTACrypto.DecryptNG(data,file.Name,size);
                default:throw new InvalidDataException("Unknown RPF encryption: "+file.Encryption);
            }
        }

        // Tables retain their original order, directory ranges, names and encryption mode.
        public void Write(RpfFile archive,string output)
        {
            foreach(var child in archive.Children){
                if(!dirty.Contains(child))continue;
                string nested=Path.Combine(directory,Guid.NewGuid().ToString("N")+".child");
                try{
                    Write(child,nested);
                    using(var data=File.OpenRead(nested)){
                        var size=checked((uint)data.Length);
                        var p=new Patch{Start=spool.Position,Size=size,System=(uint)size};
                        data.CopyTo(spool);replacements.Add(child.ParentFileEntry,p);
                    }
                }finally{File.Delete(nested);}
            }
            spool.Flush();
            using(var source=File.OpenRead(input))
            using(var reader=new BinaryReader(source))
            using(var patches=new FileStream(spoolPath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
            using(var dest=new FileStream(output,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None))
            {
                source.Position=archive.StartPos;
                byte[] header=reader.ReadBytes(16);
                int count=BitConverter.ToInt32(header,4), namesLen=BitConverter.ToInt32(header,8);
                if(count!=archive.AllEntries.Count||namesLen<0||16L+16L*count+namesLen>archive.FileSize)
                    throw new InvalidDataException("Invalid RPF table bounds");
                var originalSize=checked((uint)archive.FileSize);
                byte[] toc=Crypt(archive,reader.ReadBytes(checked(count*16)),originalSize,false);
                byte[] names=Crypt(archive,reader.ReadBytes(namesLen),originalSize,false);
                var parts=new List<Part>();
                for(int i=0;i<count;i++){
                    var e=archive.AllEntries[i] as RpfFileEntry;
                    if(e==null)continue;
                    var part=new Part{Row=i,Start=archive.StartPos+512L*e.FileOffset,Bytes=originalSizes[e]};
                    Patch change;
                    if(replacements.TryGetValue(e,out change)){
                        part.Start=change.Start;part.Bytes=change.Size;part.Patched=1;
                        int pos=i*16;uint size=change.StoredSize;
                        toc[pos+2]=(byte)size;toc[pos+3]=(byte)(size>>8);toc[pos+4]=(byte)(size>>16);
                        toc[pos+7]=(byte)(change.Resource?128:0);
                        Buffer.BlockCopy(BitConverter.GetBytes(change.System),0,toc,pos+8,4);
                        Buffer.BlockCopy(BitConverter.GetBytes(change.Graphics),0,toc,pos+12,4);
                    }else if(part.Start<archive.StartPos || part.Bytes<0 || part.Start+part.Bytes>archive.StartPos+archive.FileSize){
                        throw new InvalidDataException("Entry outside owning RPF: "+e.Path);
                    }
                    parts.Add(part);
                }
                var sorted=parts.OrderBy(p=>p.Patched).ThenBy(p=>p.Start).ToArray();
                uint finalSize = Pack(source, patches, dest, toc, names.Length, sorted);
                toc=Crypt(archive,toc,finalSize,true);names=Crypt(archive,names,finalSize,true);
                dest.Seek(0,SeekOrigin.Begin);dest.Write(header,0,header.Length);dest.Write(toc,0,toc.Length);dest.Write(names,0,names.Length);
            }
        }
    }
}
