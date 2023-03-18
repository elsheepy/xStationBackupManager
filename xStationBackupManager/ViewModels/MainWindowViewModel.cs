using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.ViewModels {
    internal class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
        private readonly IOptionsManager _options;
        private readonly IRomManager _romManager; 

        private RomViewModel[] _databaseRoms;
        private RomViewModel[] _driveRoms;
        private string[] _drives;
        private string _databasePath;
        private string _drivePath;

        public RelayCommand CloseCommand { get; }

        public RelayCommand SettingsCommand { get; }

        public string DatabasePath {
            get => _databasePath;
            set {
                _databasePath = value;
                RaisePropertyChanged();
            }
        }

        public string DrivePath {
            get => _drivePath;
            set {
                if (_drivePath != null && _drivePath.Equals(value)) return;
                _drivePath = value;
                RaisePropertyChanged();
                DriveRoms = GetRoms(_drivePath);
            }
        }

        public RomViewModel[] DatabaseRoms {
            get => _databaseRoms;
            set {
                _databaseRoms = value;
                RaisePropertyChanged();
            }
        }

        public RomViewModel[] DriveRoms {
            get => _driveRoms;
            set {
                _driveRoms = value;
                RaisePropertyChanged();
            }
        }

        public string[] Drives {
            get => _drives;
            set {
                _drives = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(IOptionsManager options, IRomManager romManager) {
            _options = options;
            DatabasePath = _options.GetOption(Options.RomPath).GetValue();
            CloseCommand = new RelayCommand(CloseCommandExecuted);
            SettingsCommand = new RelayCommand(SettingsCommandExecuted);
            _romManager = romManager;

            DatabaseRoms = GetRoms(DatabasePath);

            RefreshDrivesCommandExecuted();
        }

        private RomViewModel[] GetRoms(string path) {
            var roms = _romManager.GetRoms(path);
            var romVms = new RomViewModel[roms.Length];
            for (int i = 0; i < roms.Length; i++) {
                romVms[i] = new RomViewModel(roms[i]);
            }
            return romVms;
        }

        private void CloseCommandExecuted() {
            Application.Current.Shutdown();
        }

        private void SettingsCommandExecuted() {

        }

        private void RefreshDrivesCommandExecuted() {
            var drives = DriveInfo.GetDrives();
            var result = new List<string>();
            foreach(var drive in drives) {
                if (drive.DriveType != DriveType.Removable) continue;
                result.Add(drive.Name);
            }
            Drives = result.ToArray();
        }
    }
}
 