using FactorioModeler.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FactorioModeler.Scripts
{
    public class CurveBuilder
    {
        private readonly Canvas _canvas;
        private Path _activeCurve;
        private Point _startPoint;
        private RecipeCard _activeCard;
        private string _startPanelName;
        // Конструктор принимает холст, на котором будет идти рисование
        public CurveBuilder(Canvas canvas)
        {
            _canvas = canvas;
        }

        /// <summary>
        /// Начало рисования кривой от карточки
        /// </summary>
        public void StartDrawing(RecipeCard card, Point pointInCard, Color lineColor, string panelName)
        {
            _activeCard = card;
            _startPanelName = panelName;

            // Переводим локальные координаты карточки в координаты холста (они в одном дереве!)
            GeneralTransform transform = card.TransformToVisual(_canvas);
            _startPoint = transform.Transform(pointInCard);

            // Создаем саму линию Path
            _activeCurve = new Path
            {
                Stroke = new SolidColorBrush(lineColor),
                StrokeThickness = 2,
                Data = new PathGeometry()
            };

            // Добавляем линию на холст
            _canvas.Children.Add(_activeCurve);
        }

        /// <summary>
        /// Обновление конечной точки кривой по координатам мыши
        /// </summary>
        public void UpdateDrawing(Point currentMousePoint)
        {
            if (_activeCurve == null) return;

            // Расстояние по горизонтали между стартом и концом линии
            double deltaX = currentMousePoint.X - _startPoint.X;

            // Вычисляем силу изгиба (коэффициент 0.5 дает идеальный S-образный баланс)
            double offset = deltaX * 0.5;

            // Ограничиваем минимальный изгиб, чтобы линия красиво выходила, 
            // даже если мышка находится левее стартовой точки
            if (deltaX < 0) { offset = Math.Max(30, Math.Abs(deltaX) * 0.5); }
            else{ offset = Math.Max(30, offset); }

            Point controlPoint1;
            Point controlPoint2;

            if (_startPanelName == "OutputsPanel")
            {
                // Если тянем из ВЫХОДА: первая точка уходит НАПРАВО (+offset), 
                // а к курсору мыши линия подходит СЛЕВА (-offset)
                controlPoint1 = new Point(_startPoint.X + offset, _startPoint.Y);
                controlPoint2 = new Point(currentMousePoint.X - offset, currentMousePoint.Y);
            }
            else // InputsPanel
            {
                // Если тянем из ВХОДА: первая точка уходит НАЛЕВО (-offset), 
                // а к курсору мыши линия подходит СВЕРХУ/СНИЗУ или СПРАВА (+offset)
                controlPoint1 = new Point(_startPoint.X - offset, _startPoint.Y);
                controlPoint2 = new Point(currentMousePoint.X + offset, currentMousePoint.Y);
            }




            // Формируем геометрию кривой
            PathSegmentCollection segments = new PathSegmentCollection
            {
                new BezierSegment(controlPoint1 , controlPoint2, currentMousePoint, true)
            };

            PathFigure figure = new PathFigure(_startPoint, segments, false);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            // Обновляем Data у активной линии на лету
            _activeCurve.Data = geometry;
        }

        /// <summary>
        /// Остановка рисования и удаление временной линии
        /// </summary>
        public void StopDrawing()
        {
            if (_activeCurve == null) return;

            // Освобождаем захват мыши карточкой
            _activeCard?.ReleaseMouseCapture();

            // Удаляем линию с холста
            _canvas.Children.Remove(_activeCurve);

            // Сбрасываем состояние
            _activeCurve = null;
            _activeCard = null;
        }
    }
}
