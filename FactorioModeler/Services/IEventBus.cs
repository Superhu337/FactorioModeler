using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioModeler.Models;

namespace FactorioModeler.Services
{
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
