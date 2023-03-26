using xStationBackupManager.Enums;

namespace xStationBackupManager.Contracts {
    internal interface IRomCollectionAssigner {
        RomGroup RomGroup { get; }

        IRomCollection[] AssignRomsToCollection(IRom[] roms);
    }
}
