﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;
using xStationBackupManager.ViewModels;

namespace xStationBackupManager.Controls {
    /// <summary>
    /// Interaktionslogik für RomListControl.xaml
    /// </summary>
    public partial class RomListControl : UserControl, INotifyPropertyChanged {
        public static DependencyProperty RomsProperty = DependencyProperty.Register(nameof(Roms), typeof(RomRootViewModel[]), typeof(RomListControl));
        public static DependencyProperty CommandProviderProperty = DependencyProperty.Register(nameof(CommandProvider), typeof(ISelectRomsCommandProvider), typeof(RomListControl), new PropertyMetadata(CommandProviderChanged));
        public static DependencyProperty IsDrivePropery = DependencyProperty.Register(nameof(IsDrive), typeof(bool), typeof(RomListControl));
        public static DependencyProperty RomGroupProperty = DependencyProperty.Register(nameof(RomGroup), typeof(RomGroup), typeof(RomListControl));

        private static void CommandProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RomListControl ctrl = (RomListControl)d;
            ctrl.RaisePropertyChanged(nameof(SelectAllRomsCommand));
            ctrl.RaisePropertyChanged(nameof(DeselectAllRomsCommand));
            ctrl.RaisePropertyChanged(nameof(SelectDeltaRomsCommand));
            ctrl.RaisePropertyChanged(nameof(CheckAndFixDirectoryCommand));
            ctrl.RaisePropertyChanged(nameof(RearrangeRomsCommand));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RomRootViewModel[] Roms {
            get => (RomRootViewModel[])GetValue(RomsProperty);
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

        public string[] RomGroupOptions { get; set; }

        public string RomGroupString {
            get => RomGroup.ToString();
            set {
                if(Enum.TryParse<RomGroup>(value, out var group)) {
                    RomGroup = group;
                }
            }
        }

        public RomGroup RomGroup {
            get => (RomGroup)GetValue(RomGroupProperty);
            set => SetValue(RomGroupProperty, value);
        }

        public RelayCommand<bool> SelectAllRomsCommand => CommandProvider?.SelectAllRomsCommand;
        public RelayCommand<bool> DeselectAllRomsCommand => CommandProvider?.DeselectAllRomsCommand;
        public RelayCommand<bool> SelectDeltaRomsCommand => CommandProvider?.SelectDeltaRomsCommand;
        public RelayCommand<bool> CheckAndFixDirectoryCommand => CommandProvider?.CheckAndFixDirectoryCommand;

        public RelayCommand RearrangeRomsCommand => CommandProvider?.RearrangeRomsCommand;

        public RomListControl() {
            RomGroupOptions = Enum.GetNames(typeof(RomGroup));
            InitializeComponent();
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            if (string.IsNullOrWhiteSpace(propertyName)) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
