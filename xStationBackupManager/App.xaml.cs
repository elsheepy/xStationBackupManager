using Autofac;
using System.Windows;
using xStationBackupManager.Contracts;
using xStationBackupManager.ViewModels;

namespace xStationBackupManager {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            Autofacbuilder.Initialize();
            Current.Resources[nameof(ViewModelLocator)] = Autofacbuilder.Scope.Resolve<IViewModelLocator>();
        }
    }
}
