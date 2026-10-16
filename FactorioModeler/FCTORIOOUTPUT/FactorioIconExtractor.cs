// ============================================================================
// Factorio Icon Extractor - Копирует иконки из Factorio в проект
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Извлекает и копирует иконки предметов и рецептов из Factorio
/// </summary>

namespace FactorioModeler.FACTORIOOUTPUT
{
    public class FactorioIconExtractor
    {
        private readonly string _factorioDataPath;
        private readonly string _outputPath;
        private readonly JsonSerializerOptions _jsonOptions;

        

        public FactorioIconExtractor(string factorioDataPath, string outputPath)
        {
            _factorioDataPath = factorioDataPath;
            _outputPath = outputPath;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        /// <summary>
        /// Автоматически найти путь к Factorio
        /// </summary>
        public static string FindFactorioPath()
        {
            string[] possiblePaths = new[]
            {
                // Windows Steam
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Steam", "steamapps", "common", "Factorio", "data"),
            
                // Windows GOG
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "GOG Games", "Factorio", "data"),
            
                // Linux Steam
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".steam", "steam", "steamapps", "common", "Factorio", "data"),
            
                // macOS Steam
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Library", "Application Support", "Steam", "steamapps", "common", "Factorio", "data")
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"✓ Найден Factorio: {path}");
                    return path;
                }
            }

            throw new DirectoryNotFoundException(
                "Не найена папка Factorio. Убедитесь, что Factorio установлен или укажите путь вручную.");
        }

        /// <summary>
        /// Извлечь иконки из JSON маппинга (из icons.json)
        /// </summary>
        public void ExtractIconsFromMapping(string iconsJsonPath)
        {
            Console.WriteLine($"\n📥 Чтение маппинга иконок из: {iconsJsonPath}");

            if (!File.Exists(iconsJsonPath))
            {
                Console.WriteLine($"✗ Файл не найден: {iconsJsonPath}");
                return;
            }

            string json = File.ReadAllText(iconsJsonPath);
            var iconMapping = JsonSerializer.Deserialize<IconMapping>(json, _jsonOptions);

            int copiedCount = 0;

            // Копируем иконки предметов
            if (iconMapping?.Items != null)
            {
                copiedCount += CopyIconsFromPaths(iconMapping.Items, "items");
            }

            // Копируем иконки рецептов
            if (iconMapping?.Recipes != null)
            {
                copiedCount += CopyIconsFromPaths(iconMapping.Recipes, "recipes");
            }

            Console.WriteLine($"\n✓ Всего скопировано иконок: {copiedCount}");
        }

        /// <summary>
        /// Копировать иконки по путям из маппинга
        /// </summary>
        private int CopyIconsFromPaths(Dictionary<string, string> pathMap, string category)
        {
            Console.WriteLine($"\n📁 Обработка {category}...");

            string categoryFolder = Path.Combine(_outputPath, category);
            if (!Directory.Exists(categoryFolder))
            {
                Directory.CreateDirectory(categoryFolder);
            }

            int count = 0;

            foreach (var kvp in pathMap)
            {
                string name = kvp.Key;
                string relativePath = kvp.Value;

                if (string.IsNullOrEmpty(relativePath))
                    continue;

                // Преобразуем путь из формата Factorio
                string sourceFile = ConvertFactorioPathToReal(relativePath);

                if (!File.Exists(sourceFile))
                {
                    sourceFile = FindIconFile(sourceFile);
                    if (sourceFile == null)
                        continue;
                }

                try
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string destFile = Path.Combine(categoryFolder, $"{name}{Path.GetExtension(sourceFile)}");

                    File.Copy(sourceFile, destFile, true);
                    count++;

                    if (count % 10 == 0)
                        Console.WriteLine($"  ... {count} файлов");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ {name}: {ex.Message}");
                }
            }

            Console.WriteLine($"  ✓ {category}: {count} иконок");
            return count;
        }

        /// <summary>
        /// Преобразовать путь Factorio в реальный путь файловой системы
        /// </summary>
        private string ConvertFactorioPathToReal(string factorioPath)
        {
            factorioPath = factorioPath.Replace("__base__/", "base/");
            factorioPath = factorioPath.Replace("__core__/", "core/");
            return Path.Combine(_factorioDataPath, factorioPath);
        }

        /// <summary>
        /// Найти иконку с разными расширениями
        /// </summary>
        private string FindIconFile(string basePath)
        {
            string[] extensions = { ".png", ".jpg", ".jpeg", ".bmp" };
            string pathWithoutExt = Path.Combine(Path.GetDirectoryName(basePath), Path.GetFileNameWithoutExtension(basePath));

            foreach (var ext in extensions)
            {
                string filePath = pathWithoutExt + ext;
                if (File.Exists(filePath))
                    return filePath;
            }

            try
            {
                var dir = Path.GetDirectoryName(basePath);
                if (Directory.Exists(dir))
                {
                    var fileName = Path.GetFileNameWithoutExtension(basePath);
                    var files = Directory.GetFiles(dir, fileName + "*").FirstOrDefault();
                    if (files != null)
                        return files;
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Сканировать и копировать иконки напрямую из папки Factorio
        /// (без использования JSON маппинга)
        /// </summary>
        public void ScanAndCopyAllIcons(bool recursive = true)
        {
            Console.WriteLine("\n🔍 Сканирование папок иконок Factorio...");

            var iconPaths = new[]
            {
                Path.Combine(_factorioDataPath, "base", "graphics", "icons"),
                Path.Combine(_factorioDataPath, "core", "graphics", "icons")
            };

            int totalCopied = 0;

            foreach (var iconPath in iconPaths)
            {
                if (!Directory.Exists(iconPath))
                {
                    Console.WriteLine($"⚠ Папка не найдена: {iconPath}");
                    continue;
                }

                Console.WriteLine($"\n📁 Копирование из: {iconPath}");

                var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var files = Directory.GetFiles(iconPath, "*.*", searchOption)
                    .Where(f => new[] { ".png", ".jpg", ".jpeg", ".bmp" }.Contains(Path.GetExtension(f).ToLower()))
                    .ToList();

                Console.WriteLine($"  Найдено файлов: {files.Count}");

                foreach (var file in files)
                {
                    try
                    {
                        string relativePath = PathExtensions.GetRelativePath(iconPath, file);
                        string destFolder = Path.Combine(_outputPath, "all_icons", Path.GetDirectoryName(relativePath));

                        if (!Directory.Exists(destFolder))
                        {
                            Directory.CreateDirectory(destFolder);
                        }

                        string destFile = Path.Combine(destFolder, Path.GetFileName(file));
                        File.Copy(file, destFile, true);

                        totalCopied++;

                        if (totalCopied % 10 == 0)
                            Console.WriteLine($"  ... {totalCopied} файлов");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ✗ Ошибка: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"\n✓ Всего скопировано иконок: {totalCopied}");
            Console.WriteLine($"📁 Иконки в: {Path.Combine(_outputPath, "all_icons")}");
        }

        /// <summary>
        /// Создать маппинг иконок на основе найденных файлов
        /// </summary>
        public void GenerateIconMapping(string outputJsonPath)
        {
            Console.WriteLine("\n📝 Генерирование маппинга иконок...");

            var mapping = new IconMapping
            {
                Items = new Dictionary<string, string>(),
                Recipes = new Dictionary<string, string>()
            };

            var basePaths = new[]
            {
                Path.Combine(_factorioDataPath, "base", "graphics", "icons"),
                Path.Combine(_factorioDataPath, "core", "graphics", "icons")
            };

            foreach (var basePath in basePaths)
            {
                if (!Directory.Exists(basePath))
                    continue;

                var itemsFolder = Path.Combine(basePath, "items");
                if (Directory.Exists(itemsFolder))
                {
                    foreach (var file in Directory.GetFiles(itemsFolder, "*.*", SearchOption.AllDirectories))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        string relativePath = PathExtensions.GetRelativePath(_factorioDataPath, file);
                        mapping.Items[name] = relativePath;
                    }
                }

                var recipesFolder = Path.Combine(basePath, "recipes");
                if (Directory.Exists(recipesFolder))
                {
                    foreach (var file in Directory.GetFiles(recipesFolder, "*.*", SearchOption.AllDirectories))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        string relativePath = PathExtensions.GetRelativePath(_factorioDataPath, file);
                        mapping.Recipes[name] = relativePath;
                    }
                }
            }

            Console.WriteLine($"  Предметов: {mapping.Items.Count}");
            Console.WriteLine($"  Рецептов: {mapping.Recipes.Count}");

            string json = JsonSerializer.Serialize(mapping, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputJsonPath, json);

            Console.WriteLine($"✓ Маппинг: {outputJsonPath}");
        }
    }

    /// <summary>
    /// Структура маппинга иконок
    /// </summary>
    public class IconMapping
    {
        [JsonPropertyName("items")]
        public Dictionary<string, string> Items { get; set; }

        [JsonPropertyName("recipes")]
        public Dictionary<string, string> Recipes { get; set; }
    }

    // ============================================================================
    // ПРИМЕР ИСПОЛЬЗОВАНИЯ В MAIN
    // ============================================================================

    /*
    static void Main()
    {
        try
        {
            // Найти Factorio автоматически
            string factorioPath = FactorioIconExtractor.FindFactorioPath();
        
            // Создать экстрактор
            var extractor = new FactorioIconExtractor(factorioPath, "Icons");

            // ВАРИАНТ 1: Копировать иконки по маппингу из icons.json мода
            extractor.ExtractIconsFromMapping("Data/icons.json");

            // ВАРИАНТ 2: Копировать все иконки из Factorio
            // extractor.ScanAndCopyAllIcons(recursive: true);

            // ВАРИАНТ 3: Сгенерировать маппинг иконок
            // extractor.GenerateIconMapping("icons_mapping.json");

            Console.WriteLine("\n✅ Готово!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка: {ex.Message}");
        }
    }
    */
}