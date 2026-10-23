using FactorioModeler.Models;
using FactorioModeler.Services;
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
        private readonly IEventBus _eventBus;  // НОВОЕ!

        public CurveBuilder(Canvas canvas, IEventBus eventBus)
        {
            _canvas = canvas;
            _eventBus = eventBus;
        }

        public bool StopDrawing(out ConnectionInfo connection)
        {
            // ... существующий код ...

            if (connection != null)
            {
                // ПУБЛИКУЕМ событие вместо вызова MainWindow.CreatePermanentConnection()
                _eventBus.Publish(new ConnectionCreatedEvent { Connection = connection });
            }

            return true;
        }
    }
}
