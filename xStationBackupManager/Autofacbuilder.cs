using Autofac;
using xStationBackupManager.Assigner;
using xStationBackupManager.Contracts;
using xStationBackupManager.Manager;
using xStationBackupManager.Models;
using xStationBackupManager.ViewModels;

namespace xStationBackupManager {
    public static class Autofacbuilder {
        public static ILifetimeScope Scope { get; private set; }
        public static void Initialize() {
            var builder = new ContainerBuilder();

            builder.RegisterType<OptionImpl>().As<IOption>();
            builder.RegisterType<OptionsManager>().As<IOptionsManager>().SingleInstance();

            builder.RegisterType<Rom>().As<IRom>();
            builder.RegisterType<RomCollection>().As<IRomCollection>();
            builder.RegisterType<RomManager>().As<IRomManager>().SingleInstance();

            builder.RegisterType<RomCollectionAlphabetAssigner>().As<IRomCollectionAssigner>().SingleInstance();

            builder.RegisterType<ViewModelLocator>().As<IViewModelLocator>().SingleInstance();
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().SingleInstance();
            builder.RegisterType<OptionsWindowViewModel>().As<IOptionsWindowViewModel>().SingleInstance();
            builder.RegisterType<AboutWindowViewModel>().As<IAboutWindowViewModel>().SingleInstance();

            var container = builder.Build();
            Scope = container.BeginLifetimeScope();
        }
    }
}
