using System.Collections.Generic;

namespace xStationBackupManager.Contracts {
    public interface IRomCollection {
        bool IsRoot { get; set; }

        string Name { get; set; }

        List<IRom> Roms { get; set; }

        List<IRomCollection> Collections { get; set; }
    }
}
