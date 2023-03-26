using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xStationBackupManager.Enums;

namespace xStationBackupManager.Contracts {
    public interface IOption {
        event EventHandler Changed;

        Options Option { get; set; }

        string GetValue();

        T GetValue<T>() where T : struct, IConvertible;

        void SetValue(string value);

        void SetValue(object value);
    }
}
