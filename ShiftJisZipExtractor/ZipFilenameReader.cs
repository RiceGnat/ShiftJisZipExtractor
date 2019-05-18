using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftJisZipList
{
	public class ZipFilenameReader
	{
		
		private const int FILE_HEADER_SIGNATURE = 0x04034b50; // Reversed for endianness
		private const int FILE_NAME_LEN_OFFSET = 0x001a;
		private const int EXTRA_FIELD_LEN_OFFSET = 0x001c;
		private const int FILE_NAME_OFFSET = 0x001e;
		private const int COMPRESSED_SIZE_OFFSET = 0x0012;

		private class ZipEntry
		{
			private byte[] bytes;
			private int offset;
			private int filenameLen;

			public ZipEntry(byte[] bytes, int offset)
			{
				this.bytes = bytes;
				this.offset = offset;
				filenameLen = BitConverter.ToInt16(bytes, offset + FILE_NAME_LEN_OFFSET);
			}

			public string GetFilename()
				=> Encoding.GetEncoding("shift_jis").GetString(bytes, offset + FILE_NAME_OFFSET, filenameLen);

			public int GetBlockSize()
			{
				// Ignoring data descriptor and ZIP64 for now until it becomes a problem

				int extraLen = BitConverter.ToInt16(bytes, offset + EXTRA_FIELD_LEN_OFFSET);
				int compressedSize = BitConverter.ToInt32(bytes, offset + COMPRESSED_SIZE_OFFSET);

				return FILE_NAME_OFFSET + extraLen + compressedSize;
			}
		}

		private byte[] bytes;

		public ZipFilenameReader(string path)
		{
			bytes = File.ReadAllBytes(path);
		}
		
		public IEnumerable<string> GetAllFilenames()
		{
			List<string> filenames = new List<string>();

			int i = 0;
			while (i < bytes.Length - FILE_NAME_OFFSET)
			{
				// See if we're at a file signature
				if (BitConverter.ToInt32(bytes, i) == FILE_HEADER_SIGNATURE)
				{
					ZipEntry entry = new ZipEntry(bytes, i);
					filenames.Add(entry.GetFilename());
					i += entry.GetBlockSize();
				}
				else // Keep looking
				{
					i++;
				}
			}

			return filenames;
		}
	}
}
