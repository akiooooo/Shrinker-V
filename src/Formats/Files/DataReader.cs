using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameMath;

namespace GameFormats.Files
{
	public class DataReader
	{
		private Stream baseStream;

		public Endianess Endianess { get; set; }

		public virtual long Position
		{
			get
			{
				return baseStream.Position;
			}
			set
			{
				baseStream.Position = value;
			}
		}

		public DataReader(Stream stream, Endianess endianess = Endianess.LittleEndian)
		{
			baseStream = stream;
			Endianess = endianess;
		}

		protected virtual byte[] ReadFromStream(int count, bool ignoreEndianess = false)
		{
			byte[] array = new byte[count];
			baseStream.Read(array, 0, count);
			if (!ignoreEndianess && Endianess == Endianess.BigEndian)
			{
				Array.Reverse(array);
			}
			return array;
		}

		public byte ReadByte()
		{
			return ReadFromStream(1)[0];
		}

		public byte[] ReadBytes(int count)
		{
			return ReadFromStream(count, ignoreEndianess: true);
		}

		public short ReadInt16()
		{
			return BitConverter.ToInt16(ReadFromStream(2), 0);
		}

		public int ReadInt32()
		{
			return BitConverter.ToInt32(ReadFromStream(4), 0);
		}

		public long ReadInt64()
		{
			return BitConverter.ToInt64(ReadFromStream(8), 0);
		}

		public ushort ReadUInt16()
		{
			return BitConverter.ToUInt16(ReadFromStream(2), 0);
		}

		public uint ReadUInt32()
		{
			return BitConverter.ToUInt32(ReadFromStream(4), 0);
		}

		public ulong ReadUInt64()
		{
			return BitConverter.ToUInt64(ReadFromStream(8), 0);
		}

		public float ReadSingle()
		{
			return BitConverter.ToSingle(ReadFromStream(4), 0);
		}

		public string ReadString()
		{
			List<byte> list = new List<byte>();
			for (byte b = ReadFromStream(1)[0]; b != 0; b = ReadFromStream(1)[0])
			{
				list.Add(b);
			}
			return Encoding.UTF8.GetString(list.ToArray());
		}

		public Vector3 ReadVector3()
		{
			return new Vector3
			{
				X = ReadSingle(),
				Y = ReadSingle(),
				Z = ReadSingle()
			};
		}

		public Vector4 ReadVector4()
		{
			return new Vector4
			{
				X = ReadSingle(),
				Y = ReadSingle(),
				Z = ReadSingle(),
				W = ReadSingle()
			};
		}

		public Matrix ReadMatrix()
		{
			return new Matrix
			{
				M11 = ReadSingle(),
				M21 = ReadSingle(),
				M31 = ReadSingle(),
				M41 = ReadSingle(),
				M12 = ReadSingle(),
				M22 = ReadSingle(),
				M32 = ReadSingle(),
				M42 = ReadSingle(),
				M13 = ReadSingle(),
				M23 = ReadSingle(),
				M33 = ReadSingle(),
				M43 = ReadSingle(),
				M14 = ReadSingle(),
				M24 = ReadSingle(),
				M34 = ReadSingle(),
				M44 = ReadSingle()
			};
		}
	}
}
