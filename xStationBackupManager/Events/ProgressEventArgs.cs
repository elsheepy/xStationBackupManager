
namespace xStationBackupManager.Events {
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

    public class ProgressEventArgs {
        public int RomProgress { get; }
        public int TotalProgress { get; }
        public string CurrentRom { get; }

        public ProgressEventArgs(int romProgress, int totalProgress, string currentRom) {
            RomProgress = romProgress;
            TotalProgress = totalProgress;
            CurrentRom = currentRom;
        }
    }
}
