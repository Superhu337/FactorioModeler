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

using FactorioModeler.Models;
using FactorioModeler.Scripts;
using FactorioModeler.Services;
using FactorioModeler.ViewModels;
using FactorioModeler.Views;

/*
 *     <!-- #2f2f2f color under 
    #606060 color up 
    #ffafaf input 
    #95f6a8 output 
    #a0a0a0 line 
    Background="#2f2f2f" Foreground="#f8f8f8" butt on -->
 */

namespace FactorioModeler.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IDataService _dataService;
        private readonly IWindowManager _windowManager;
        private readonly IEventBus _eventBus;
        private readonly ICardDataExtractor _cardDataExtractor;

        public MainWindow(
            IDataService dataService,
            IWindowManager windowManager,
            IEventBus eventBus,
            ICardDataExtractor cardDataExtractor)
        {
            InitializeComponent();

            _dataService = dataService;
            _windowManager = windowManager;
            _eventBus = eventBus;
            _cardDataExtractor = cardDataExtractor;

            // Подписываемся на события
            _eventBus.Subscribe<CardSelectedEvent>(OnCardSelected);
            _eventBus.Subscribe<ConnectionCreatedEvent>(OnConnectionCreated);
            _eventBus.Subscribe<DataLoadedEvent>(OnDataLoaded);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Не создаем SettingsMenu здесь!
            // Просто говорим менеджеру окон показать его
            _windowManager.ShowMenuWindow(this);
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _windowManager.ShowSearchWindow(this);
        }

        // Обработчики событий
        private void OnCardSelected(CardSelectedEvent @event)
        {
            // Получаем данные из карточки БЕЗ сложной логики
            var itemName = _cardDataExtractor.GetItemName(
                @event.Card,
                @event.BorderData.PanelName);

            Console.WriteLine($"Selected: {itemName}");
        }

        private void OnConnectionCreated(ConnectionCreatedEvent @event)
        {
            CreatePermanentConnection(@event.Connection);
        }

        private void OnDataLoaded(DataLoadedEvent @event)
        {
            Console.WriteLine($"Loaded {(@event.Recipes?.Count ?? 0)} recipes");
        }
        
    }
}
/*
private SearchPopUp activeSearchWindow = null;
DataCollectorServise data = new DataCollectorServise();
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

}
// Обработчик нажатия ПКМ в любом месте экрана
private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
{
    OpenSearchPopup(sender, e);
}
// 1. НАЧАЛО: Срабатывает, когда кликнули по Border внутри карточки
private void WorkspaceCanvas_IngredientClicked(RecipeCard card, Point pointInCard, string panelName, Border clickedBorder)
{
    // Передаем карточку, точку клика и цвет

    _curveBuilder.StartDrawing(card, pointInCard, Color.FromRgb(0xa0, 0xa0, 0xa0), panelName, clickedBorder); //a0a0a0

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
    bool wasDrawing = _curveBuilder.StopDrawing(out var connection);

    if (connection != null)
    {
        CreatePermanentConnection(connection);
    }
    else {
        OpenSearchPopup(sender, e);
    }
}

/// <summary>
/// Создать постоянное визуальное соединение между карточками
/// </summary>
private void CreatePermanentConnection(ConnectionInfo conn)
{
    // Визуализировать соединение между карточками по названию предмета


    // Нарисовать постоянную линию на холсте
    var permanentCurve = new Path
    {
        Stroke = new SolidColorBrush(Color.FromRgb(0xa0, 0xa0, 0xa0)),
        StrokeThickness = 3,
        Data = GenerateCurveGeometry(conn.FromCard, conn.ToCard, conn.FromPanel)
    };

    WorkspaceCanvas.Children.Add(permanentCurve);

    // Сохранить информацию о связи
    // (для сохранения/загрузки проекта потом)
    //_connections.Add(conn);
}
private PathGeometry GenerateCurveGeometry(RecipeCard fromCard, RecipeCard toCard,
                                      string fromPanel)
{
    // Получить координаты карточек на холсте
    double fromX = Canvas.GetLeft(fromCard);
    double fromY = Canvas.GetTop(fromCard);
    double toX = Canvas.GetLeft(toCard);
    double toY = Canvas.GetTop(toCard);

    // Если координаты не установлены, устанавливаем в 0
    if (double.IsNaN(fromX)) fromX = 0;
    if (double.IsNaN(fromY)) fromY = 0;
    if (double.IsNaN(toX)) toX = 0;
    if (double.IsNaN(toY)) toY = 0;

    // Где на fromCard выход (OutputsPanel = справа) или вход (InputsPanel = слева)
    Point startPt = fromPanel == "OutputsPanel"
        ? new Point(fromX + fromCard.ActualWidth, fromY + fromCard.ActualHeight / 2)
        : new Point(fromX, fromY + fromCard.ActualHeight / 2);

    // На toCard вход = слева
    Point endPt = new Point(toX, toY + toCard.ActualHeight / 2);

    // Кривая Безье с красивым изгибом
    double offset = Math.Abs(endPt.X - startPt.X) * 0.3;
    offset = Math.Max(30, offset);  // Минимальный изгиб

    var controlPt1 = new Point(startPt.X + offset, startPt.Y);
    var controlPt2 = new Point(endPt.X - offset, endPt.Y);

    var segment = new BezierSegment(controlPt1, controlPt2, endPt, true);
    var figure = new PathFigure(startPt, new[] { segment }, false);
    var geometry = new PathGeometry();
    geometry.Figures.Add(figure);

    return geometry;
}*/


