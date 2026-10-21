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
    public partial class RecipeCard : Window
    {
        private const string DEFAULT_ICON_PATH = "/ui_icons/factory-default.png";
        private const string DEFAULT_INPUT_PATH = "/ui_icons/input-default.png";
        private const string DEFAULT_OUTPUT_PATH = "/ui_icons/output-default.png";

        private const int ICON_SIZE_SINGLE = 32;      // Когда 1 вход/выход
        private const int ICON_SIZE_MULTIPLE = 32;    // Когда 2+ входов/выходов
        private const int ICON_SPACING = 3;    // Когда 2+ входов/выходов
        private const int BORDER_THICKNESS = 3;       // Толщина рамки



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

        public void SpawnCard(FactorioRecipe recipe, DataCollector data)
        {
            // Иконка процесса (пока null → дефолт, см. примечание ниже)
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

                var border = new Border
                {
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE,
                    //Margin = new Thickness(10,0,0,0),
                    Background = new SolidColorBrush(backgroundColor),
                    ToolTip = $"{item.Amount}x {item.Name}"   // наглядно
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


        //public void ChangeText(int a, TextBlock panel)
        //{
        //    List<string> list = [];

        //    for (int i = 1; i <= a; i++)
        //    {
        //        list.Add(i.ToString());
        //    }

        //    StringBuilder sb = new StringBuilder();
        //    foreach (string item in list)
        //    {
        //        sb.AppendLine(item); // Добавляет элемент и перенос строки в конец
        //    }

        //    string result = sb.ToString();

        //    panel.Text = result;
        //}

        //private void MouseLeftButt_onDown(object sender, MouseButtonEventArgs e)
        //{
        //    Point positionOnScreen = ProcessIcon.PointToScreen(new Point(0, 0));

        //    double screenX = positionOnScreen.X;
        //    double screenY = positionOnScreen.Y;
        //    //// 1. Берем вашу иконку процесса (или ProcessBorder)
        //    //UIElement element = ProcessIcon;

        //    //// 2. Вычисляем трансформацию относительно главного окна (this)
        //    //GeneralTransform transform = element.TransformToAncestor(this);

        //    //// 3. Получаем точку (0,0) - верхний левый угол иконки относительно окна
        //    //Point positionInWindow = transform.Transform(new Point(0, 0));

        //    //// Теперь у вас есть точные X и Y внутри окна:
        //    //double x = positionInWindow.X;
        //    //double y = positionInWindow.Y;

        //    //Console.WriteLine(x);
        //    //Console.WriteLine(y);

        //    // Отмечаем событие как обработанное, чтобы клик не "проваливался" в карточку
        //    e.Handled = true;

        //    // sender — это именно тот Border внутри InputsPanel, на который нажали
        //    Border clickedBorder = sender as Border;

        //    if (clickedBorder != null)
        //    {
        //        // Пример: получаем координаты этого Border относительно текущего окна карточки
        //        GeneralTransform transform = clickedBorder.TransformToAncestor(this);
        //        Point borderPosition = transform.Transform(new Point(0, 0));

        //        // Логика при клике (пока покажем сообщение с координатами)
        //        //MessageBox.Show($"Нажали на входной ингредиент! Координаты в окне: X={borderPosition.X}, Y={borderPosition.Y} \nКоординаты окна: X={screenX}, Y={screenY}");
        //    }

        //}

        // Новое событие: передает саму карточку и точку, где нажали
        public event Action<RecipeCard, Point> IngredientClicked;

        private void MouseLeftButt_onDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Border clickedBorder = sender as Border;

            if (clickedBorder != null)
            {
                // Переводим координаты кликнутого Border в координаты относительно ОКНА КАРТОЧКИ
                GeneralTransform transform = clickedBorder.TransformToAncestor(this);
                Point borderCenterInCard = transform.Transform(new Point(ICON_SIZE_SINGLE / 2, ICON_SIZE_SINGLE / 2));

                // Захватываем мышь, чтобы отслеживать отпускание, даже если курсор уйдет с карточки
                this.CaptureMouse();

                // Вызываем событие для главного окна
                IngredientClicked?.Invoke(this, borderCenterInCard);
            }
        }


        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); // Запускает встроенный механизм перетаскивания окна
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
}
