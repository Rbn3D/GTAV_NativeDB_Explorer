using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using NativeDb_Explorer.Business;
using NativeDb_Explorer.Model;
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
using System.Windows.Shapes;

namespace NativeDb_Explorer
{
    /// <summary>
    /// Lógica de interacción para SourceSelectionWindow.xaml
    /// </summary>
    public partial class SourceSelectionWindow : MetroWindow
    {
        public SourceSelectionWindow()
        {
            InitializeComponent();
        }

        private void DownloadNativesBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String url = "http://www.dev-c.com/nativedb/reference.zip";

                var natives = HTMLNativeParser.ParseZippedHtmlUrl(url); // TODO load this assync (proggress bar etc)

                var window = new MainWindow(natives);
                window.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Download/Parsing error", $"Reason: {ex.Message}");
            }
        }

        private void LoadNativesFromFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "NativeDB reference (reference.html)|reference.html|NativeDB zipped reference (reference.zip)|reference.zip|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var ext = System.IO.Path.GetExtension(openFileDialog.FileName);

                    List<GTAVNative> natives = null;

                    // TODO load this assync (proggress bar etc)
                    if (ext == ".html" || ext == ".htm")
                        natives = HTMLNativeParser.ParseLocalHtmlFile(openFileDialog.FileName);
                    else if (ext == ".zip")
                        natives = HTMLNativeParser.ParseZippedLocalHtmlFile(openFileDialog.FileName);
                    else
                        throw new ArgumentException($"Unknown file extension {ext}");


                    var window = new MainWindow(natives);
                    window.Show();

                    this.Close();
                }
                catch (Exception ex)
                {
                    this.ShowMessageAsync("Parsing error", $"Reason: {ex.Message}");
                }
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            //Add link to 5-Mods page instead of Github since I assume most people asking for help don't use/know what Github is
            System.Diagnostics.Process.Start("https://www.gta5-mods.com/tools/gta-v-nativedb-explorer");
        }
    }
}
