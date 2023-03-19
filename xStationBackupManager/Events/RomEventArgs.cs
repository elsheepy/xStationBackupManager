using System;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.Events {
    public delegate void RomEventHandler(object sender, RomEventArgs e);

    public class RomEventArgs : EventArgs {
        public IRom Rom { get; }

        public RomEventArgs(IRom rom) {
            Rom = rom;
        }
    }
}
