using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NativeDb_Explorer
{
    /// <summary>
    /// Lógica de interacción para AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : CustomDialog
    {
        private MetroWindow Window;

        public AboutDialog(MetroWindow window)
        {
            InitializeComponent();
            this.Window = window;
        }

        private void gta5ModsBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.gta5-mods.com/tools/gta-v-nativedb-explorer");
        }

        private void githubBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Rbn3D/GTAV_NativeDB_Explorer");
        }
    }
}
