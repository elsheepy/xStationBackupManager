using Autofac;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.Manager {
    public class OptionsManager : IOptionsManager {
        private ILifetimeScope _scope;

        public OptionsManager(ILifetimeScope scope) {
            _scope = scope;
        }

        IOption IOptionsManager.GetOption(Options option) {
            var result = _scope.Resolve<IOption>();
            result.SetValue(@"D:\Emulatoren\PlayStation\Roms");
            return result;
        }
    }
}
