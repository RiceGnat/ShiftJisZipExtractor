using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ShiftJisZipEncoder
{
	class Program
	{
		static void Main(string[] args)
		{
			int offset = 0;
			string output;

			if (args[0].ToLowerInvariant() == "-o") {
				output = args[1];
				offset += 2;
			}
			else
			{
				output = ".";
			}

			int i = offset;

			while (i < args.Length)
			{
				string path = args[i];
				Extract(path, output);
				i++;
			}
		}

		private static void Extract(string path, string output)
		{
			// Get full paths
			string zipPath = Path.GetFullPath(path);
			string extractPath = Path.GetFullPath(output);

			string zipName = Path.GetFileNameWithoutExtension(path);
			extractPath = Path.Combine(extractPath, zipName);

			// Ensures that the last character on the extraction path is the directory separator char
			if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				extractPath += Path.DirectorySeparatorChar;

			using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Read, Encoding.GetEncoding("shift_jis")))
			{
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					// Get full path
					string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

					// Check that path is safe
					if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
					{
						// Create directory if it does not exist
						Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

						// If it's a file, extract it
						if (Path.GetFileName(destinationPath) != String.Empty)
						{
							entry.ExtractToFile(destinationPath);
							Console.WriteLine(destinationPath);
						}
					}
				}
			}
		}
	}
}
