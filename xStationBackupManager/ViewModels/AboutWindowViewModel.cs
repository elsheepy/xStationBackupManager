using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    internal class AboutWindowViewModel : ViewModelBase, IAboutWindowViewModel {
        public RelayCommand CloseCommand { get; set; }

        public Action CloseCallback { get; set; }

        public string Version { get; set; }

        public AboutWindowViewModel() {
            CloseCommand = new RelayCommand(CloseCommandExecuted);
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version = $"v{Version.Substring(0, Version.Length - 2)}";
        }

        private void CloseCommandExecuted() {
            CloseCallback?.Invoke();
        }
    }
}
