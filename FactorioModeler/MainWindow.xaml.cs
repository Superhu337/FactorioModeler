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
    #606060 color up 
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
        RecipeCard Card = new RecipeCard();
        public string GetStatistics { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            data.StartDataColletctor();
            //Console.WriteLine(GetStatistics);

            Card.SpawnCard(1, 1);
            Card.Show();

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
                activeSearchWindow = new SearchPopUp(data);
                activeSearchWindow.Owner = this;
            }

            // --- ЛОГИКА УДЕРЖАНИЯ В ГРАНИЦАХ ЭКРАНА ---
            // Получаем рабочую область экрана (без панели задач)
            Rect workArea = SystemParameters.WorkArea;

            double x = screenPosition.X;
            double y = screenPosition.Y;

            // Если окно выходит за правую границу
            if (x + activeSearchWindow.Width > workArea.Right)
                x = workArea.Right - activeSearchWindow.Width;

            // Если окно выходит за нижнюю границу
            if (y + activeSearchWindow.Height > workArea.Bottom)
                y = workArea.Bottom - activeSearchWindow.Height;

            // Если окно выходит за левую или верхнюю границу
            if (x < workArea.Left) x = workArea.Left;
            if (y < workArea.Top) y = workArea.Top;

            // Устанавливаем безопасные координаты
            activeSearchWindow.Left = x;
            activeSearchWindow.Top = y;

            // Показываем окно и выводим на передний план
            activeSearchWindow.Show();
            activeSearchWindow.Activate();
            activeSearchWindow.FocusSearchBox();

            e.Handled = true;
        }
    }
}
