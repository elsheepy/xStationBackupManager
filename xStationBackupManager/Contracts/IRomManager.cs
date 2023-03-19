using System.Threading.Tasks;
using xStationBackupManager.Events;

namespace xStationBackupManager.Contracts {
    public interface IRomManager {
        event ProgressEventHandler Progress;
        event RomEventHandler RomCompleted;

        IRom[] GetRoms(string path);

        Task<bool> TransferRoms(IRom[] roms, string target);
    }
}
