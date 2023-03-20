using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using xStationBackupManager.Contracts;
using xStationBackupManager.ViewModels;

namespace xStationBackupManager.Controls {
    /// <summary>
    /// Interaktionslogik für RomListControl.xaml
    /// </summary>
    public partial class RomListControl : UserControl, INotifyPropertyChanged {
        public static DependencyProperty RomsProperty = DependencyProperty.Register(nameof(Roms), typeof(RomViewModel[]), typeof(RomListControl));
        public static DependencyProperty CommandProviderProperty = DependencyProperty.Register(nameof(CommandProvider), typeof(ISelectRomsCommandProvider), typeof(RomListControl), new PropertyMetadata(CommandProviderChanged));
        public static DependencyProperty IsDrivePropery = DependencyProperty.Register(nameof(IsDrive), typeof(bool), typeof(RomListControl));

        private static void CommandProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RomListControl ctrl = (RomListControl)d;
            ctrl.RaisePropertyChanged(nameof(SelectAllRomsCommand));
            ctrl.RaisePropertyChanged(nameof(DeselectAllRomsCommand));
            ctrl.RaisePropertyChanged(nameof(SelectDeltaRomsCommand));
            ctrl.RaisePropertyChanged(nameof(CheckAndFixDirectoryCommand));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RomViewModel[] Roms {
            get => (RomViewModel[])GetValue(RomsProperty);
            set => SetValue(RomsProperty, value);
        }

        public ISelectRomsCommandProvider CommandProvider { 
            get => (ISelectRomsCommandProvider)GetValue(CommandProviderProperty);
            set => SetValue(CommandProviderProperty, value);
        }

        public bool IsDrive {
            get => (bool)GetValue(IsDrivePropery);
            set => SetValue(IsDrivePropery, value);
        }

        public RelayCommand<bool> SelectAllRomsCommand => CommandProvider?.SelectAllRomsCommand;
        public RelayCommand<bool> DeselectAllRomsCommand => CommandProvider?.DeselectAllRomsCommand;
        public RelayCommand<bool> SelectDeltaRomsCommand => CommandProvider?.SelectDeltaRomsCommand;
        public RelayCommand<bool> CheckAndFixDirectoryCommand => CommandProvider?.CheckAndFixDirectoryCommand;

        public RomListControl() {
            InitializeComponent();
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            if (string.IsNullOrWhiteSpace(propertyName)) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
