using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class OptionsWindowViewModel : ViewModelBase, IOptionsWindowViewModel {
        private readonly IOptionsManager _options;
        private IOption _romPathOption;
        private string _romsPath;

        public RelayCommand BrowseRomPathCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand SaveCommand { get; }

        public string RomsPath {
            get => _romsPath;
            set {
                _romsPath = value;
                RaisePropertyChanged();
            }
        }

        public Action CloseCallback { get; set; }


        public OptionsWindowViewModel(IOptionsManager options) {
            _options = options;
            _romPathOption = _options.GetOption(Enums.Options.RomPath);
            RomsPath = _romPathOption.GetValue();

            BrowseRomPathCommand = new RelayCommand(BrowseRomPathCommandExecuted);
            CloseCommand = new RelayCommand(CloseCommandExecuted);
            SaveCommand = new RelayCommand(SaveCommandExecuted);
        }
        private void BrowseRomPathCommandExecuted() {
            var dialog = new FolderBrowserDialog();
            if(!string.IsNullOrWhiteSpace(RomsPath)) dialog.SelectedPath = RomsPath;
            dialog.ShowDialog();
            RomsPath = dialog.SelectedPath;
        }
        private void CloseCommandExecuted() {
            CloseCallback?.Invoke();
        }
        private void SaveCommandExecuted() {
            _romPathOption.SetValue(RomsPath);
            _options.Save();
            CloseCommandExecuted();
        }
    }
}
