# LocDbConverterConsole

This is a simple console application, that converts configuration files for model railroad digital control stations.
It converts a lokomotive.cs2 file (configuration for Central Station 2/3) to a locomotive.z21loco file (configuration for Roco/Fleischmann Z21).

The code is available under GNU General Public License.


The following commands are available:

    -h / -?    Help menu
    
    -c <file>  Converts a given locomotive config file from CS2/CS3-format to Z21-format.
    
    -a         Auto convert locomotive.cs2 file according to settings in App.config file.
    
    -m         Manually force convert locomotive.cs2 file (only when auto-mode ist ON.
    
...or you can simply drop a locomotive.cs2 file on the .exe to be converted.
  
    
The settings for convertion are made in the mappings.xml file (mapping of the function icons and names) and the LocDbConverterConsole.config file (paths for auto convertion).

    
    
Current version Beta 1.2 is available for download:
    
    Windows Arm64
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/win-arm64.zip
    
    Windows x64
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/win-x64.zip
    
    Windows x86
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/win-x86.zip
    
    OSX Arm64 (Silicon)
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/osx-arm64.zip
    
    OSX x64 (Intel)
     https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/osx-x64.zip    
    
    Linux x64:
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/linux-x64.zip
  
