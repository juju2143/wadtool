# IND/WAD file format and notes

From the [no$psx docs](http://problemkaputt.de/psx-spx.htm#cdromfilearchiveindwadmtvmusicgenerator), annotated and formatted

### ECTS.IND contains FOLDER info:

| Offset | Size  | |
|--------|-------|-|
| 0000h  | 20h   | Name/ID ("Music 2", zeropadded)
| 0020h  | 4     | Unknown (110000h)
| 0024h  | 4     | Filesize-1000h (size excluding last 1000h-byte padding)
| 0028h  | 4     | Unknown (17E0h)
| 002Ch  | 4     | Unknown (5)
| 0030h  | N*10h | Folder List, starting with Root in first 10h-byte
| 2CF0h  | 4     | Small Padding (34h-filled)
| 2CF4h  | 1000h | Final Padding (34h-filled)

Folder List entries that refer to Child Folders in ECTS.IND:
| Offset | Size | |
|--------|------|-|
| 000h   | 8    | Folder Name ("EXTRA*~*", zeropadded if less than 8) ("" for root)
| 008h   | 2    | Self-relative Index to first Child folder (positive)
| 00Ah   | 2    | Number of Child Folders (0..7FFFh)
| 00Ch   | 4    | Always 0007FFFFh (19bit Offset=7FFFFh, plus 13bit Size=0000h)

Folder List entries that refer to File Folders in ECTS.WAD:
| Offset | Size | |
|--------|------|-|
| 000h   | 8    | Folder Name ("EXTRA*~*", zeropadded if less than 8)
| 008h   | 2    | Self-relative Index to Parent folder (negative)
| 00Ah   | 2    | Number of Child Folders (always 8000h=None)
| 00Ch   | 4    | Offset and Size in ECTS.WAD

The 32bit "Offset and Size" entry consists of:
| Bits  | Size (bits) | |
|-------|-------------|-|
| 0-18  | 19          | Offset/800h in ECTS.WAD
| 19-31 | 13          | Size/800h-1 in ECTS.WAD

```
 0                   1                   2                   3
 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|       Size >> 11        |            Offset >> 11             |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

### ECTS.WAD contains FILE info and actual FILE data:

There are several File Folders (at the locations specified in ECTS.IND).

The separate File Folders look as so:
| Offset | Size  | |
|--------|-------|-|
| 000h   | 4     | Number of files (N)
| 004h   | N*10h | File List
| ...    | ..    | 34h-Padding to 800h-byte boundary
| ...    | ..    | File Data area

File List entries:
| Offset | Size | |
|--------|------|-|
| 000h   | 8    | File Name ("NAMELIST", "ACIDWO~1", etc.) (00h-padded if shorter)
| 008h   | 4    | Offset/800h (always from begin of WAD, not from begin of Folder)
| 00Ch   | 4    | Filesize in bytes

The first file in each folder is called "NAMELIST" and contains this:
| Offset | Size  | |
|--------|-------|-|
| 000h   | 20h   | Long Name for Parent Folder (eg. "Backgrounds", zeropadded)
| 020h   | 20h   | Long Name for this Folder   (eg. "Extra 1", zeropadded)
| 040h   | N*20h | Long Names for all files in folder (except for NAMELIST itself)

For example, Long name for "ACIDWO~1" would be "Acidworld". Short names are uppercase, max 8 chars, without spaces (with "~N" suffix if the long name contains spaces or more than 8 chars). Many folder names are truncated to one char (eg. "D" for Long name "DTex"), in such cases short names CAN be lowercase (eg. "z" for Long name "zTrans").

The Long Names are scattered around in the NAMELIST files in ECTS.WAD file, so they aren't suitable for lookup (unless when loading all NAMELIST's).

## Notes

- It's andy.ind/wad on my American copy of MTVMG
- The file name decoder function currently trims zero bytes and replaces any that's left in the middle of the name with a point (`.`), seems to be an extension separator
- We'll probably lose information about short names if we extract files to filesystem and pack them again, check whether MTVMG minds the short names being different?
- A long name for a child folder only exist if it contains at least a file folder
- Someone apparently matched a Music 2002 PC WAD with Music 2000 on PS1, so it seems to be compatible across most versions
- Pratically, every size field (except file size in WAD) is redundant with item count, MTVMG might rely on this though
- Every file and folder in WAD starts on a 2048 byte boundary (because offset is always stored as a multiple of 2048 as a CD has 2048 byte sectors)