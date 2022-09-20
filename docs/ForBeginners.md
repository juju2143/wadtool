# Beginner's guide
### (aka "Non-nerd Starter Pack)

Currently `wadtool` is command line only -
no nice graphical interface with nice buttons to click
and process your nice "Music 2000" assets... yet!

Still, it's much easier to use than you might think!
This guide here is supposed to guide you from very beginning!

### Step 1 - Preparation / Downloading
Please choose right `wadtool` for your system,
*(e.g. choose Windows download if you got Windows etc etc)* otherwise it won't run!

I assume you got your legal copy of "MTV Music Generator" / "Music 2000" either
already in your disk drive, or as a __.iso__ OR __.bin__ + __.cue__ files  
In particular, what we need is 2 files from inside disk:  
__ANDY.IND__ and __ANDY.WAD__ - both located in "WADS" folder!  
(that's where our music app stores all sounds and graphics!)

Accessing the disk on Windowds 10 and newer ones is as easy
as double-clicking the image file (preferably __.cue__) -
This should mount your disk into virtual drive that you can access inside `My PC` folder;

other than that use tool like ultraISO, as it offers extracting functionality;

It's good idea to place all those 3 files ( `wadtool`, `ANDY.IND`, `ANDY.WAD`) in same folder for ease of use! 

### Step 2 Browsing Sample/Graphic library (through terminal!):

Now that you got all important files together, it's time to start using `wadtool`!  
Unfortunately, double clicking the app makes it pop for split-second and dissapear with no word.  
Again, that's command-line app, so we need to use terminal!

You probably seen or heard about `cmd` or `powershell` -
if you're using Windows, chances are you got one of those -
It doesn't matter which terminal you use, but I personally recommend Powershell 
since it understands some Linux comamnds too (trust me it gets handy heheh)

Easy trick to open up terminal on Windows in the folder we're in is to type name of terminal app we want to use inside address bar!


![explorer_YiwAmQQ26C](https://user-images.githubusercontent.com/66220663/190938763-5ecbeaa8-763b-486b-ba8e-d28ea43fcbb1.png)

After you open it up, type at least letter `w` and press tab -
that should "autocomplete" what we just wrote to *something* like `wadtool.exe`
(Powershell shows `.\wadtool.exe` - it's exactly same thing as `wadtool.ext` so no worries!)
when you press enter, we run the app, but this time it doesnt automatically closes down the terminal, we can read what it says!

----- TO ADD: -----
- opening and finishing disk image using mkpsxiso: https://github.com/Lameguy64/mkpsxiso
- merging `.bin` files because correct PSX image of MTVMG is split into 2 `.bin` files 
- link to M2K guide
- add more images
- link some `.tim` & `.vag` tools

```
wadtool --help
wadtool tree -i D:\wads\andy.ind -w D:\wads\andy.wad
wadtool ls -i D:\wads\andy.ind -w D:\wads\andy.wad
wadtool extract --help
```
