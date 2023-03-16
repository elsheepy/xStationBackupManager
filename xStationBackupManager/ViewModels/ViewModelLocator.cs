using Autofac;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class ViewModelLocator : IViewModelLocator {
        public IMainWindowViewModel MainWindowViewModel { get; }

        public ViewModelLocator(ILifetimeScope scope) {
            MainWindowViewModel = scope.Resolve<IMainWindowViewModel>();
        }       
    }
}
