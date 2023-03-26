using Autofac;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;
using xStationBackupManager.Views;

namespace xStationBackupManager.ViewModels {
    internal class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, ISelectRomsCommandProvider {
        private readonly IOptionsManager _options;
        private readonly IRomManager _romManager;
        private readonly IOption _databasePathOption;
        private readonly ILifetimeScope _scope;

        private RomRootViewModel[] _databaseRoms;
        private RomRootViewModel[] _driveRoms;
        private string[] _drives;
        private string _databasePath;
        private string _drivePath;
        private string _currentRom;
        private int _romProgress;
        private int _totalProgress;
        private string _logText = string.Empty;
        private RomGroup _driveRomGroup;
        private RomGroup _databaseRomGroup;

        public RelayCommand CloseCommand { get; }

        public RelayCommand SettingsCommand { get; }

        public RelayCommand AboutCommand { get; }

        public RelayCommand TransferToDeviceCommand { get; }

        public RelayCommand TransferToDatabaseCommand { get; }

        public RelayCommand RefreshDrivesCommand { get; }

        public RelayCommand<bool> SelectAllRomsCommand { get; }

        public RelayCommand<bool> DeselectAllRomsCommand { get; }

        public RelayCommand<bool> SelectDeltaRomsCommand { get; }

        public RelayCommand<bool> CheckAndFixDirectoryCommand { get; }

