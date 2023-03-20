using System;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.Models {
    public class OptionImpl : IOption {
        private string _value;

        public event EventHandler Changed;

        public Options Option { get; set; }

        public string GetValue() {
            return _value;
        }

        public void SetValue(string value) {
            if (_value != null && _value.Equals(value)) return;
            _value = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
