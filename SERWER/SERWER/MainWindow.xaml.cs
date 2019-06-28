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

namespace SERWER
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;
        
        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
         
        }
        
        private void Stop_Serwer_Click(object sender, RoutedEventArgs e)
        {
            Serwer.Zatrzymaj_serwer();

            this.Close();
        }

        private void Start_Serwer_Click(object sender, RoutedEventArgs e)
        {
            Serwer.Uruchom_serwer();
        }
        
        
    }
}