        public RelayCommand RearrangeRomsCommand { get; }

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
                DriveRoms = GetRoms(_drivePath, DriveRomGroup);
            }
        }

        public RomRootViewModel[] DatabaseRoms {
            get => _databaseRoms;
            set {
                _databaseRoms = value;
                RaisePropertyChanged();
            }
        }

        public RomGroup DatabaseRomGroup {
            get => _databaseRomGroup;
            set {
                _databaseRomGroup = value;
                RaisePropertyChanged();
                DatabaseRoms = GetRoms(DatabasePath, DatabaseRomGroup);
            }
        }

        public RomRootViewModel[] DriveRoms {
            get => _driveRoms;
            set {
                _driveRoms = value;
                RaisePropertyChanged();
            }
        }

        public RomGroup DriveRomGroup {
            get => _driveRomGroup;
            set {
                _driveRomGroup = value;
                RaisePropertyChanged();
                DriveRoms = GetRoms(DrivePath, DriveRomGroup);
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

        public MainWindowViewModel(IOptionsManager options, IRomManager romManager, ILifetimeScope scope) {
            _scope = scope;
            _options = options;
            _databasePathOption = _options.GetOption(Options.RomPath);
            _romManager = romManager;
            _romManager.Progress += RomManagerOnProgress;
            _romManager.RomCompleted += RomManagerOnRomCompleted;
            _databasePathOption.Changed += OnDatabasePathOptionChanged;

            DatabasePath = _databasePathOption.GetValue();
            DatabaseRoms = GetRoms(DatabasePath, DatabaseRomGroup);

            RefreshDrivesCommandExecuted();

            CloseCommand = new RelayCommand(CloseCommandExecuted);
            SettingsCommand = new RelayCommand(SettingsCommandExecuted);
            AboutCommand = new RelayCommand(AboutCommandExecuted);
            TransferToDeviceCommand = new RelayCommand(TransferToDeviceCommandExecuted);
            TransferToDatabaseCommand = new RelayCommand(TransferToDatabaseCommandExecuted);
            RefreshDrivesCommand = new RelayCommand(RefreshDrivesCommandExecuted);
            SelectAllRomsCommand = new RelayCommand<bool>(SelectAllRomsCommandExecuted);
            DeselectAllRomsCommand = new RelayCommand<bool>(DeselectAllRomsCommandExecuted);
            SelectDeltaRomsCommand = new RelayCommand<bool>(SelectDeltaRomsCommandExecuted);
            CheckAndFixDirectoryCommand = new RelayCommand<bool>(CheckAndFixDirectoryCommandExecuted);
            RearrangeRomsCommand = new RelayCommand(RearrangeRomsCommandExecuted);

            Log(Resources.Localization.Resources.Welcome);
        }

        private void OnDatabasePathOptionChanged(object? sender, EventArgs e) {
            DatabasePath = _databasePathOption.GetValue();
            DatabaseRoms = GetRoms(DatabasePath, DatabaseRomGroup);
        }

        public void OnViewActivated() {
            // Wir prüfen ob wir ein Rom Verzeichnis haben, wen nicht öffnen wir die Optionen
            var romDirOption = _options.GetOption(Options.RomPath);
            if (string.IsNullOrWhiteSpace(romDirOption.GetValue())) {
                SettingsCommandExecuted();
            }
        }

        public void OnViewDeactivated() {

        }

        private void RomManagerOnRomCompleted(object sender, Events.RomEventArgs e) {
            Log($"{CurrentRom} {Resources.Localization.Resources.Finished}");
            CurrentRom = string.Empty;
        }

        private void RomManagerOnProgress(object sender, Events.ProgressEventArgs e) {
            if (string.IsNullOrWhiteSpace(CurrentRom)) {
                CurrentRom = e.CurrentRom;
                Log($"{CurrentRom} {Resources.Localization.Resources.Started}");
            }
            RomProgress = e.RomProgress;
            TotalProgress = e.TotalProgress;
        }

        private RomRootViewModel[] GetRoms(string path, RomGroup group) {
            var root = new RomRootViewModel();
            var roms = _romManager.GetRoms(path);
            var collections = _romManager.AssignRomsToCollection(roms, group);
            if(collections == null) {
                for (int i = 0; i < roms.Length; i++) {
                    var vm = new RomViewModel(roms[i]);
                    root.AllRoms.Add(vm);
                    root.Entrys.Add(vm);
                }
                return new[] { root };
            }

            var colVms = new RomCollectionViewModel[collections.Length];
            for (int i = 0; i < collections.Length; i++) {
                colVms[i] = new RomCollectionViewModel(collections[i]);
                root.Entrys.Add(colVms[i]);
                
            }
            var allRoms = new List<RomViewModel>();
            foreach (var colVm in colVms) {
                AddAllRoms(colVm, ref allRoms);
            }
            root.AllRoms = allRoms;
            return new[] { root };
        }

        private void AddAllRoms(RomCollectionViewModel collection, ref List<RomViewModel> allRoms) {

            foreach(var entry in collection.Entrys) {
                var rom = entry as RomViewModel;
                if(rom != null) {
                    allRoms.Add(rom);
                    continue;
                }
                AddAllRoms(entry as RomCollectionViewModel, ref allRoms);
            }
        }

        private void CloseCommandExecuted() {
            Application.Current.Shutdown();
        }

        private void SettingsCommandExecuted() {
            var dialog = new OptionsWindow();
            dialog.ShowDialog();
        }

        private void AboutCommandExecuted() {
            var dialog = new AboutWindow();
            dialog.ShowDialog();
        }

        private void TransferToDeviceCommandExecuted() {
            Task.Run(async () => {
                await TransferRoms(DatabaseRoms[0], DrivePath);
                DriveRoms = GetRoms(DrivePath, DriveRomGroup);
            });
        }

        private void TransferToDatabaseCommandExecuted() {
            Task.Run(async () => {
                await TransferRoms(DriveRoms[0], DatabasePath);
                DatabaseRoms = GetRoms(DatabasePath, DatabaseRomGroup);
            });
        }

        private async Task TransferRoms(RomRootViewModel romRoot, string targetPath) {
            if(!Directory.Exists(targetPath)) {
                MessageBox.Show(Resources.Localization.Resources.ErrorPath, Resources.Localization.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var romList = new List<IRom>();
            foreach (var rom in romRoot.AllRoms) {
                if (!rom.Selected) continue;
                romList.Add(rom.Rom);
            }

            if (romList.Count == 0) {
                MessageBox.Show(Resources.Localization.Resources.ErrorNoRoms, Resources.Localization.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Log(Resources.Localization.Resources.InitializeCopy.Replace("#PATH#", targetPath));
            await _romManager.TransferRoms(romList.ToArray(), targetPath);
            Log(Resources.Localization.Resources.AllRomsCopied);
            
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

        private void SelectAllRomsCommandExecuted(bool isDrive) {
            var vms = isDrive ? DriveRoms : DatabaseRoms;
            if (vms == null) return;
            SetSelect(vms[0], true);
        }

        private void DeselectAllRomsCommandExecuted(bool isDrive) {
            var vms = isDrive ? DriveRoms : DatabaseRoms;
            if (vms == null) return;
            SetSelect(vms[0], false);
        }

        private void SetSelect(RomRootViewModel root, bool selected) {
            foreach (var rom in root.AllRoms) {
                rom.Selected = selected;
                continue;                
            }
        }

        private void SelectDeltaRomsCommandExecuted(bool isDrive) {
            var sourceRoot = isDrive ? DriveRoms[0] : DatabaseRoms[0];
            var compareRoot = isDrive ? DatabaseRoms[0] : DriveRoms[0];
            if (sourceRoot == null || compareRoot == null) return;
            foreach (var rom in sourceRoot.AllRoms) {
                rom.Selected = compareRoot.AllRoms.FirstOrDefault(x => x.Name.Equals(rom.Name)) == null;
            }
        }

        private void CheckAndFixDirectoryCommandExecuted(bool isDrive) {
            var path = isDrive ? DrivePath : DatabasePath;
            if (string.IsNullOrWhiteSpace(path)) return;
            _romManager.CheckAndFixDirectory(path);
        }

        private void RearrangeRomsCommandExecuted() {
            var result = MessageBox.Show(Resources.Localization.Resources.RearrangeSD, Resources.Localization.Resources.RearrangeSDTitel, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return;
            var root = _scope.Resolve<IRomCollection>();
            root.Name = string.Empty;
            var collections = new List<IRomCollection>();
            foreach(var vm in DriveRoms[0].Entrys) {
                var col = vm as RomCollectionViewModel;
                if (col != null) {
                    collections.Add(col.RomCollection);
                    continue;
                }
                var rom = vm as RomViewModel;
                if (rom == null) continue;
                root.Roms.Add(rom.Rom);
            }
            if (root.Roms.Count > 0) collections.Add(root);
            _romManager.RearrangeDrive(DrivePath, collections.ToArray());
        }

        private void Log(string message) {
            var newLine = _logText.Equals(string.Empty) ? string.Empty : "\r\n";
            _logText = $"{LogText}{newLine}{DateTime.Now:hh:mm:ss} {message}";
            RaisePropertyChanged(nameof(LogText));
        }
    }
}
 