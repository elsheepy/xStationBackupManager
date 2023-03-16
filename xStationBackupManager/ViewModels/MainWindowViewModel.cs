using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.ViewModels {
    internal class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
        private readonly IOptionsManager _options;

        public string RomsPath {
            get;
            set;
        }

        public MainWindowViewModel(IOptionsManager options) {
            _options = options;
            RomsPath = _options.GetOption(Options.RomPath).GetValue();
        }
    }
}
