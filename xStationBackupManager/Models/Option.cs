using xStationBackupManager.Contracts;

namespace xStationBackupManager.Models {
    public class Option : IOption {
        private string _value;

        public string GetValue() {
            return _value;
        }

        public void SetValue(string value) {
            _value = value;
        }
    }
}
