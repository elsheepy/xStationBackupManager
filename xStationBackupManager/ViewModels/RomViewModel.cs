using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class RomViewModel : ViewModelBase {
        private IRom _rom;
        private bool _selected;

        public IRom Rom => _rom;

        public bool Selected {
            get => _selected;
            set {
                _selected = value;
                RaisePropertyChanged();
            }
        }

        public string Name { get => _rom.Name; set => _rom.Name = value; }
        public string Path { get => _rom.Path; set => _rom.Path = value; }

        public RomViewModel(IRom rom) {
            _rom = rom;
        }
    }
}
