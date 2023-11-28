using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace nvmParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            initHexEdit();
            string flNm = @"Descripts/ruXaml.xaml";//c:\tmp\
            FileName.Text = Path.GetFullPath(flNm);

            flNm = @"binStreams\initialArr.BIN";
            BinFileName.Text = Path.GetFullPath(flNm);

            CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(
                            Command.OpenDescrFile,
                            (obj, e) =>
                            {
                                e.Handled = true;
                                ((MainWindow)obj).ShowFileOpenDialog((TextBox)e.Parameter);
                            },
                            (obj, e) => { e.CanExecute = true; }));
        }

        internal void initHexEdit(byte[] arrp = null) 
        {
            byte[] arr = new byte[] { 1,2,3,4,5,6,7,8,9,10,
                        1,2,3,4,5,6,7,8,9,10,
                        1,2,3,4,5,6,7,8,9,10};
            if (arrp != null) arr = arrp;
            HexEdit.Stream = new MemoryStream(arr);
        }
        void ShowFileOpenDialog(TextBox textBox)
        {
            string fname = textBox.Text;
            string path;
            if (string.IsNullOrEmpty(fname)) 
            {
                path = Environment.CurrentDirectory;
            }
            else
            {
                path = System.IO.Path.GetFullPath(fname);
            }
            
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                InitialDirectory = path,
            };
            if (dialog.ShowDialog() == true)
                textBox.Text = dialog.FileName;
        }

        object rootElmnt;
        private void LoadXaml_Click(object sender, RoutedEventArgs e)
        {
            //            string xamlFile = @"Descripts/ruXaml.xaml";//c:\tmp\
            string xamlFile = FileName.Text;
            using (FileStream fs = new FileStream(xamlFile, FileMode.Open))
            {
                rootElmnt = (object)XamlReader.Load(fs);
                xamlFile  = fs.Name;
            }
            if (rootElmnt is DescriptionRoot root)
            {
                DeclarationProc proc = new DeclarationProc(this);
                root.LoadedFile = xamlFile;
                proc.Init(root);
            }
            else 
            {
                LogFunc($"Root object error in XML:{xamlFile}");
            }
        }

        private void ParseArr_Click(object sender, RoutedEventArgs e)
        {
            textBox.Items.Clear();
            string binFileName = BinFileName.Text;
            parseProc proc = new parseProc(this);
            proc.Init(rootElmnt, binFileName);
        }
        internal void scrollToButtom()
        {
            Border border = (Border)VisualTreeHelper.GetChild(textBox, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            scrollViewer.ScrollToBottom();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            LoadXaml_Click(null, null);
        }

        void LogFunc(string msg)
        {
                    textBox.Items.Add(msg); 
                    scrollToButtom(); 
        }

        private void fieldsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //            MessageBox.Show(((TreeViewItem)e.NewValue).Header.ToString());
            BaseBinValue itm = e.NewValue as BaseBinValue;
            if (itm != null) 
            {
                int pos = itm.bitPos / 8;
                HexEdit.SelectionStart = pos;
                HexEdit.SelectionStop = itm.getEndPos();
            }
        }
    }
    public static class Command
    {

        public static readonly RoutedUICommand OpenDescrFile = new RoutedUICommand("Open Descr File", "OpenDescrFile", typeof(MainWindow));
//        public static readonly RoutedUICommand OpenBinFile = new RoutedUICommand("Open Bin File", "OpenBinFile", typeof(MainWindow));
    }
}
