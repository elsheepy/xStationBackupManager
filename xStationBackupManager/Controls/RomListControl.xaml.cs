using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using xStationBackupManager.ViewModels;

namespace xStationBackupManager.Controls {
    /// <summary>
    /// Interaktionslogik für RomListControl.xaml
    /// </summary>
    public partial class RomListControl : UserControl {
        public static DependencyProperty RomsProperty = DependencyProperty.Register(nameof(Roms), typeof(RomViewModel[]), typeof(RomListControl), new PropertyMetadata(default(RomViewModel[]), changed));

        public static void changed(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var bla = true;
        }

        public RomViewModel[] Roms {
            get => (RomViewModel[])GetValue(RomsProperty);
            set => SetValue(RomsProperty, value);
        }

        public RomListControl() {
            InitializeComponent();
        }
    }
}
