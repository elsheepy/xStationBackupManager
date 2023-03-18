using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Input;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.ViewModels {
    internal class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
        private readonly IOptionsManager _options;

        public RelayCommand CloseCommand { get; }

        public RelayCommand SettingsCommand { get; }

        public string RomsPath {
            get;
            set;
        }

        public MainWindowViewModel(IOptionsManager options) {
            _options = options;
            RomsPath = _options.GetOption(Options.RomPath).GetValue();
            CloseCommand = new RelayCommand(CloseCommandExecuted);
            SettingsCommand = new RelayCommand(SettingsCommandExecuted);
        }

        private void CloseCommandExecuted() {
            Application.Current.Shutdown();
        }

        private void SettingsCommandExecuted() {

        }
    }
}
