﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private string _currentRom;
        private int _romProgress;
        private int _totalProgress;
        private string _logText = string.Empty;

        public RelayCommand CloseCommand { get; }

        public RelayCommand SettingsCommand { get; }

        public RelayCommand TransferToDeviceCommand { get; }

        public RelayCommand TransferToDatabaseCommand { get; }

        public RelayCommand RefreshDrivesCommand { get; }

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

        public string CurrentRom {
            get => _currentRom;
            set {
                _currentRom = value;
                RaisePropertyChanged();
            }
        }

        public int RomProgress {
            get => _romProgress;
            set {
                _romProgress = value;
                RaisePropertyChanged();
            }
        }

        public int TotalProgress {
            get => _totalProgress;
            set {
                _totalProgress = value;
                RaisePropertyChanged();
            }
        }

        public string LogText => _logText;
        public MainWindowViewModel(IOptionsManager options, IRomManager romManager) {
            _options = options;
            DatabasePath = _options.GetOption(Options.RomPath).GetValue();
            _romManager = romManager;
            _romManager.Progress += RomManagerOnProgress;
            _romManager.RomCompleted += RomManagerOnRomCompleted;

            DatabaseRoms = GetRoms(DatabasePath);

            RefreshDrivesCommandExecuted();

            CloseCommand = new RelayCommand(CloseCommandExecuted);
            SettingsCommand = new RelayCommand(SettingsCommandExecuted);
            TransferToDeviceCommand = new RelayCommand(TransferToDeviceCommandExecuted);
            TransferToDatabaseCommand = new RelayCommand(TransferToDatabaseCommandExecuted);
            RefreshDrivesCommand = new RelayCommand(RefreshDrivesCommandExecuted);

            Log("Wilkommen");
            for(int i = 0; i < 100; i++) {
                Log($"Wilkommen {i}");

            }
        }

        private void RomManagerOnRomCompleted(object sender, Events.RomEventArgs e) {
            Log($"{CurrentRom} fertig");
            CurrentRom = string.Empty;
        }

        private void RomManagerOnProgress(object sender, Events.ProgressEventArgs e) {
            if (string.IsNullOrWhiteSpace(CurrentRom)) {
                CurrentRom = e.CurrentRom;
                Log($"{CurrentRom} gestartet");
            }
            RomProgress = e.RomProgress;
            TotalProgress = e.TotalProgress;
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

        private void TransferToDeviceCommandExecuted() {
            Task.Run(async () => {
                await TransferRoms(DatabaseRoms, DrivePath);
                DriveRoms = GetRoms(DrivePath);
            });
        }

        private void TransferToDatabaseCommandExecuted() {
            Task.Run(async () => {
                await TransferRoms(DriveRoms, DatabasePath);
                DatabaseRoms = GetRoms(DatabasePath);
            });
        }

        private async Task TransferRoms(RomViewModel[] romRawList, string targetPath) {
            if(!Directory.Exists(targetPath)) {
                MessageBox.Show("Es wurde kein gültiger Pfad angegeben", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var romList = new List<IRom>();
            foreach (var rom in romRawList) {
                if (!rom.Selected) continue;
                romList.Add(rom.Rom);
            }

            if (romList.Count == 0) {
                MessageBox.Show("Es wurde keine Roms ausgewählt", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Log($"Kopieren nach {targetPath} wird initialisiert");
            await _romManager.TransferRoms(romList.ToArray(), targetPath);
            Log($"Alle Roms kopiert");
            
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

        private void Log(string message) {
            var newLine = _logText.Equals(string.Empty) ? string.Empty : "\r\n";
            _logText = $"{LogText}{newLine}{DateTime.Now:hh:mm:ss} {message}";
            RaisePropertyChanged(nameof(LogText));
        }
    }
}
 