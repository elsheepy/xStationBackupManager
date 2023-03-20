using Autofac;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using System.Windows;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.Manager {
    internal class RomManager : IRomManager {
        private readonly ILifetimeScope _scope;
        private readonly string[] RomExtensions = new[] { ".bin", ".cue" };
        private readonly string[] ZipExtensions = new[] { ".7z", ".zip", ".bzip2", ".gzip", ".tar", ".rar" };

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
                    if (target[target.Length-1] != '\\') { target += "\\"; }
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

        public async Task CheckAndFixDirectory(string directory) {
            var fixedDirs = new List<string>();
            var directories = Directory.GetDirectories(directory);
            foreach (var subDirectory in directories) {
                var subDirectories = Directory.GetDirectories(subDirectory);
                foreach (var subDirectory2 in subDirectories) {
                    try {
                        foreach(var dir in Directory.GetDirectories(subDirectory2)) {
                            var dInfo = new DirectoryInfo(dir);
                            Directory.Move(dir, $"{subDirectory}\\{dInfo.Name}");
                        }
                        foreach (var file in Directory.GetFiles(subDirectory2)) {
                            var info = new FileInfo(file);
                            File.Move(file, $"{subDirectory}\\{info.Name}");
                        }
                        fixedDirs.Add(new DirectoryInfo(subDirectory2).Name);
                        Directory.Delete(subDirectory2);
                    } catch(Exception ex) {
                        var msg = ex.Message;
                    }
                }
            }
            if(fixedDirs.Count == 0) {
                MessageBox.Show("Keine Fehler gefunden.", "Check & Fix", MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                MessageBox.Show($"{fixedDirs.Count} Fehler gefunden:\r\n\r\n{String.Join("\r\n", fixedDirs)}", "Check & Fix", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
