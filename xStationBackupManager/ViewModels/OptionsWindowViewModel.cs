using CommunityToolkit.Mvvm.Input;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;
using xStationBackupManager.Utilities;

namespace xStationBackupManager.ViewModels {
    public class OptionsWindowViewModel : ViewModelBase, IOptionsWindowViewModel {
        private readonly IOptionsManager _options;
        private IOption _romPathOption;
        private IOption _languageOption;
        private string _romsPath;
        private Language _language;

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

        public string[] Languages { get; }

        public string LanguageString {
            get => Language.ToString();
            set {
                if(Enum.TryParse<Language>(value, out var tmp)) {
                    Language = tmp;
                }
            }
        }

        public Language Language {
            get => _language;
            set {
                _language = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LanguageString));
            }
        }

        public Action CloseCallback { get; set; }


        public OptionsWindowViewModel(IOptionsManager options) {
            _options = options;
            _romPathOption = _options.GetOption(Enums.Options.RomPath);
            _languageOption = _options.GetOption(Enums.Options.Language);
            RomsPath = _romPathOption.GetValue();
            Language = _languageOption.GetValue<Language>();
            Languages = Enum.GetNames<Language>();

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
            if (_languageOption.GetValue<Language>() != Language) {
                _languageOption.SetValue(Language);                
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Language.GetAttributeOfType<DescriptionAttribute>().Description);
                MessageBox.Show(Resources.Localization.Resources.PleaseRestart);
            }
            _options.Save();
            CloseCommandExecuted();
        }
    }
}
