using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xStationBackupManager.Enums;

namespace xStationBackupManager.Contracts {
    internal interface IOptionsManager {
        IOption GetOption(Options option);
    }
}
