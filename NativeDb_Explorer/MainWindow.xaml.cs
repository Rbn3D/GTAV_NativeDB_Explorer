using MahApps.Metro.Controls;
using NativeDb_Explorer.Commands;
using NativeDb_Explorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public List<GTAVNative> Natives { get; set; }

        public String currentTextFilter = "";
        public List<GTAVNative> FilteredNatives { get; set; }

        public Timer TimerTextFilter;

        public MainWindow(List<GTAVNative> natives)
        {
            InitializeComponent();

            TimerTextFilter = new Timer((c) => Application.Current.Dispatcher.Invoke(new Action(() => { UpdateTextFilter(); })), null, Timeout.Infinite, Timeout.Infinite);

            this.Natives = natives;
            this.FilteredNatives = natives;

            LoadGrid();
        }

        private void LoadGrid()
        {
            GridNatives.ItemsSource = FilteredNatives;
            UpdateFilteringStats();
        }

        private void UpdateFilteringStats()
        {
            if(TxtFilter.Text.Trim() == String.Empty)
                CountFiltered.Content = $"List: Showing {FilteredNatives.Count()} Natives";
            else
                CountFiltered.Content = $"List: Showing {FilteredNatives.Count()} of {Natives.Count()} Natives";
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TimerTextFilter.Change(150, Timeout.Infinite);
        }

        private void UpdateTextFilter()
        {
            var text = TxtFilter.Text.ToLower().Trim();

            if (text == String.Empty)
                FilteredNatives = Natives;
            else
                FilteredNatives = Natives.Where(n => n.FunctionName.ToLower().Contains(text) || n.Namespace.ToLower().Contains(text) || n.MemoryAddress.ToLower().Contains(text)).ToList();

            GridNatives.ItemsSource = FilteredNatives;
            UpdateFilteringStats();
        }

        private ICommand copyNativeFull;

        public ICommand CopyNativeFull
        {
            get
            {
                return this.copyNativeFull ?? (this.copyNativeFull = new SimpleCommand
                {
                    ExecuteDelegate = async x =>
                    {
                        GTAVNative nat = (GTAVNative)GridNatives.SelectedValue;

                        if (nat != null)
                            Clipboard.SetText($"{nat.Namespace}::{nat.FunctionName}{nat.ParametersSignature};");
                    }
                });
            }
        }

        private ICommand copyNativeNoParams;

        public ICommand CopyNativeNoParams
        {
            get
            {
                return this.copyNativeNoParams ?? (this.copyNativeNoParams = new SimpleCommand
                {
                    ExecuteDelegate = async x =>
                    {
                        GTAVNative nat = (GTAVNative)GridNatives.SelectedValue;

                        if (nat != null)
                            Clipboard.SetText($"{nat.Namespace}::{nat.FunctionName}");
                    }
                });
            }
        }

        private ICommand copyNativeSimple;

        public ICommand CopyNativeSimple
        {
            get
            {
                return this.copyNativeSimple ?? (this.copyNativeSimple = new SimpleCommand
                {
                    ExecuteDelegate = async x =>
                    {
                        GTAVNative nat = (GTAVNative)GridNatives.SelectedValue;

                        if (nat != null)
                            Clipboard.SetText($"{nat.FunctionName}");
                    }
                });
            }
        }

        private ICommand copyMemAddress;

        public ICommand CopyMemAddress
        {
            get
            {
                return this.copyMemAddress ?? (this.copyMemAddress = new SimpleCommand
                {
                    ExecuteDelegate = async x =>
                    {
                        GTAVNative nat = (GTAVNative)GridNatives.SelectedValue;

                        if (nat != null)
                            Clipboard.SetText($"0x{getMemoryAddressFromCommentary(nat.Commentary)}");
                    }
                });
            }
        }

        private ICommand openInNativeDB;

        public ICommand OpenInNativeDB
        {
            get
            {
                return this.openInNativeDB ?? (this.openInNativeDB = new SimpleCommand
                {
                    ExecuteDelegate = async x =>
                    {
                        GTAVNative nat = (GTAVNative)GridNatives.SelectedValue;

                        if (nat != null)
                        {
                            var fnId = getMemoryAddressFromCommentary(nat.Commentary).ToLower();

                            OpenUrlInDefaultBrowser($"http://www.dev-c.com/nativedb/func/info/{ fnId }#fntrg-{ fnId }");
                        }
                            
                    }
                });
            }
        }

        private string getMemoryAddressFromCommentary(string commentary)
        {
            return commentary.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        public void OpenUrlInDefaultBrowser(string url)
        {
            Uri uriResult;

            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if(result)
                System.Diagnostics.Process.Start(url);
        }
    }
}
