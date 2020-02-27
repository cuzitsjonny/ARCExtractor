using System;
using System.IO;

namespace ARCExtractor
{
    public class ArcExtractor
    {
        public void ExtractSingleArchiveFile(string sourceFilePath, string destinationDirectoryPath)
        {
            Console.WriteLine("Extracting archive file (" + sourceFilePath + ")");

            Extract(sourceFilePath, destinationDirectoryPath);
        }

        public void ExtractFromIndexFile(string sourceFilePath, string destinationDirectoryPath)
        {
            Console.WriteLine("Reading index file (" + sourceFilePath + ")");

            IndexFile indexFile = new IndexFile(sourceFilePath);

            indexFile.Open();
            indexFile.Check();

            if (indexFile.IsValid)
            {
                indexFile.Read();

                Console.WriteLine(" Found " + indexFile.ArcFileEntries.Length + " archive file entries");

                for (int i = 0; i < indexFile.ArcFileEntries.Length; i++)
                {
                    IndexFile.ArcFileEntry arcFileEntry = indexFile.ArcFileEntries[i];
                    string archiveFilePath = Path.Combine(Path.GetDirectoryName(sourceFilePath), arcFileEntry.Name);
                    string archiveDestinationDirectoryPath = Path.Combine(destinationDirectoryPath, arcFileEntry.Name);

                    Console.WriteLine("Extracting archive file (" + archiveFilePath + ") [" +
                        (i + 1) + "/" + indexFile.ArcFileEntries.Length + "]");

                    Extract(archiveFilePath, archiveDestinationDirectoryPath);
                }
            }

            indexFile.Close();
        }

        private void Extract(string archiveFilePath, string destinationDirectoryPath)
        {
            if (File.Exists(archiveFilePath))
            {
                ArcFile arcFile = new ArcFile(archiveFilePath);

                arcFile.Open();
                arcFile.Check();

                if (arcFile.IsValid)
                {
                    arcFile.Read();

                    Directory.CreateDirectory(destinationDirectoryPath);

                    Console.WriteLine(" Found " + arcFile.ArcEntries.Length + " archive entries");

                    for (int i = 0; i < arcFile.ArcEntries.Length; i++)
                    {
                        ArcFile.ArcEntry arcEntry = arcFile.ArcEntries[i];
                        string arcEntryFilePath = Path.Combine(destinationDirectoryPath, arcEntry.TextIdentifier);

                        Console.WriteLine(" Extracting archive entry (" + arcEntryFilePath + ") [" +
                            (i + 1) + "/" + arcFile.ArcEntries.Length + "]");

                        arcFile.WriteEntryData(arcEntry, arcEntryFilePath);
                    }
                }

                arcFile.Close();
            }
            else
            {
                Console.WriteLine(" File not found");
            }
        }
    }
}
