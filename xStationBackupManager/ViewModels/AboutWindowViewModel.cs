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

        public AboutWindowViewModel() {
            CloseCommand = new RelayCommand(CloseCommandExecuted);
        }

        private void CloseCommandExecuted() {
            CloseCallback?.Invoke();
        }
    }
}
