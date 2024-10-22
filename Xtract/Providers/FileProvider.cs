using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;

namespace Xtract.Providers
{
  public class FileProvider
  {
    public DefaultFileProvider _fileProvider { get; private set; }
    private readonly string _path;
    private readonly EGame _ueVersion;

    public FileProvider(string path, EGame ueVersion)
    {
      _path = path;
      _ueVersion = ueVersion;
    }

    /// <summary>
    /// Initializes the FileProvider with the specified game directory and unreal engine version.
    /// </summary>
    public async Task Initialize()
    {
      try
      {
        _fileProvider = new DefaultFileProvider(_path, SearchOption.AllDirectories, true, new VersionContainer(_ueVersion));
        _fileProvider.Initialize();
        _fileProvider.LoadLocalization();

        // TODO: Mappings & aes keys

        var keysCount = _fileProvider.Keys.Count;
        if (keysCount == 0)
        {
          throw new Exception("No keys found.");
        }

        Console.WriteLine($"FileProvider initialized successfully with {keysCount} {(keysCount == 1 ? "key" : "keys")}.");

                Parallel.ForEach(_fileProvider.MountedVfs, vfs =>
                {
                    Console.WriteLine($"Successfully mounted {vfs.Name} at {vfs.MountPoint}.");
                });
      }
      catch (FileNotFoundException ex) 
      {
        throw new FileNotFoundException("Game directory not found.", ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new UnauthorizedAccessException("Game directory not accessible.", ex);
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new DirectoryNotFoundException("Game directory not found.", ex);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to initialize FileProvider.", ex);
      }
    }
  }
}