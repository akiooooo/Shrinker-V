using System;
using System.Text;
using System.Threading;
using ShrinkerV.Core;
internal static class CommandLine {
    internal static int Run(string[] a) {
        Console.OutputEncoding = new UTF8Encoding(false);
        try {
            if (a.Length < 2) return 2;
            var o=Shrinker.CreateDefaultOptions();
            o.InputFolder=a[0]; o.OutputFolder=a[1];
            for (int i=2;i<a.Length;i++) {
                switch(a[i]){
                    case "--gray": o.UseGrayscale=true;break;
                    case "--factor": o.CompressionFactor=int.Parse(a[++i]);break;
                    case "--npc-factor": o.PedsCompressionFactor=int.Parse(a[++i]);break;
                    case "--player-factor": o.ClothingCompressionFactor=int.Parse(a[++i]);break;
                    case "--npc-gray":o.PedsUseGrayscale=true;break;
                    case "--player-gray":o.ClothingUseGrayscale=true;break;
                    case "--process-player":o.ProtectCharacters=false;break;
                    case "--keep-npcs":o.SkipPeds=true;break;
                    case "--process-weapons":o.SkipWeapons=false;break;
                    case "--remove-map":o.RemoveMap=true;break;
                    case "--replace-audio":o.KeepAudio=false;break;
                    case "--replace-video":o.KeepVideo=false;break;
                    case "--replace-effects":o.KeepEffects=false;break;
                    case "--min-size":o.MinTextureSize=int.Parse(a[++i]);break;
                    case "--threads":o.MaxThreads=int.Parse(a[++i]);break;
                    case "--verbose":o.VerboseLog=true;break;
                    case "--in-place":o.ProcessInPlace=true;break;
                    default:throw new ArgumentException("Unknown option: "+a[i]);
                }
            }
            using (var stop=new CancellationTokenSource()){
                o.Cancel=stop.Token;
                var listener=new Thread(()=>{try{ if(Console.ReadLine()=="cancel") stop.Cancel(); }catch{}});
                listener.IsBackground=true; listener.Start();
                var stats=Shrinker.Run(o,Console.WriteLine);
                return stats.ArchivesFailed+stats.TexturesFailed+stats.ErrorsEncountered==0?0:1;
            }
        }catch(OperationCanceledException){Console.WriteLine("Отменено.");return 3;}
         catch(Exception e){Console.WriteLine(e.Message);return 1;}
    }
}
