using FactorioModeler.FCTORIOOUTPUT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading; 
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FactorioModeler.FACTORIOOUTPUT
{
    public class DataCollector
    {
        public string PathToFactorio = SearchFunc.FindFactorioPath();
        public string PathToFactorioExe = SearchFunc.FindFactorioExePath();
        public string ExporterPath = SearchFunc.FindFactorioSrciptOutput();
        public string ModeSourse = SearchFunc.FindModSourse();
        public string PathToMods = SearchFunc.FindFactorioMods();
        private List<FactorioItem> _items;
        private List<FactorioRecipe> _recipes;
        private List<FactorioMachine> _machines;
        private List<FactorioTechnology> _technologies;
        private IconPathMap _iconPaths;
        private Dictionary<string, FactorioItem> _itemCache;
        private Dictionary<string, FactorioRecipe> _recipeCache;
        private Dictionary<string, FactorioMachine> _machineCache;
        private Dictionary<string, FactorioTechnology> _technologyCache;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        /// <summary>
        /// Загрузить все данные из директории
        /// </summary>
        public void LoadAll()
        {
            string dataDirectory = ExporterPath;
            Thread.Sleep(5000);

            LoadItems(Path.Combine(dataDirectory, "items.json"));
            LoadRecipes(Path.Combine(dataDirectory, "recipes.json"));
            LoadMachines(Path.Combine(dataDirectory, "machines.json"));
            LoadTechnologies(Path.Combine(dataDirectory, "technologies.json"));
            //LoadIconPaths(Path.Combine(dataDirectory, "icons.json"));
        }

        /// <summary>
        /// Загрузить предметы
        /// </summary>
        public void LoadItems(string filePath)
        {
            string json = File.ReadAllText(filePath);
            _items = JsonSerializer.Deserialize<List<FactorioItem>>(json, _jsonOptions);
            _itemCache = null; // Инвалидируем кэш
        }

        /// <summary>
        /// Загрузить рецепты
        /// </summary>
        public void LoadRecipes(string filePath)
        {
            string json = File.ReadAllText(filePath);
            _recipes = JsonSerializer.Deserialize<List<FactorioRecipe>>(json, _jsonOptions);
            _recipeCache = null;
        }

        /// <summary>
        /// Загрузить машины
        /// </summary>
        public void LoadMachines(string filePath)
        {
            string json = File.ReadAllText(filePath);
            _machines = JsonSerializer.Deserialize<List<FactorioMachine>>(json, _jsonOptions);
            _machineCache = null;
        }

        /// <summary>
        /// Загрузить технологии
        /// </summary>
        public void LoadTechnologies(string filePath)
        {
            string json = File.ReadAllText(filePath);
            _technologies = JsonSerializer.Deserialize<List<FactorioTechnology>>(json, _jsonOptions);
            _technologyCache = null;
        }

        /// <summary>
        /// Загрузить пути к иконкам
        /// </summary>
        public void LoadIconPaths(string filePath)
        {
            Console.WriteLine("not implemented");
            //string json = File.ReadAllText(filePath);
            //_iconPaths = JsonSerializer.Deserialize<IconPathMap>(json, _jsonOptions);
        }

        // ========================================================================
        // GETTERS (Методы получения данных)
        // ========================================================================

        public List<FactorioItem> GetItems() => _items ?? new();
        public List<FactorioRecipe> GetRecipes() => _recipes ?? new();
        public List<FactorioMachine> GetMachines() => _machines ?? new();
        public List<FactorioTechnology> GetTechnologies() => _technologies ?? new();
        public IconPathMap GetIconPaths() => _iconPaths;

        // ========================================================================
        // SEARCH METHODS (Методы поиска)
        // ========================================================================

        /// <summary>
        /// Найти предмет по названию
        /// </summary>
        public FactorioItem GetItem(string name)
        {
            if (_itemCache == null)
            {
                _itemCache = _items?.ToDictionary(i => i.Name) ?? new();
            }
            _itemCache.TryGetValue(name, out var item);
            return item;
        }

        /// <summary>
        /// Найти рецепт по названию
        /// </summary>
        public FactorioRecipe GetRecipe(string name)
        {
            if (_recipeCache == null)
            {
                _recipeCache = _recipes?.ToDictionary(r => r.Name) ?? new();
            }
            _recipeCache.TryGetValue(name, out var recipe);
            return recipe;
        }

        /// <summary>
        /// Найти машину по названию
        /// </summary>
        public FactorioMachine GetMachine(string name)
        {
            if (_machineCache == null)
            {
                _machineCache = _machines?.ToDictionary(m => m.Name) ?? new();
            }
            _machineCache.TryGetValue(name, out var machine);
            return machine;
        }

        /// <summary>
        /// Найти технологию по названию
        /// </summary>
        public FactorioTechnology GetTechnology(string name)
        {
            if (_technologyCache == null)
            {
                _technologyCache = _technologies?.ToDictionary(t => t.Name) ?? new();
            }
            _technologyCache.TryGetValue(name, out var tech);
            return tech;
        }

        // ========================================================================
        // ADVANCED QUERIES (Продвинутые запросы)
        // ========================================================================

        /// <summary>
        /// Найти все рецепты, которые производят конкретный предмет
        /// </summary>
        public List<FactorioRecipe> GetRecipesProducing(string itemName)
        {
            return _recipes
                ?.Where(r => r.Products.Any(p => p.Name == itemName))
                .ToList() ?? new();
        }

        /// <summary>
        /// Найти все машины, которые могут производить конкретный рецепт
        /// </summary>
        public List<FactorioMachine> GetMachinesForRecipe(string recipeName)
        {
            var recipe = GetRecipe(recipeName);
            if (recipe == null) return new();

            return _machines
                ?.Where(m => m.CraftingCategories?.Contains(recipe.Category) ?? false)
                .ToList() ?? new();
        }

        /// <summary>
        /// Получить цепочку зависимостей технологий
        /// </summary>
        public List<string> GetTechnologyDependencyChain(string techName)
        {
            var chain = new List<string> { techName };
            var tech = GetTechnology(techName);

            if (tech?.Prerequisites != null)
            {
                foreach (var prereq in tech.Prerequisites)
                {
                    chain.AddRange(GetTechnologyDependencyChain(prereq));
                }
            }

            return chain.Distinct().ToList();
        }

        /// <summary>
        /// Найти все рецепты в конкретной категории
        /// </summary>
        public List<FactorioRecipe> GetRecipesByCategory(string category)
        {
            return _recipes
                ?.Where(r => r.Category == category)
                .ToList() ?? new();
        }

        /// <summary>
        /// Получить иконку предмета (NOT WORKING)
        /// </summary>
        public void GetItemIcon(string itemName)
        {
            Console.WriteLine("not implemented");
            //return _iconPaths?.Items?.TryGetValue(itemName, out var path) == true ? path : null;
        }

        /// <summary>
        /// Получить иконку рецепта
        /// </summary>
        public string GetRecipeIcon(string recipeName)
        {
            return _iconPaths?.Recipes?.TryGetValue(recipeName, out var path) == true ? path : null;
        }

        // ========================================================================
        // STATISTICS (Статистика)
        // ========================================================================

        public int GetItemCount() => _items?.Count ?? 0;
        public int GetRecipeCount() => _recipes?.Count ?? 0;
        public int GetMachineCount() => _machines?.Count ?? 0;
        public int GetTechnologyCount() => _technologies?.Count ?? 0;

        public string Statistics()
        {
            string s = "" +
                $"Factorio Data Statistics{Environment.NewLine}" +
                $"Items: {GetItemCount()}{Environment.NewLine}" +
                $"Recipes: {GetRecipeCount()}{Environment.NewLine}" +
                $"Machines: {GetMachineCount()}{Environment.NewLine}" +
                $"Technologies: {GetTechnologyCount()}";
            return s;
        }

        static void CopyModeToDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);
            var dirdist = new DirectoryInfo(destinationDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            var filesA = dir.GetFiles("*.*", SearchOption.AllDirectories).Select(f => f.Name);
            var filesB = dirdist.GetFiles("*.*", SearchOption.AllDirectories).Select(f => f.Name);

            foreach (var filesdir in filesA)
            {
                foreach (var filesdirsist in filesB)
                    if (filesdir.Equals(filesdirsist))
                    {
                        return;
                    }
            }
            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {

                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyModeToDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public async void LaunchGame(string SaveName)
        {
            if (SaveName == "") //TODO implement
            {
                SaveName = SearchFunc.FindFactorioSave() + "\\ForScript.zip";//"C:\\Users\\dima\\AppData\\Roaming\\Factorio\\saves\\ForScript.zip"
            }
            // "C:\Program Files (x86)\Steam\steamapps\common\Factorio\bin\x64\factorio.exe" --start-server "C:\Users\dima\AppData\Roaming\Factorio\saves\123.zip"
            string argument = $"--start-server \"{SaveName}\"";

            string targetLine = "changing state from(CreatingGame) to(InGame)";

            StartServerAsWaitForLineAsync(PathToFactorioExe, argument, targetLine);

            

        }
        static void StartServerAsWaitForLineAsync(string pathToExe, string argument, string targetLine)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pathToExe.Trim('"'),
                Arguments = $"{argument}",
                RedirectStandardOutput = true, // Разрешаем перехват вывода
                //RedirectStandardError = true,  // Разрешаем перехват ошибок
                UseShellExecute = false,       // Обязательно для перенаправления потоков
                CreateNoWindow = true,         // Не открывать черное окно консоли cmd
                StandardOutputEncoding = System.Text.Encoding.GetEncoding(866) // Исправляет кракозябры (кириллицу) в Windows
            };

            Process process = new Process { StartInfo = startInfo };

            process.OutputDataReceived += (sender, e) =>
            {
                if (string.IsNullOrEmpty(e.Data)) return;

                // Выводим логи в консоль, чтобы видеть что происходит
                Console.WriteLine($"[Factorio]: {e.Data}");

                // Если нашли нужную строку и задача еще не была завершена
                if (e.Data.Contains(targetLine, StringComparison.OrdinalIgnoreCase))
                {
                    StopFactorioServer(process);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            
            }
        static void StopFactorioServer(Process serverProcess)
        {

            Console.WriteLine("Останавливаем сервер...");
            serverProcess.Kill();
            serverProcess.WaitForExit();
            serverProcess.Dispose();
            Console.WriteLine("Сервер успешно остановлен.");

        }

        public string StartDataColletctor()
        {

            string SaveName = ""; //TODO ForScript => SaveName = "C:\\Users\\dima\\AppData\\Roaming\\Factorio\\saves\\ForScript.zip"

            CopyModeToDirectory(ModeSourse, PathToMods, true);
            LaunchGame(SaveName);
            LoadAll();
            var statistic = Statistics();
            return statistic;
        }
    }
}
