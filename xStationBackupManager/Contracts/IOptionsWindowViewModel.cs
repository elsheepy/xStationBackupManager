using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xStationBackupManager.Contracts {
    public interface IOptionsWindowViewModel {
        Action CloseCallback { get; set; }
    }
}
