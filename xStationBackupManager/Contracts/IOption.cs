using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xStationBackupManager.Enums;

namespace xStationBackupManager.Contracts {
    public interface IOption {
        public event EventHandler Changed;

        public Options Option { get; set; }

        public string GetValue();

        public void SetValue(string value);
    }
}
