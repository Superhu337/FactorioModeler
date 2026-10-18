using FactorioModeler.Models;
using FactorioModeler.Scripts;
using FactorioModeler.Views;
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
/*
 *     <!-- #2f2f2f color under 
    #606060 colo up 
    #ffafaf input 
    #95f6a8 output 
    #a0a0a0 line 
    Background="#2f2f2f" Foreground="#f8f8f8" butt on -->
 */

namespace FactorioModeler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SearchPopUp activeSearchWindow = null;
        DataCollector data = new DataCollector();
        public string GetStatistics { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            //GetStatistics = data.StartDataColletctor();
            //Console.WriteLine(GetStatistics);


        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonMenuPopup.IsOpen = !ButtonMenuPopup.IsOpen;
        }

        // Обработчик нажатия ПКМ в любом месте экрана
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Получаем текущие координаты мыши относительно главного окна
            Point mousePosition = e.GetPosition(this);

            // Переводим их в абсолютные экранные координаты (учитывая, что окно развернуто)
            Point screenPosition = this.PointToScreen(mousePosition);

            // 2. ПРОВЕРКА: Если окно еще не создано ИЛИ оно было закрыто пользователем
            if (activeSearchWindow == null || !activeSearchWindow.IsLoaded)
            {
                activeSearchWindow = new SearchPopUp();
                activeSearchWindow.Owner = this;
            }

            // 3. Позиционируем окно точно по курсору мыши
            activeSearchWindow.Left = screenPosition.X;
            activeSearchWindow.Top = screenPosition.Y;

            // 4. Показываем окно и выводим на передний план (активируем)
            activeSearchWindow.Show();
            activeSearchWindow.Activate();

            e.Handled = true;
        }
    }
}
