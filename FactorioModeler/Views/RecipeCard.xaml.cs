using FactorioModeler.Models;
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
        private const string DEFAULT_ICON_PATH = "/ui_icons/mining-default.png";
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

        public void SpawnCard(int inputs, int outputs)
        {

            ProcessIcon.Source = LoadImage(DEFAULT_ICON_PATH);

            CreateInput(inputs);
            CreateOutput(outputs);
            //ResizeCardBasedOnItems(inputs,outputs);
            
        }

        public void CreateInput(int count)
        {
            int iconSize = ICON_SIZE_SINGLE;
            

            InputsPanel.Width = iconSize;
            InputsPanel.Children.Clear();
            Input.Text = $"{count}";
            for (int i = 0; i < count; i++)
            {
                var border = new Border
                {

                    Width = iconSize,
                    Height = iconSize,
                    Background = new SolidColorBrush(Color.FromRgb(0xff, 0xaf, 0xaf)) //ffafaf
                };

                var image = new Image
                {
                    Source = LoadImage(DEFAULT_OUTPUT_PATH),
                    Width = iconSize,
                    Height = iconSize
                };

                border.Child = image;
                InputsPanel.Children.Add(border);
                
            }
        }

        public void CreateOutput(int count)
        {
            int iconSize = ICON_SIZE_SINGLE;
            Output.Text = $"{count}";
            OutputsPanel.Width = iconSize;
            OutputsPanel.Children.Clear();

            for (int i = 0; i < count; i++)
            {

                var border = new Border
                {
                    
                    Width = iconSize,
                    Height = iconSize,
                    Background = new SolidColorBrush(Color.FromRgb(0x95, 0xf6, 0xa8)) //95f6a8
                };

                var image = new Image
                {
                    Source = LoadImage(DEFAULT_OUTPUT_PATH),
                    Width = iconSize,
                    Height = iconSize,
                };

                border.Child = image;
                OutputsPanel.Children.Add(border);
                
            }
        }


        /// <summary>
        /// Изменить размер карточки в зависимости от количества входов/выходов
        /// </summary>
        private void ResizeCardBasedOnItems(int inputCount, int outputCount)
        {
            int iconHeight = 32;

            // Вычисляем высоту, требуемую для иконок
            int requiredHeight = Math.Max(inputCount, outputCount) * (iconHeight + ICON_SPACING * 2);

            // Если входов/выходов 1, используем фиксированную высоту
            if (inputCount <= 1 && outputCount <= 1)
            {
                requiredHeight = 118;  // Стандартная высота
            }
            else
            {
                // Минимум 118, максимум для 6 элементов
                requiredHeight = Math.Max(118, requiredHeight + BORDER_THICKNESS * 2 + 24);
            }

            // Устанавливаем размеры окна
            this.Height = requiredHeight;
            MainRow.Height = new GridLength(requiredHeight - 24 - BORDER_THICKNESS);

            MainBorder.Height = requiredHeight - BORDER_THICKNESS;
        }

    }
}
