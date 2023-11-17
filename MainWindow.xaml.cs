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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        object rootElmnt;
        private void LoadXaml_Click(object sender, RoutedEventArgs e)
        {
            string xamlFile = @"Descripts/ruXaml.xaml";//c:\tmp\
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
            parseProc proc = new parseProc(this);
            proc.Init(rootElmnt);
            
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
}

}
