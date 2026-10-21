using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FactorioModeler.Scripts
{
    internal class Curve
    {

        private Path CreateCurveBetweenPoints(Point start, Point end)
        {
            // 1. Вычисляем контрольную точку для изгиба.
            // Находим середину по X, чтобы кривая изгибалась плавно горизонтально.
            double controlX = start.X + (end.X - start.X) / 2;

            // Квадратная кривая Безье (одна контрольная точка)
            Point controlPoint = new Point(controlX, start.Y);

            // Если нужна более S-образная линия (как в нодовых редакторах), 
            // можно использовать две контрольные точки (Cubic Bezier), но для дуги хватит и одной.

            // 2. Строим геометрию линии
            PathSegmentCollection segments = new PathSegmentCollection();
            segments.Add(new QuadraticBezierSegment(controlPoint, end, isStroked: true));

            PathFigure figure = new PathFigure(start, segments, false);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            // 3. Создаем сам UI-элемент Path
            Path curvePath = new Path
            {
                Stroke = new SolidColorBrush(Color.FromRgb(0xff, 0xaf, 0xaf)), // Цвет линии
                StrokeThickness = 2, // Толщина линии
                Data = geometry
            };

            return curvePath;
        }
    }
}
