using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Input;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.ViewModels {
    internal class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
        private readonly IOptionsManager _options;
        private readonly IRomManager _romManager;

        public RelayCommand CloseCommand { get; }

        public RelayCommand SettingsCommand { get; }

        public string RomsPath { get; set; }

        public RomViewModel[] Roms { get; set; }

        public MainWindowViewModel(IOptionsManager options, IRomManager romManager) {
            _options = options;
            RomsPath = _options.GetOption(Options.RomPath).GetValue();
            CloseCommand = new RelayCommand(CloseCommandExecuted);
            SettingsCommand = new RelayCommand(SettingsCommandExecuted);
            _romManager = romManager;

            var roms = _romManager.GetRoms(RomsPath);
            var romVms = new RomViewModel[roms.Length];
            for(int i = 0; i < roms.Length; i++) {
                romVms[i] = new RomViewModel(roms[i]);
            }
            Roms = romVms;
        }

        private void CloseCommandExecuted() {
            Application.Current.Shutdown();
        }

        private void SettingsCommandExecuted() {

        }
    }
}
