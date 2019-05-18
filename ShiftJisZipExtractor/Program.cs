using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ShiftJisZipList
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = args[0];
			string output = args[1];

			Extract(path, output);
		}

		private static void Extract(string path, string output)
		{
			// Get full paths
			string zipPath = Path.GetFullPath(path);
			string extractPath = Path.GetFullPath(output);

			ZipFilenameReader zip = new ZipFilenameReader(zipPath);
			IEnumerable<string> filenames = zip.GetAllFilenames();

			// Ensures that the last character on the extraction path is the directory separator char
			if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				extractPath += Path.DirectorySeparatorChar;

			using (ZipArchive archive = ZipFile.OpenRead(zipPath))
			{
				IEnumerable<string> files = archive.Entries.Zip(filenames, (entry, filename) =>
				{
					// Get full path
					string destinationPath = Path.GetFullPath(Path.Combine(extractPath, filename));

					// Check that path is safe
					if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
					{
						// Create directory if it does not exist
						Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

						// If it's a file, extract it
						if (Path.GetFileName(destinationPath) != String.Empty)
						{
							entry.ExtractToFile(destinationPath);
						}
					}

					return destinationPath;
				});
				foreach (string file in files)
				{
					Console.WriteLine(file);
				}
			}
		}
	}
}
