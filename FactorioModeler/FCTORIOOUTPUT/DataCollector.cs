using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FactorioModeler.FACTORIOOUTPUT
{
    public class DataCollector
    {
        public string PathToFactorio = FactorioIconExtractor.FindFactorioPath();    
        string ExporterPath = "script-output\\exporter";
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
        public void LoadAll(string dataDirectory)
        {
            LoadItems(Path.Combine(dataDirectory, "items.json"));
            LoadRecipes(Path.Combine(dataDirectory, "recipes.json"));
            LoadMachines(Path.Combine(dataDirectory, "machines.json"));
            LoadTechnologies(Path.Combine(dataDirectory, "technologies.json"));
            LoadIconPaths(Path.Combine(dataDirectory, "icons.json"));
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
            string json = File.ReadAllText(filePath);
            _iconPaths = JsonSerializer.Deserialize<IconPathMap>(json, _jsonOptions);
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
        /// Получить иконку предмета
        /// </summary>
        public string GetItemIcon(string itemName)
        {
            return _iconPaths?.Items?.TryGetValue(itemName, out var path) == true ? path : null;
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

        public void PrintStatistics()
        {
            Console.WriteLine("=== Factorio Data Statistics ===");
            Console.WriteLine($"Items: {GetItemCount()}");
            Console.WriteLine($"Recipes: {GetRecipeCount()}");
            Console.WriteLine($"Machines: {GetMachineCount()}");
            Console.WriteLine($"Technologies: {GetTechnologyCount()}");
        }


    }
}
