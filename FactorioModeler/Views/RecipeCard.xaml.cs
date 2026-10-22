using FactorioModeler.Models;
using FactorioModeler.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;     

namespace FactorioModeler.Views
{
    /// <summary>
    /// Логика взаимодействия для RecipeCard.xaml
    /// </summary>
    /// 
    public partial class RecipeCard : UserControl
    {
        private const string DEFAULT_ICON_PATH = "/ui_icons/factory-default.png";
        private const string DEFAULT_INPUT_PATH = "/ui_icons/input-default.png";
        private const string DEFAULT_OUTPUT_PATH = "/ui_icons/output-default.png";

        private const int ICON_SIZE_SINGLE = 32;       
        private const int ICON_SPACING = 3;    
        private const int BORDER_THICKNESS = 3;      

        public event Action<RecipeCard, Point, string, Border> IngredientClicked;

        public RecipeCard()
        {
            InitializeComponent();

        }
        private BitmapImage LoadImage(string path)
        {

            var uri = new Uri($"pack://application:,,,{path}", UriKind.Absolute);
            var image = new BitmapImage(uri);
            image.Freeze();
            return image;
        }

        public void SpawnCard(FactorioRecipe recipe, DataCollectorServise data)
        {
            
            string iconPath = data?.GetRecipeIcon(recipe.Name);
            ProcessIcon.Source = LoadImage(iconPath ?? DEFAULT_ICON_PATH);

            // ВЫЗОВ ДЛЯ ВХОДОВ (Розовый фон, иконка входа)
            CreateInputsOutputs(
                recipe.Ingredients,
                InputsPanel,
                InputsNumbers,
                Color.FromRgb(0xff, 0xaf, 0xaf),
                DEFAULT_INPUT_PATH
            );

            // ВЫЗОВ ДЛЯ ВЫХОДОВ (Зеленый фон, иконка выхода)
            CreateInputsOutputs(
                recipe.Products,
                OutputsPanel,
                OutputsNumbers,
                Color.FromRgb(0x95, 0xf6, 0xa8),
                DEFAULT_OUTPUT_PATH
            );

            ResizeCardBasedOnItems(
                recipe.Ingredients?.Count ?? 0,
                recipe.Products?.Count ?? 0);
        }

        private void CreateInputsOutputs<T>(List<T> list, Panel targetpanel, Panel targetpanelNumbers, Color backgroundColor, string defaultImagePath)
        {
            targetpanel.Children.Clear();
            targetpanelNumbers.Children.Clear();
            targetpanel.Width = ICON_SIZE_SINGLE;



            if (list == null || list.Count == 0) return;

            foreach (dynamic item in list)
            {

                var TextBlock = new TextBlock
                {
                    Text = item.Amount.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 8, 0, 8),
                    Foreground = new SolidColorBrush(Color.FromRgb(0xf8, 0xf8, 0xf8))
                };
                targetpanelNumbers.Children.Add(TextBlock);

                var borderData = new BorderData
                {
                    ItemName = item.Name,
                    ItemAmount = item.Amount,
                    PanelName = targetpanel.Name
                };

                var border = new Border
                {
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE,
                    Background = new SolidColorBrush(backgroundColor),
                    ToolTip = $"{item.Amount}x {item.Name}",
                    Tag = borderData
                };


                var image = new Image
                {
                    Source = LoadImage(defaultImagePath),
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE
                };


                border.MouseLeftButtonDown += MouseLeftButt_onDown;
                 
                border.Child = image;
                targetpanel.Children.Add(border);

            }
        }

        private void MouseLeftButt_onDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Border clickedBorder = sender as Border;

            if (clickedBorder != null)
            {
                clickedBorder.Background = new SolidColorBrush(Color.FromRgb(0x60, 0x60, 0x60)); //606060

                Panel parentPanel = clickedBorder.Parent as Panel;
                string panelName = parentPanel?.Name ?? "Unknown";

                // Переводим координаты кликнутого Border в координаты относительно ОКНА КАРТОЧКИ
                GeneralTransform transform = clickedBorder.TransformToAncestor(this);
                Point borderCenterInCard = transform.Transform(new Point(ICON_SIZE_SINGLE / 2, ICON_SIZE_SINGLE / 2));

                // Захватываем мышь, чтобы отслеживать отпускание, даже если курсор уйдет с карточки
                this.CaptureMouse();

                // Вызываем событие для главного окна
                IngredientClicked?.Invoke(this, borderCenterInCard, panelName, clickedBorder);
            }
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Вместо DragMove запускаем стандартный перенос элемента внутри Canvas
                var card = this;
                var canvas = VisualTreeHelper.GetParent(card) as Canvas;

                if (canvas != null)
                {
                    e.Handled = true;

                    // Захватываем мышь для перемещения карточки
                    card.CaptureMouse();

                    Point startMousePos = e.GetPosition(canvas);
                    double startLeft = Canvas.GetLeft(card);
                    double startTop = Canvas.GetTop(card);

                    // Если координаты еще не были заданы на Canvas, инициализируем их текущим положением
                    if (double.IsNaN(startLeft)) startLeft = 0;
                    if (double.IsNaN(startTop)) startTop = 0;

                    MouseEventHandler mouseMoveHandler = null;
                    mouseMoveHandler = (s, args) =>
                    {
                        Point currentMousePos = args.GetPosition(canvas);
                        Canvas.SetLeft(card, startLeft + (currentMousePos.X - startMousePos.X));
                        Canvas.SetTop(card, startTop + (currentMousePos.Y - startMousePos.Y));
                    };

                    MouseButtonEventHandler mouseUpHandler = null;
                    mouseUpHandler = (s, args) =>
                    {
                        card.ReleaseMouseCapture();
                        card.MouseMove -= mouseMoveHandler;
                        card.MouseLeftButtonUp -= mouseUpHandler;
                    };

                    card.MouseMove += mouseMoveHandler;
                    card.MouseLeftButtonUp += mouseUpHandler;
                }
            }
        }
        /// <summary>
        /// Изменить размер карточки в зависимости от количества входов/выходов
        /// </summary>
        private void ResizeCardBasedOnItems(int inputCount, int outputCount)
        {
            int iconHeight = 32;

            // Вычисляем высоту, требуемую для иконок
            int requiredHeight = 126;

            // Если входов/выходов 1, используем фиксированную высоту
            if (inputCount <= 3 && outputCount <= 3)
            {
                requiredHeight = 126;  // Стандартная высота
            }
            else
            {   
                requiredHeight+= (Math.Max(inputCount, outputCount)-3) * iconHeight;
            }
            //Console.WriteLine(requiredHeight);
            //Console.WriteLine(inputCount);
            // Устанавливаем размеры окна
            m.Height = requiredHeight;
            //MainRow.Height = new GridLength(requiredHeight - 24 - BORDER_THICKNESS);

            MainBorder.Height = requiredHeight; //- BORDER_THICKNESS;
        }

    }
    public class BorderData
    {
        public string ItemName { get; set; }
        public double ItemAmount { get; set; }
        public string PanelName { get; set; }
    }
}
