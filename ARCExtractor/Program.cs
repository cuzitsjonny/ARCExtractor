using System;
using System.IO;

namespace ARCExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFilePath;
            string destinationDirectoryPath;

            if (args.Length > 0)
            {
                sourceFilePath = args[0];
            }
            else
            {
                throw new ArgumentException("args[0] - source file path missing");
            }

            if (args.Length > 1)
            {
                destinationDirectoryPath = args[1];
            }
            else
            {
                throw new ArgumentException("args[1] - destination directory path missing");
            }

            if (sourceFilePath != null && destinationDirectoryPath != null)
            {
                ArcExtractor extractor = new ArcExtractor();
                string extension = Path.GetExtension(sourceFilePath);

                switch (extension.ToLower())
                {
                    case ".arc":
                        extractor.ExtractSingleArchiveFile(sourceFilePath, destinationDirectoryPath);
                        break;
                    case ".ind":
                        extractor.ExtractFromIndexFile(sourceFilePath, destinationDirectoryPath);
                        break;
                    default:
                        throw new ArgumentException("args[0] - source file path is not a .arc or .ind file");
                }

                Console.WriteLine("Extraction process done. Press any key to continue . . .");
                Console.ReadKey();
            }
        }
    }
}
