using System;
using System.IO;

namespace ARCExtractor
{
    public class IndexFile
    {
        public class ArcEntry
        {
            public uint Identifier { get; set; }
            public uint DataAddress { get; set; }
            public uint DataLength { get; set; }
            public uint Unknown0 { get; set; }
        }

        public class ArcFileEntry
        {
            public byte[] Unknown0 { get; set; }
            public string Name { get; set; }
            public uint Unknown1 { get; set; }
            public uint Unknown2 { get; set; }
            public ArcEntry[] ArcEntries { get; set; }
        }

        public static readonly ulong Magic = 4123828222553821653;

        public ArcFileEntry[] ArcFileEntries { get; set; }
        public string BuildVersionId { get; set; }
        public string BuildMachineName { get; set; }
        public string BuildBranch { get; set; }
        public string BuildChangeList { get; set; }
        public string BuildDateTime { get; set; }
        public string BuildLabel { get; set; }
        public uint BuildCheckSum { get; set; }
        public byte[] Unknown0 { get; set; }
        public bool IsValid { get; protected set; }
        public bool IsClosed { get; protected set; }

        private string filePath;
        private Stream fileStream;
        private BinaryReader fileReader;

        public IndexFile(string filePath)
        {
            this.filePath = filePath;
            this.IsValid = false;
            this.IsClosed = true;
        }

        public void Open()
        {
            if (IsClosed)
            {
                fileStream = File.OpenRead(filePath);
                fileReader = new BinaryReader(fileStream);
                IsClosed = false;
            }
            else
            {
                throw new InvalidOperationException("File already open");
            }
        }

        public void Close()
        {
            if (!IsClosed)
            {
                fileReader.Close();
                fileReader = null;
                fileStream = null;
                IsClosed = true;
            }
            else
            {
                throw new InvalidOperationException("File already closed");
            }
        }

        public void Check()
        {
            if (!IsClosed)
            {
                ulong magic = fileReader.ReadUInt64();
                ulong padding = fileReader.ReadUInt64();

                if (magic == Magic)
                {
                    if (padding == 0)
                    {
                        IsValid = true;
                    }
                    else
                    {
                        Console.WriteLine(" File invalid: Padding contains data");
                    }
                }
                else
                {
                    Console.WriteLine(" File invalid: Magic mismatch");
                }
            }
            else
            {
                throw new InvalidOperationException("File closed");
            }
        }

        public void Read()
        {
            if (!IsClosed)
            {
                if (IsValid)
                {
                    ArcFileEntries = new ArcFileEntry[fileReader.ReadInt32()];

                    for (int i = 0; i < ArcFileEntries.Length; i++)
                    {
                        ArcFileEntry arcFileEntry = ArcFileEntries[i] = new ArcFileEntry();

                        arcFileEntry.Unknown0 = fileReader.ReadBytes(24);
                        arcFileEntry.Name = IOUtility.ReadString(fileReader, 12);

                        arcFileEntry.Unknown1 = fileReader.ReadUInt32();
                        arcFileEntry.Unknown2 = fileReader.ReadUInt32();
                        arcFileEntry.ArcEntries = new ArcEntry[fileReader.ReadInt32()];

                        for (int k = 0; k < arcFileEntry.ArcEntries.Length; k++)
                        {
                            ArcEntry arcEntry = arcFileEntry.ArcEntries[k] = new ArcEntry();

                            arcEntry.Identifier = fileReader.ReadUInt32();
                        }

                        for (int k = 0; k < arcFileEntry.ArcEntries.Length; k++)
                        {
                            ArcEntry arcEntry = arcFileEntry.ArcEntries[k];

                            arcEntry.DataAddress = fileReader.ReadUInt32();
                            arcEntry.DataLength = fileReader.ReadUInt32();
                            arcEntry.Unknown0 = fileReader.ReadUInt32();
                        }
                    }

                    BuildVersionId = IOUtility.ReadString(fileReader, 20);
                    BuildMachineName = IOUtility.ReadString(fileReader, 32);
                    BuildBranch = IOUtility.ReadString(fileReader, 20);
                    BuildChangeList = IOUtility.ReadString(fileReader, 16);
                    BuildDateTime = IOUtility.ReadString(fileReader, 32);
                    BuildLabel = IOUtility.ReadString(fileReader, 256);
                    BuildCheckSum = fileReader.ReadUInt32();
                    Unknown0 = fileReader.ReadBytes(124);
                }
                else
                {
                    throw new InvalidOperationException("File invalid");
                }
            }
            else
            {
                throw new InvalidOperationException("File closed");
            }
        }
    }
}
