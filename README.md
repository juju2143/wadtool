# wadtool

A collection of tools to extract files from an IND/WAD package for MTV Music Generator, Music 2000 and others, eventually a comprehensive swiss knife toolkit to easily hack and mod your copy of the game.

## Compatible titles

- MTV Music Generator (PS1)
- Music 2000 (PS1)
- Music 2000 (PC)
- Music 2002 (PC)
- ...

## Compilation

### Ingredients

- .NET 6 or later
- A legitimate copy of one of the above titles

### Instructions

```
dotnet build
```

## Usage

```
wadtool --help
wadtool tree -i D:\wads\andy.ind -w D:\wads\andy.wad
wadtool ls -i D:\wads\andy.ind -w D:\wads\andy.wad
wadtool extract -i D:\wads\andy.ind -w D:\wads\andy.wad -o Wowww.jo3 D/B/B/WOWWW
```

## TODO

- [ ] Ability to unpack and repack whole tree
- [ ] Nice cross-platform GUI
- [ ] Find and test every version of MTVMG this should be compatible with