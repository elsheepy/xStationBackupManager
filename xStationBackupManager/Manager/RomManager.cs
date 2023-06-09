﻿using Autofac;
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
using xStationBackupManager.Enums;

namespace xStationBackupManager.Manager {
    internal class RomManager : IRomManager {
        private readonly ILifetimeScope _scope;
        private readonly string[] RomExtensions = new[] { ".bin", ".cue", ".iso" };
        private readonly string[] ZipExtensions = new[] { ".7z", ".zip", ".bzip2", ".gzip", ".tar", ".rar" };
        private readonly IRomCollectionAssigner[] _collectionAssigners;

        public event Events.ProgressEventHandler Progress;
        public event Events.RomEventHandler RomCompleted;

        public RomManager(ILifetimeScope scope, IRomCollectionAssigner[] collectionAssigners) {
            _scope = scope;
            _collectionAssigners = collectionAssigners;
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

        public IRomCollection[] AssignRomsToCollection(IRom[] roms, RomGroup group) {
            var assigner = _collectionAssigners.FirstOrDefault(x => x.RomGroup == group);
            if(assigner != null) {
                return assigner.AssignRomsToCollection(roms);
            }
            return null;
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
                MessageBox.Show(Resources.Localization.Resources.NoErrorsFound, Resources.Localization.Resources.CheckTitel, MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                MessageBox.Show($"{fixedDirs.Count} {Resources.Localization.Resources.ErrorsFound}:\r\n\r\n{String.Join("\r\n", fixedDirs)}", Resources.Localization.Resources.CheckTitel, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public Task RearrangeDrive(string drivePath, IRomCollection[] collections) {
            var moved = 0;
            foreach (var collection in collections) {
                RearrangeCollection(drivePath, collection, ref moved);
            }
            DeleteEmptyFolders(drivePath);

            MessageBox.Show($"{moved} {Resources.Localization.Resources.RomsMoved}!", Resources.Localization.Resources.RearrangedSD, MessageBoxButton.OK, MessageBoxImage.Information);

            return Task.CompletedTask;
        }

        private void RearrangeCollection(string path, IRomCollection collection, ref int moved) {
            path = $"{path}{collection.Name}\\".Replace("\\\\", "\\");
            Directory.CreateDirectory(path);
            foreach (var rom in collection.Roms) {
                var newPath = $"{path}{rom.Name}";
                if (rom.Path.Equals(newPath)) continue;
                Directory.Move(rom.Path, newPath);
                moved++;
            }
        }

        private void DeleteEmptyFolders(string path) {
            var dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            var dirs = dirInfo.GetDirectories();
            if(files.Length == 0 && dirs.Length == 0) {
                // delete
                Directory.Delete(path);
                return;
            }
            foreach(var dir in dirs) {
                DeleteEmptyFolders(dir.FullName);
            }
        }
    }
}
