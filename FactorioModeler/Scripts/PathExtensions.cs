using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FactorioModeler.Scripts
{

    public static class PathExtensions
    {
        /// <summary>
        /// Возвращает относительный путь от одного пути к другому (аналог Path.GetRelativePath для .NET Framework)
        /// </summary>
        public static string GetRelativePath(string relativeTo, string path)
        {
            if (string.IsNullOrEmpty(relativeTo)) throw new ArgumentNullException(nameof(relativeTo));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            // Приводим пути к абсолютному и каноничному виду (удаляем ".." и ".")
            string fromPath = Path.GetFullPath(relativeTo);
            string toPath = Path.GetFullPath(path);

            // Для Uri папка обязательно должна заканчиваться на слэш
            if (!fromPath.EndsWith(Path.DirectorySeparatorChar.ToString()) && !fromPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                fromPath += Path.DirectorySeparatorChar;
            }

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            // Если у путей разные диски (например, C:\ и D:\), MakeRelativeUri вернет абсолютный путь
            if (fromUri.Scheme != toUri.Scheme || fromUri.Host != toUri.Host)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            // Заменяем веб-слэши (/) на системные (\)
            return relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }

}
