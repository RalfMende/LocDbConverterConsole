# LocDbConverterConsole

This is a simple console application, that converts configuration files for model railroad digital control stations.
It converts a lokomotive.cs2 file (configuration for Maerklin CS2/CS3/CS3+ / SRSEII) to a locomotive.z21loco file (configuration for Roco/Fleischmann Z21-Mobile-App).
To do this, you can either specify the path or the IP-address/Hostname of the Lokomotive.cs file. The Program will connect the directly access the file for convertion.

The code is available under GNU General Public License.



The following commands are available:
            
            h / ?  Help menu
            c      Convert Lokomotive.cs2 file
            a 5    Auto convert Lokomotive.cs2 file according to settings in App.config file. Opt. Timing in [min].
            f      Force convert of all entries in Lokomotive.cs2 file
            x      Exit the program

Also the following Arguments are available

            --help / -h    Help menu
            --importpath=  Sets local path or IP address, where Lokomotive.cs2 file and images can be found.
            --exportpath=  Sets local path to where converted files are stored.
    
The settings for convertion are made in the mappings.xml file (mapping of the function icons and names) and the LocDbConverterConsole.config file (paths for auto convertion).

    
    
Current version v0.2.1 (beta) is available for download:
    
    Windows x64
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/win-x64.zip
    
    Windows x86
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/win-x86.zip
      
    Windows ARM64
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/win-arm64.zip    
    
    OSX x64 (Intel)
     https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/osx-x64.zip  
     
    OSX ARM64 (Silicon)
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/osx-arm64.zip    
    
    Linux x64 (.Net-Runtime self-contained)
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/linux-x64.zip

    Linux ARM64 (.Net-Runtime self-contained)
      https://github.com/RalfMende/LocDbConverterConsole/tree/main/publish/linux-arm64.zip
