using System.Collections.Generic;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class RomRootViewModel : ViewModelBase {
        public List<RomViewModel> AllRoms { get; set; } = new List<RomViewModel>();
        public List<RomEntryViewModel> Entrys { get; set; } = new List<RomEntryViewModel>();
    }
}
