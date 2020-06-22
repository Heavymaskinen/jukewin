# J.U.K.E. for Windows
## Description
This is an ongoing sparetime project consisting of a Jukebox-application.
The basic functionality is:
* Add MP3-files to a library and save the library 
* Let the user enqueue songs from the library (through various types of UI)
* Make it impossible to skip (except for admins)
* Make it possible to edit the library and rename songs/albums (in tags)

This version has been made using TDD and my attempt at Clean Architecture.

## Branches
### master
Contains the Windows-version, which is the most polished one so far.
Windows Media Player is used for playback and WPF is used for GUI.

### core_version
Most of the standard-version ported over to .NET Core. This version experiments with different kinds of UI, 
including an attempt at an elaborate console GUI. The plan is to make a client/server version, where the client can be an app (done in Xamarin).

### uwp_version
A port to UWP. This has been stale for a while.
