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

- [.NET 6 or later](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- A legitimate copy[*!*](https://www.youtube.com/watch?v=dQw4w9WgXcQ) of one of the above titles

### Instructions

```
dotnet build
```

## Usage

```
# display commands and options you can use:
wadtool --help
# show's all directories in tree style
wadtool tree -i D:\wads\andy.ind -w D:\wads\andy.wad
# show current single directory
wadtool ls -i D:\wads\andy.ind -w D:\wads\andy.wad
# display help screen for "extract" command
wadtool extract --help
# extract all assets! 
wadtool extract -i D:\wads\andy.ind -w D:\wads\andy.wad
```

## Download

| Platform | Nightly |
|----------|---------|
| Windows (x64) | [Latest](https://nightly.link/juju2143/wadtool/workflows/main/master/nightly-win-x64.zip)
| macOS (x64) | [Latest](https://nightly.link/juju2143/wadtool/workflows/main/master/nightly-osx-x64.zip)
| Linux (x64) | [Latest](https://nightly.link/juju2143/wadtool/workflows/main/master/nightly-linux-x64.zip)
| NuGet package | [Latest](https://nightly.link/juju2143/wadtool/workflows/main/master/nightly-nupkg.zip)
## Links

- [Feature list](https://github.com/juju2143/wadtool/discussions/1)
- [Documentation](https://github.com/juju2143/wadtool/wiki)
- [Submit a bug report](https://github.com/juju2143/wadtool/issues/new)
- [Discuss about it and get help](https://github.com/juju2143/wadtool/discussions)
- [Music 2000 Community Discord Server](https://discord.gg/n8DNzxQ)
