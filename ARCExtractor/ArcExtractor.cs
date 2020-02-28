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
                ArchiveFile archiveFile = new ArchiveFile(archiveFilePath);

                archiveFile.Open();
                archiveFile.Check();

                if (archiveFile.IsValid)
                {
                    archiveFile.Read();

                    Directory.CreateDirectory(destinationDirectoryPath);

                    Console.WriteLine(" Found " + archiveFile.ArcEntries.Length + " archive entries");

                    for (int i = 0; i < archiveFile.ArcEntries.Length; i++)
                    {
                        ArchiveFile.ArcEntry arcEntry = archiveFile.ArcEntries[i];
                        string arcEntryFilePath = Path.Combine(destinationDirectoryPath, arcEntry.TextIdentifier);

                        Console.WriteLine(" Extracting archive entry (" + arcEntryFilePath + ") [" +
                            (i + 1) + "/" + archiveFile.ArcEntries.Length + "]");

                        archiveFile.WriteEntryData(arcEntry, arcEntryFilePath);
                    }
                }

                archiveFile.Close();
            }
            else
            {
                Console.WriteLine(" File not found");
            }
        }
    }
}
