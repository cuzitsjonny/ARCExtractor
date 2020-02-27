using System;
using System.IO;

namespace ARCExtractor
{
    public class ArcFile
    {
        public class ArcEntry
        {
            public uint Identifier { get; set; }
            public uint DataAddress { get; set; }
            public uint DataLength { get; set; }
            public uint Unknown0 { get; set; }
            public string TextIdentifier { get; set; }
            public byte[] Unknown1 { get; set; }
        }

        public static readonly uint BufferSize = 4096;
        public static readonly ulong Magic = 5832969526958728389;

        public ArcEntry[] ArcEntries { get; set; }
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

        public ArcFile(string filePath)
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
                uint headerAddress = fileReader.ReadUInt32();
                
                if (headerAddress > fileStream.Length)
                {
                    Console.WriteLine(" File invalid: Header address out of range");
                }
                else
                {
                    fileStream.Seek(headerAddress, SeekOrigin.Begin);

                    ulong magic = fileReader.ReadUInt64();

                    if (magic == Magic)
                    {
                        IsValid = true;
                    }
                    else
                    {
                        Console.WriteLine(" File invalid: Magic mismatch");
                    }
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
                    ArcEntries = new ArcEntry[fileReader.ReadInt32()];

                    for (int i = 0; i < ArcEntries.Length; i++)
                    {
                        ArcEntry arcEntry = ArcEntries[i] = new ArcEntry();

                        arcEntry.Identifier = fileReader.ReadUInt32();
                        arcEntry.DataAddress = fileReader.ReadUInt32();
                        arcEntry.DataLength = fileReader.ReadUInt32();
                        arcEntry.Unknown0 = fileReader.ReadUInt32();
                        arcEntry.TextIdentifier = IOUtility.ReadTString(fileReader);
                        arcEntry.Unknown1 = fileReader.ReadBytes(12);
                    }

                    while ((fileStream.Position % 32) != 0)
                    {
                        fileStream.Position++;
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

        public void WriteEntryData(ArcEntry arcEntry, string destinationFilePath)
        {
            uint leftToWrite = arcEntry.DataLength;
            byte[] buffer = new byte[BufferSize];
            Stream writeStream = File.OpenWrite(destinationFilePath);

            fileStream.Seek(arcEntry.DataAddress, SeekOrigin.Begin);

            while (leftToWrite > 0)
            {
                uint write = Math.Min(leftToWrite, BufferSize);

                fileStream.Read(buffer, 0, (int)write);
                writeStream.Write(buffer, 0, (int)write);

                leftToWrite -= write;
            }

            writeStream.Close();
        }
    }
}
