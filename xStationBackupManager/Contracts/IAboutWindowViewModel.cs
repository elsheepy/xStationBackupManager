using System;

namespace xStationBackupManager.Contracts {
    public interface IAboutWindowViewModel {
        Action CloseCallback { get; set; }
    }
}
