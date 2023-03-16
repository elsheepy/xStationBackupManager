using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    internal class RomViewModel : ViewModelBase {
        private IRom _rom;

        public string Name { get => _rom.Name; set => _rom.Name = value; }
        public string Path { get => _rom.Path; set => _rom.Path = value; }

        public RomViewModel(IRom rom) {
            _rom = rom;
        }
    }
}
