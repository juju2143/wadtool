# MTVMG/Music 2000 directory structure

## PSX CD root

| File | Description |
|------|-------------|
| SLUS_010.06 | Entry point for the bootloader
| SYSTEM.CNF | System configuration for the bootloader
| GAME.EXE | Main executable
| WADS/ | Most of the assets of the software packed in a WAD archive, can be opened with wadtool

## WAD

| Directory | Description |
|-----------|-------------|
| BOOT | Misc files used for the software itself
| [HELP](#helphelpus) | Help files for Music 2000
| [HELPUS](#helphelpus) | Help files for MTV Music Generator
| M1* | Seems to be holdovers from Music 1, mostly .lnk files with invalid offsets
| [N](#n) | Sound samples
| PERF | Example save files

### HELP/HELPUS

Text files found in the help feature, encoded in Windows-1252.

Music 2000 has subdirectories for 5 different languages: `english`, `french`, `german`, `italian`, and `spanish`.

MTV Music Generator uses `HELPUS` instead of `HELP`, and only have the subdirectory for English.

A tilde followed with a letter will show the graphic of a PS1 controller button. `BOOT/buttons.txt` has this message with the following table:

> Type tilda (~) and a letter. ~m would give you triangle.
> 
> Lazy Paul has only drawn the ones with an asterisk.

|                |   |   |
|----------------|---|---|
| Select         | a | * |
| Left analogue  | b |   |
| Right analogue | c |   |
| Start          | d | * |
| Up             | e |   |
| Right          | f |   |
| Down           | g |   |
| Left           | h |   |
| L2             | i | * |
| R2             | j | * |
| L1             | k | * |
| R1             | l | * |
| Triangle       | m | * |
| Circle         | n | * |
| Cross          | o | * |
| Square         | p | * |

Some intersting files:

| File      | Description |
|-----------|-------------|
| `101/000` | Welcome text shown on boot.

### N

This is the directory with all of the sound samples in .vag format. They're sorted by bitrate, then by category and subcategory.

|   | Bitrate |
|---|-------- |
|`B`| 11 kHz  |
|`D`| 22 kHz  |
|`F`| 44 kHz  |