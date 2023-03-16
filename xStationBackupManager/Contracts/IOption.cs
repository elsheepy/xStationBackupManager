using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xStationBackupManager.Contracts {
    internal interface IOption {
        public string GetValue();

        public void SetValue(string value);
    }
}
