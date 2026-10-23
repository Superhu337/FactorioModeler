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



namespace FactorioModeler.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsMenu.xaml
    /// </summary>
    public partial class SettingsMenu : UserControl
    {

        public SettingsMenu()
        {
            InitializeComponent();
        }
        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Import_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Import_Click");
        }

        public void Export_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Export_Click");
        }

        public void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Refresh_Click");
        }

        public void Redo_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Redo_Click");
        }
        public void Undo_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Undo_Click");
        }
        public void Settings_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Settings_Click");
        }
    }
}
