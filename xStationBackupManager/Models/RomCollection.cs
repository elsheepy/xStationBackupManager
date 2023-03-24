using System.Collections.Generic;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.Models {
    internal class RomCollection : IRomCollection {
        public bool IsRoot { get; set; }
        public string Name { get; set; }
        public List<IRom> Roms { get; set; } = new List<IRom>();
        public List<IRomCollection> Collections { get; set; } = new List<IRomCollection>();
    }
}
