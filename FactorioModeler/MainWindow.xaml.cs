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
using FactorioModeler.FACTORIOOUTPUT;

namespace FactorioModeler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataCollector data = new DataCollector();
        public string GetStatistics;
        public MainWindow()
        {
            InitializeComponent();

            GetStatistics = data.StartDataColletctor();
            Console.WriteLine(GetStatistics);

            this.DataContext = this;
        }
    }
}
