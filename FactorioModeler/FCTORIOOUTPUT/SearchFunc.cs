using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FactorioModeler.FCTORIOOUTPUT
{
    public class SearchFunc
    {
        /// <summary>
        /// Автоматически найти путь к Factorio
        /// C:\Program Files (x86)\Steam\steamapps\common\Factorio
        /// </summary>
        public static string FindFactorioPath()
        {
            string[] possiblePaths = new[]
            {
                // Windows Steam
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Steam", "steamapps", "common", "Factorio"),
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
        /// Автоматически найти путь к FACTORIOMOD
        /// C:\FACTORIOMOD
        /// </summary>
        public static string FindModSourse()
        {
            string[] possiblePaths = new[]
            {
                "C:\\FACTORIOMOD"
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"✓ Найден FACTORIOMOD: {path}");
                    return path;
                }
            }

            throw new DirectoryNotFoundException(
                "Не найена папка FACTORIOMOD. Убедитесь, что FACTORIOMOD установлен или укажите путь вручную.");
        }

        /// <summary>
        /// Автоматически найти путь к Factorio в AppData
        /// C:\Users\{UserName}\AppData\Roaming\Factorio
        /// </summary>
        public static string FindFactorioAppdata()
        {
            string[] possiblePaths = new[]
            {
                // Windows Steam
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "AppData", "Roaming", "Factorio"),
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"✓ Найден Factorio Mod: {path}");
                    return path;
                }
            }

            throw new DirectoryNotFoundException(
                "Не найена папка Factorio Mod. Убедитесь, что Factorio установлен или укажите путь вручную.");
        }

        /// <summary>
        /// Автоматически найти путь к Factorio.exe
        /// C:\Program Files (x86)\Steam\steamapps\common\Factorio\bin\x64\factorio.exe
        /// </summary>
        public static string FindFactorioExePath()
        {
            return FindFactorioPath() + "\\bin\\x64\\factorio.exe";
        }

        /// <summary>
        /// Автоматически найти путь к папке с сейвами
        /// C:\Users\{UserName}\AppData\Roaming\Factorio\saves
        /// </summary>
        public static string FindFactorioSave()
        {
            return FindFactorioAppdata() +"\\saves";
        }

        /// <summary>
        /// Автоматически найти путь к папке с сейвами
        /// C:\Users\{UserName}\AppData\Roaming\Factorio\mods
        /// </summary>
        public static string FindFactorioMods()
        {
            return FindFactorioAppdata() + "\\mods";
        }

        /// <summary>
        /// Автоматически найти путь к папке с сейвами
        /// C:\Users\{UserName}\AppData\Roaming\Factorio\script-output
        /// </summary>
        public static string FindFactorioSrciptOutput()
        {
            return FindFactorioAppdata() + "\\script-output\\exporter";
        }
    }
}
