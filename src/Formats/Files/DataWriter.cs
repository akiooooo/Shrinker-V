using System;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	public class DataWriter
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

		public DataWriter(Stream stream, Endianess endianess = Endianess.LittleEndian)
		{
			baseStream = stream;
			Endianess = endianess;
		}

		protected virtual void WriteToStream(byte[] value, bool ignoreEndianess = false)
		{
			if (!ignoreEndianess && Endianess == Endianess.BigEndian)
			{
				byte[] array = (byte[])value.Clone();
				Array.Reverse(array);
				baseStream.Write(array, 0, array.Length);
			}
			else
			{
				baseStream.Write(value, 0, value.Length);
			}
		}

		public void Write(byte value)
		{
			WriteToStream(new byte[1] { value });
		}

		public void Write(byte[] value)
		{
			WriteToStream(value, ignoreEndianess: true);
		}

		public void Write(short value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(int value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(long value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(ushort value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(uint value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(ulong value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(float value)
		{
			WriteToStream(BitConverter.GetBytes(value));
		}

		public void Write(string value)
		{
			foreach (char c in value)
			{
				Write((byte)c);
			}
			Write((byte)0);
		}

		public void Write(Vector3 value)
		{
			Write(value.X);
			Write(value.Y);
			Write(value.Z);
		}

		public void Write(Vector4 value)
		{
			Write(value.X);
			Write(value.Y);
			Write(value.Z);
			Write(value.W);
		}

		public void Write(Matrix value)
		{
			Write(value.M11);
			Write(value.M21);
			Write(value.M31);
			Write(value.M41);
			Write(value.M12);
			Write(value.M22);
			Write(value.M32);
			Write(value.M42);
			Write(value.M13);
			Write(value.M23);
			Write(value.M33);
			Write(value.M43);
			Write(value.M14);
			Write(value.M24);
			Write(value.M34);
			Write(value.M44);
		}
	}
}
