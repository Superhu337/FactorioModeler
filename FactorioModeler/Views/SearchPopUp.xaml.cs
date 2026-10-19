using FactorioModeler.Models;
using FactorioModeler.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // нужно для работы с JsonElement
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;      // Для Keyboard.Focus
using System.Windows.Threading;

namespace FactorioModeler.Views
{
    public partial class SearchPopUp : Window
    {
        private readonly DataCollector _dataCollector;
        private bool _isClosing = false;
        public SearchPopUp(DataCollector dataCollector)
        {
            InitializeComponent();
            _dataCollector = dataCollector;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем поле и результаты, а не закрываем окно
            SearchTextBox.Clear();
            ResultsListBox.ItemsSource = null;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearch();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        // Этот метод будет вызываться при любом изменении тумблеров
        private void SearchSettings_Changed(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        // Закрытие по нажатию клавиши Esc
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClosePopup();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            if (!_isClosing)
            {
                _isClosing = true;
                this.Close();
            }
        }


        public void FocusSearchBox()
        {
            // Используем Dispatcher с приоритетом Input, чтобы фокус установился 
            // только после того, как окно полностью отрисуется и станет активным.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SearchTextBox.Focus();
                Keyboard.Focus(SearchTextBox);
                SearchTextBox.SelectAll(); // Выделяем текст, если он там был
            }), DispatcherPriority.Input);
        }

        private void PerformSearch()
        {
            if (ResultsListBox == null || SearchTextBox == null) return;

            string query = SearchTextBox.Text?.Trim() ?? "";

            // Если поле пустое - очищаем результаты
            if (string.IsNullOrEmpty(query))
            {
                ResultsListBox.ItemsSource = null;
                return;
            }

            // Считываем состояние переключателей
            bool searchName = ToggleRecipeName.IsChecked == true;
            bool searchInputs = ToggleInputs.IsChecked == true;
            bool searchOutputs = ToggleOutputs.IsChecked == true;

            // Если пользователь выключил все тумблеры, по умолчанию ищем по имени
            if (!searchName && !searchInputs && !searchOutputs)
            {
                searchName = true;
            }

            var recipes = _dataCollector.GetRecipes();

            // Фильтруем рецепты
            var results = recipes.Where(r =>
            {
                // 1. Поиск по названию рецепта (Name или LocalisedName)
                if (searchName)
                {
                    if (Contains(r.Name, query)) return true;

                    // Проверяем локализованное имя, если оно представлено строкой в JSON
                    if (r.LocalisedName.ValueKind == JsonValueKind.String)
                    {
                        if (Contains(r.LocalisedName.GetString(), query)) return true;
                    }
                }

                // 2. Поиск по входным ингредиентам (Inputs)
                if (searchInputs && r.Ingredients != null)
                {
                    if (r.Ingredients.Any(i => Contains(i.Name, query))) return true;
                }

                // 3. Поиск по выходным продуктам (Outputs)
                if (searchOutputs && r.Products != null)
                {
                    if (r.Products.Any(p => Contains(p.Name, query))) return true;
                }

                return false;
            }).ToList();

            // Выводим результат
            ResultsListBox.ItemsSource = results;
            //Console.WriteLine(results);
        }


        // Обновленный метод поиска (нестрогий)
        private bool Contains(string source, string query)
        {
            if (string.IsNullOrEmpty(source)) return false;

            string normalizedSource = Normalize(source);
            string normalizedQuery = Normalize(query);

            return normalizedSource.Contains(normalizedQuery);
        }


        // Вспомогательный метод: убирает дефисы, пробелы, подчеркивания и делает буквы строчными
        private string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("-", "")
                       .Replace("_", "")
                       .Replace(" ", "")
                       .ToLowerInvariant();
        }
    }
}