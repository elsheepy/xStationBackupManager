using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xStationBackupManager.Contracts {
    public interface ISelectRomsCommandProvider {
        public RelayCommand<bool> SelectAllRomsCommand { get; }
        public RelayCommand<bool> DeselectAllRomsCommand { get; }
        public RelayCommand<bool> SelectDeltaRomsCommand { get; }
        public RelayCommand<bool> CheckAndFixDirectoryCommand { get; }
    }
}
