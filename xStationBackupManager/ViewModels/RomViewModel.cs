using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class RomViewModel : RomEntryViewModel {
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

        public override string Name { get => _rom.Name; }
        public string Path { get => _rom.Path; set => _rom.Path = value; }

        public RomViewModel(IRom rom) {
            _rom = rom;
        }
    }
}
