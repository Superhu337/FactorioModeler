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
        private RecipeCard _endCard;
        private Border _clickedBorder;

        // Конструктор принимает холст, на котором будет идти рисование
        public CurveBuilder(Canvas canvas)
        {
            _canvas = canvas;
        }

        /// <summary>
        /// Найти предка конкретного типа в визуальном дереве
        /// </summary>
        private T FindAncestor<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T ancestor)
                    return ancestor;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Начало рисования кривой от карточки
        /// </summary>
        public void StartDrawing(RecipeCard card, Point pointInCard, Color lineColor, string panelName, Border clickedBorder = null)
        {
            _activeCard = card;
            _startPanelName = panelName;
            _clickedBorder = clickedBorder;     // Сохраняем Border с информацией о предмете
            _endCard = null;

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

            DetectHoveredCard(currentMousePoint);

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
            Point Z;

            if (_startPanelName == "OutputsPanel")
            {
                // Если тянем из ВЫХОДА: первая точка уходит НАПРАВО (+offset), 
                // а к курсору мыши линия подходит СЛЕВА (-offset)
                controlPoint1 = new Point(_startPoint.X + offset, _startPoint.Y);
                controlPoint2 = new Point(currentMousePoint.X - offset, currentMousePoint.Y);

                Z = new Point(16, 0);

            }
            else // InputsPanel
            {
                // Если тянем из ВХОДА: первая точка уходит НАЛЕВО (-offset), 
                // а к курсору мыши линия подходит СВЕРХУ/СНИЗУ или СПРАВА (+offset)
                controlPoint1 = new Point(_startPoint.X - offset, _startPoint.Y);
                controlPoint2 = new Point(currentMousePoint.X + offset, currentMousePoint.Y);

                Z = new Point(-16, 0);
            }


            // Формируем геометрию кривой
            PathSegmentCollection segments = new PathSegmentCollection
            {
                new BezierSegment(controlPoint1 , controlPoint2, currentMousePoint, true)
            };

            PathFigure figure = new PathFigure(new Point(_startPoint.X + Z.X, _startPoint.Y + Z.Y), segments, false);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            // Обновляем Data у активной линии на лету
            _activeCurve.Data = geometry;
            DetectHoveredCard(currentMousePoint);

        }
        /// <summary>
        /// Детектировать, над какой карточкой находится мышка
        /// </summary>
        private void DetectHoveredCard(Point mousePos)
        {
            // Hit test — найти элемент под курсором
            var hitTest = VisualTreeHelper.HitTest(_canvas, mousePos);

            if (hitTest?.VisualHit is FrameworkElement element)
            {
                // Ищем RecipeCard в иерархии предков
                var card = FindAncestor<RecipeCard>(element);
                _endCard = card;
            }
            else
            {
                _endCard = null;
            }
        }

        /// <summary>
        /// Остановка рисования и удаление временной линии
        /// </summary>
        public bool StopDrawing(out ConnectionInfo connection)
        {
            connection = null;
            // Если активной кривой нет, значит рисование и не начиналось — возвращаем false
            if (_activeCurve == null) return false;

            // Освобождаем захват мыши карточкой
            _activeCard?.ReleaseMouseCapture();

            // Удаляем временную "тянущуюся" линию с холста, чтобы она пропала
            _canvas.Children.Remove(_activeCurve);


            // Если указали на ДРУГУЮ карточку — создаем информацию о соединении
            if (_endCard != null && _endCard != _activeCard && _clickedBorder != null)
            {
                string itemName = ExtractItemNameFromBorder(_clickedBorder);

                connection = new ConnectionInfo
                {
                    FromCard = _activeCard,
                    ToCard = _endCard,
                    ItemName = itemName,
                    FromPanel = _startPanelName
                };
            }

            // Сбрасываем внутреннее состояние
            _activeCurve = null;
            _activeCard = null;
            _startPanelName = null;
            _endCard = null;
            _clickedBorder = null;

            // Возвращаем true — сигнал для MainWindow, что пора открывать SearchPopUp
            return true;
        }

        /// <summary>
        /// Извлечь название предмета из Border.Tag
        /// </summary>
        private string ExtractItemNameFromBorder(Border border)
        {
            if (border?.Tag is BorderData tag)
            {
                try
                {
                    return tag.ItemName ?? "";
                }
                catch { }
            }
            return "";
        }

    }
}
