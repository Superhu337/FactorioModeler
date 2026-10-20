using FactorioModeler.Models;
using FactorioModeler.Scripts;
using System;
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

            CreateInputs(recipe.Ingredients);
            CreateOutputs(recipe.Products);

            ResizeCardBasedOnItems(
                recipe.Ingredients?.Count ?? 0,
                recipe.Products?.Count ?? 0);
        }

        private void CreateInputs(List<RecipeIngredient> ingredients)
        {
            InputsPanel.Children.Clear();
            InputsPanel.Width = ICON_SIZE_SINGLE;

            if (ingredients == null || ingredients.Count == 0)
            {
                Input.Text = "0";
                return;
            }


            foreach (var ing in ingredients)
            {

                var TextBlock = new TextBlock //<TextBlock x:Name="Output" Text="output" FontSize="14" HorizontalAlignment="Right" VerticalAlignment="Top" Foreground="#f8f8f8" Grid.Column="3" Margin="0,11,0,0"/>
                {
                    Text = "cdj",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 0),
                    Foreground = new SolidColorBrush(Color.FromRgb(0xf8, 0xf8, 0xf8))
                };

                var border = new Border
                {
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE,
                    //Margin = new Thickness(10,0,0,0),
                    Background = new SolidColorBrush(Color.FromRgb(0xff, 0xaf, 0xaf)),
                    ToolTip = $"{ing.Amount}x {ing.Name}"    // наглядно
                };

                var image = new Image
                {
                    Source = LoadImage(DEFAULT_INPUT_PATH),
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE
                };

                border.Child = image;
                InputsPanel.Children.Add(border);
            }
        }

        private void CreateOutputs(List<RecipeProduct> products)
        {
            OutputsPanel.Children.Clear();
            OutputsPanel.Width = ICON_SIZE_SINGLE;

            if (products == null || products.Count == 0)
            {
                Output.Text = "0";
                return;
            }

            ChangeTextOutputs(products.Count);

            foreach (var p in products)
            {
                var border = new Border
                {
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE,
                    Background = new SolidColorBrush(Color.FromRgb(0x95, 0xf6, 0xa8)),
                    ToolTip = $"{p.Amount}x {p.Name}"
                };

                var image = new Image
                {
                    Source = LoadImage(DEFAULT_OUTPUT_PATH),
                    Width = ICON_SIZE_SINGLE,
                    Height = ICON_SIZE_SINGLE
                };

                border.Child = image;
                OutputsPanel.Children.Add(border);
            }
        }

        public void ChangeTextInputs(int a)
        {
            List<string> list = [];

            for (int i = 1; i <= a; i++)
            {
                list.Add(i.ToString());
            }

            StringBuilder sb = new StringBuilder();
            foreach (string item in list)
            {
                sb.AppendLine(item); // Добавляет элемент и перенос строки в конец
            }

            string result = sb.ToString();

            Input.Text = result;
        }
        public void ChangeTextOutputs(int a)
        {

            List<string> list = [];

            for (int i = 1; i <= a; i++)
            {
                list.Add(i.ToString());
            }

            StringBuilder sb = new StringBuilder();
            foreach (string item in list)
            {
                sb.AppendLine(item); // Добавляет элемент и перенос строки в конец
            }

            string result = sb.ToString();

            Output.Text = result;
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
            int requiredHeight = Math.Max(inputCount, outputCount) * iconHeight;

            // Если входов/выходов 1, используем фиксированную высоту
            if (inputCount <= 1 && outputCount <= 1)
            {
                requiredHeight = 126;  // Стандартная высота
            }
            else
            {
                // Минимум 118, максимум для 6 элементов
                requiredHeight = 126;//Math.Max(126, requiredHeight + BORDER_THICKNESS * 2 + 24);
            }

            // Устанавливаем размеры окна
            this.Height = requiredHeight;
            //MainRow.Height = new GridLength(requiredHeight - 24 - BORDER_THICKNESS);

            MainBorder.Height = requiredHeight - BORDER_THICKNESS;
        }

    }
}
