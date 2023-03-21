using Autofac;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.Views {
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window {
        public AboutWindow() {
            InitializeComponent();
            Autofacbuilder.Scope.Resolve<IAboutWindowViewModel>().CloseCallback = () => { this.Close(); };
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            // for .NET Core you need to add UseShellExecute = true
            // see https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            var proc = new Process();
            proc.StartInfo.FileName = e.Uri.AbsoluteUri;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            e.Handled = true;
        }
    }
}
