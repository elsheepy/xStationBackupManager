using System.Threading.Tasks;
using xStationBackupManager.Enums;
using xStationBackupManager.Events;

namespace xStationBackupManager.Contracts {
    public interface IRomManager {
        event ProgressEventHandler Progress;
        event RomEventHandler RomCompleted;

        IRom[] GetRoms(string path);

        IRomCollection[] AssignRomsToCollection(IRom[] roms, RomGroup groups);

        Task<bool> TransferRoms(IRom[] roms, string target);

        Task CheckAndFixDirectory(string directory);

        Task RearrangeDrive(string drivePath, IRomCollection[] collections);
    }
}
