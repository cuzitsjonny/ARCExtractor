# ARCExtractor
## Introduction
This tool can extract a single archive file (.arc) or multiple ones, if an index file (.ind) is provided.
More information about these file types can be found [here](https://docs.google.com/document/d/1Kn2KrpXRizShMpyRmibUjYktJovF-_EQ36Mbht5q7sY).
## Usage
In any case an input file (source) and an output directory (destination) must be provided.
### Single archive file
If an archive file is provided as the input file, the tool will extract all entries from the archive directly into the output directory.

Example:
```sh
ARCExtractor.exe "C:\Program Files (x86)\Steam\steamapps\common\Loadout\Data\01DBC177.ARC" "C:\Extracted"
```
Result:
```
C:\
└── Extracted
        ├── 0x1abd63ca
        ├── 0x1c2e041
        ├── 0x1c236c91
        └── ...
```
### Index file
If an index file is provided as the input file, the tool will create a subdirectory in the output directory for each archive file registered in the index and extract the respective entries into the respective subdirectories. It will try to find the archive files in the same directory as the provided index file.

Example:
```sh
ARCExtractor.exe "C:\Program Files (x86)\Steam\steamapps\common\Loadout\Data\index.ind" "C:\Extracted"
```
Result:
```
C:\
└── Extracted
        ├── 01DBC177.ARC
        │       ├── 0x1abd63ca
        │       ├── 0x1c2e041
        │       ├── 0x1c236c91
        │       └── ...
        ├── 4ED964ED.ARC
        │      └── ...
        └── ...
```
