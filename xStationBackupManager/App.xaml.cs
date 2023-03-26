using Autofac;
using System.ComponentModel;
using System.Windows;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;
using xStationBackupManager.Utilities;
using xStationBackupManager.ViewModels;

namespace xStationBackupManager {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            Autofacbuilder.Initialize();
            var language = Autofacbuilder.Scope.Resolve<IOptionsManager>().GetOption(Options.Language).GetValue<Language>();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.GetAttributeOfType<DescriptionAttribute>().Description);
            
            Current.Resources[nameof(ViewModelLocator)] = Autofacbuilder.Scope.Resolve<IViewModelLocator>();
        }
    }
}
