using Autofac;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.Manager {
    internal class RomManager : IRomManager {
        private readonly ILifetimeScope _scope;
        private readonly string[] RomExtensions = new[] { ".bin", ".cue" };
        private readonly string[] ZipExtensions = new[] { ".7z" };

        public event Events.ProgressEventHandler Progress;
        public event Events.RomEventHandler RomCompleted;

        public RomManager(ILifetimeScope scope) {
            _scope = scope;
        }

        public IRom[] GetRoms(string path) {
            if (!Directory.Exists(path)) return new IRom[0];

            var result = new List<IRom>();
            foreach (var file in Directory.GetFiles(path)) {
                var fileInfo = new FileInfo(file);
                if (!ZipExtensions.Contains(fileInfo.Extension)) continue;
                var rom = _scope.Resolve<IRom>();
                rom.Name = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                rom.Path = file;
                result.Add(rom);
            }
            foreach (var directory in Directory.GetDirectories(path)) {
                var isRom = false;
                foreach (var file in Directory.GetFiles(directory)) {
                    if (RomExtensions.Contains(new FileInfo(file).Extension)) {
                        isRom = true;
                        break;
                    }
                }
                if(!isRom) {
                    var roms = GetRoms(directory);
                    if (roms.Length > 0) result.AddRange(roms);
                    continue;
                }
                var rom = _scope.Resolve<IRom>();
                var fileInfo = new DirectoryInfo(directory);
                rom.Name = fileInfo.Name;
                rom.Path = directory;
                result.Add(rom);
            }
            return result.OrderBy(x => x.Name).ToArray();
        }

        private IRom _currentRom;
        private int _romsCompleted;
        private int _romsTotal;

        public async Task<bool> TransferRoms(IRom[] roms, string target) {
            if (!Directory.Exists(target)) return false;

            _romsCompleted = 0;
            _romsTotal = roms.Length;

            foreach (var rom in roms) {
                _currentRom = rom;
                if(Directory.Exists(rom.Path)) {
                    // Directory with Rom
                    Progress?.Invoke(this, new Events.ProgressEventArgs(0, GetTotalProgress(0), rom.Name));
                    var info = new DirectoryInfo(rom.Path);
                    var targetPath = $"{target}{info.Name}\\";
                    Directory.CreateDirectory(targetPath);
                    var files = Directory.GetFiles(rom.Path);
                    for(int i = 0; i < files.Length; i++) {
                        var file = files[i];
                        var fileInfo = new FileInfo(file);
                        File.Copy(file, $"{targetPath}{fileInfo.Name}");
                        var progress = 100 * (i + 1) / files.Length;
                        Progress?.Invoke(this, new Events.ProgressEventArgs(progress, GetTotalProgress(progress), rom.Name));
                    }
                } else {
                    // File Rom
                    var fileInfo = new FileInfo(rom.Path);
                    SevenZipCompressor.SetLibraryPath(@"x64\7z.dll");
                    var extractor = new SevenZipExtractor(rom.Path);
                    extractor.Extracting += Extractor_Extracting;
                    try {
                        await extractor.ExtractArchiveAsync($"{target}{rom.Name}");
                    } catch(Exception ex) {
                        var msg = ex.Message;
                    }
                    _romsCompleted++;
                    RomCompleted?.Invoke(this, new Events.RomEventArgs(rom));
                }
            }

            return true;
        }

        private void Extractor_Extracting(object? sender, ProgressEventArgs e) {
            Progress?.Invoke(this, new Events.ProgressEventArgs(e.PercentDone, GetTotalProgress(e.PercentDone), _currentRom.Name));
        }

        private int GetTotalProgress(int romProgress) {
            return (_romsCompleted * 100 + romProgress) / _romsTotal;
        }
    }
}
