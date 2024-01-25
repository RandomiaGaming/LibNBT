using System;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;

namespace LibNBT
{
	namespace Core
	{
		public abstract class Tag
		{
			internal Tag() { }
		}
		public abstract class NamedTag : Tag
		{
			public ushort NameLength;
			public string Name;
		}
		public sealed class Tag_End : Tag
		{
			public const byte ID = 0;
		}
		public sealed class Tag_Byte
		{
			public const byte ID = 1;
			public ushort NameLength;
			public string Name;
			public byte Payload;
		}
		public sealed class Tag_Short
		{
			public const byte ID = 2;
			public ushort NameLength;
			public string Name;
			public short Payload;
		}
		public sealed class Tag_Int
		{
			public const byte ID = 3;
			public ushort NameLength;
			public string Name;
			public int Payload;
		}
		public sealed class Tag_Long
		{
			public const byte ID = 4;
			public ushort NameLength;
			public string Name;
			public long Payload;
		}
		public sealed class Tag_Float
		{
			public const byte ID = 5;
			public ushort NameLength;
			public string Name;
			public float Payload;
		}
		public sealed class Tag_Double
		{
			public const byte ID = 6;
			public ushort NameLength;
			public string Name;
			public double Payload;
		}
		public sealed class Tag_Byte_Array
		{
			public const byte ID = 7;
			public ushort NameLength;
			public string Name;
			public int Length;
			public byte[] Payload;
		}
		public sealed class Tag_String
		{
			public const byte ID = 8;
			public ushort NameLength;
			public string Name;
			public ushort Length;
			public string Payload;
		}
		public sealed class Tag_List
		{
			public const byte ID = 9;
			public ushort NameLength;
			public string Name;
			public byte PayloadType;
			public int Length;
			public object[] Payload;
		}
		public sealed class Tag_Compound
		{

		}
		public sealed class Tag_Int_Array
		{

		}
		public sealed class Tag_Long_Array
		{

		}
	}


	//90px wide
	public static class NBTCodec
	{
		static void DecompressGZipFile(string sourcePath, string destinationPath)
		{
			using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
			{
				using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create))
				{
					using (GZipStream gzipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
					{
						byte[] buffer = new byte[4096];
						int bytesRead;

						while ((bytesRead = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
						{
							destinationStream.Write(buffer, 0, bytesRead);
						}
					}
				}
			}
		}
		public static GZipStream WrapWithGZip(Stream source)
		{
			return new GZipStream(source, CompressionMode.Decompress);
		}

		public static Tag Deserialize(string filePath, bool compressed = false)
		{
			FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

			Tag output = Deserialize(fileStream, compressed);

			fileStream.Close();
			fileStream.Dispose();

			return output;
		}
		public static Tag Deserialize(byte[] bytes, bool compressed = false)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);

			Tag output = Deserialize(memoryStream, compressed);

			memoryStream.Dispose();

			return output;
		}
		public static Tag Deserialize(Stream source, bool compressed = false)
		{
			if (compressed)
			{
				source = WrapWithGZip(source);
			}

			using (BinaryReader reader = new BinaryReader(source))
			{
				// Read the root compound tag
				ReadCompoundTag(reader);
			}

			return null;
		}

		private static void ReadCompoundTag(BinaryReader reader)
		{
			byte tagType = reader.ReadByte();
			while (tagType != 0) // 0 indicates end of compound tag
			{
				short nameLength = reader.ReadInt16();
				byte[] nameBytes = reader.ReadBytes(nameLength);
				string tagName = System.Text.Encoding.UTF8.GetString(nameBytes);

				Console.WriteLine($"Tag Type: {tagType}, Tag Name: {tagName}");

				ReadTagPayload(tagType, reader);

				tagType = reader.ReadByte();
			}
		}
		private static void ReadTagPayload(byte tagType, BinaryReader reader)
		{
			switch (tagType)
			{
				case 1: // TAG_Byte
					byte byteValue = reader.ReadByte();
					Console.WriteLine($"Byte Value: {byteValue}");
					break;
				case 2: // TAG_Short
					short shortValue = reader.ReadInt16();
					Console.WriteLine($"Short Value: {shortValue}");
					break;
				// Add cases for other tag types here...
				default:
					Console.WriteLine($"Unsupported Tag Type: {tagType}");
					break;
			}
		}

		public static void Main(string[] args)
		{
			Deserialize("D:\\Structure.nbt");
			Console.ReadLine();
		}
	}
}