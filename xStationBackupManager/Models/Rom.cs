using xStationBackupManager.Contracts;

namespace xStationBackupManager.Models {
    public class Rom : IRom {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
