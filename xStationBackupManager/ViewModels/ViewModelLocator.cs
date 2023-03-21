using Autofac;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class ViewModelLocator : IViewModelLocator {
        public IMainWindowViewModel MainWindowViewModel { get; }

        public IOptionsWindowViewModel OptionsWindowViewModel { get; }

        public IAboutWindowViewModel AboutWindowViewModel { get; }

        public ViewModelLocator(ILifetimeScope scope) {
            MainWindowViewModel = scope.Resolve<IMainWindowViewModel>();
            OptionsWindowViewModel = scope.Resolve<IOptionsWindowViewModel>();
            AboutWindowViewModel = scope.Resolve<IAboutWindowViewModel>();
        }       
    }
}
