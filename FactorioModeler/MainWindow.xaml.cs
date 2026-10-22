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
        private RecipeCard Card = new RecipeCard();
        public string GetStatistics { get; set; }
        private CurveBuilder _curveBuilder;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            data.StartDataColletctor();
            //Console.WriteLine(GetStatistics);
            _curveBuilder = new CurveBuilder(WorkspaceCanvas);
        }



        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonMenuPopup.IsOpen = !ButtonMenuPopup.IsOpen;
        }

        public void CreateRecipeCard(FactorioRecipe recipe)
        {
            if (recipe == null) return;

            var card = new RecipeCard();
            //card.Owner = this;              // MainWindow уже показан — ок
            card.SpawnCard(recipe, data);   // data — то же поле, что уже есть
            card.IngredientClicked += WorkspaceCanvas_IngredientClicked;

            // Получаем позицию клика мыши относительно ХОЛСТА (а не экрана)
            Point mousePos = Mouse.GetPosition(WorkspaceCanvas);

            // Устанавливаем положение карточки на холсте
            Canvas.SetLeft(card, mousePos.X + 10);
            Canvas.SetTop(card, mousePos.Y + 10);

            // Добавляем карточку физически ВНУТРЬ холста главного окна
            WorkspaceCanvas.Children.Add(card);
        }

        public void OpenSearchPopup(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            Point screenPosition = this.PointToScreen(mousePosition);

            // 1. Создаем окно только ОДИН РАЗ при самом первом клике в сессии
            if (activeSearchWindow == null || !activeSearchWindow.IsLoaded)
            {
                activeSearchWindow = new SearchPopUp(data);
                activeSearchWindow.Owner = this;
                activeSearchWindow.Show(); // Открываем ОДИН раз на старте программы
            }

            // 2. Рассчитываем координаты удержания в границах экрана
            Rect workArea = SystemParameters.WorkArea;
            double x = screenPosition.X - 540 / 2;
            double y = screenPosition.Y - 440 / 2;

            if (x + activeSearchWindow.Width > workArea.Right) x = workArea.Right - activeSearchWindow.Width;
            if (y + activeSearchWindow.Height > workArea.Bottom) y = workArea.Bottom - activeSearchWindow.Height;
            if (x < workArea.Left) x = workArea.Left;
            if (y < workArea.Top) y = workArea.Top;

            activeSearchWindow.Left = x;
            activeSearchWindow.Top = y;

            // 3. Вместо Show() плавно делаем его видимым и кликабельным
            activeSearchWindow.Opacity = 1.0;
            activeSearchWindow.IsHitTestVisible = true;

            activeSearchWindow.Activate();
            activeSearchWindow.FocusSearchBox();

            e.Handled = true;
        }


        /*private void PositionAndFocusPopup(Point screenPosition)
        {
            // --- ЛОГИКА УДЕРЖАНИЯ В ГРАНИЦАХ ЭКРАНА ---
            // Получаем рабочую область экрана (без панели задач)
            Rect workArea = SystemParameters.WorkArea;

            double x = screenPosition.X - 540 / 2;
            double y = screenPosition.Y - 440 / 2;

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
            activeSearchWindow.Opacity = 1.0;
            activeSearchWindow.IsHitTestVisible = true;

            activeSearchWindow.Activate();
            activeSearchWindow.FocusSearchBox();

        }*/
        // Обработчик нажатия ПКМ в любом месте экрана
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenSearchPopup(sender, e);
        }
        // 1. НАЧАЛО: Срабатывает, когда кликнули по Border внутри карточки
        private void WorkspaceCanvas_IngredientClicked(RecipeCard card, Point pointInCard, string panelName)
        {
            // Передаем карточку, точку клика и цвет

            _curveBuilder.StartDrawing(card, pointInCard, Color.FromRgb(0xa0, 0xa0, 0xa0), panelName); //a0a0a0
            
        }

        // 4. ДВИЖЕНИЕ: Просто передаем позицию мыши в отдельный файл
        private void WorkspaceCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentMousePoint = e.GetPosition(WorkspaceCanvas);
            _curveBuilder.UpdateDrawing(currentMousePoint);
        }

        // 5. ФИНАЛ: Сигнализируем о том, что кнопку отпустили
        private void WorkspaceCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool wasDrawing = _curveBuilder.StopDrawing();
            
            if (wasDrawing)
            {
                OpenSearchPopup(sender, e);
            }
        }

    }
}
