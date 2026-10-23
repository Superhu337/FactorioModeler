using FactorioModeler.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Models
{
    public interface IEvent { }

    public class CardSelectedEvent : IEvent
    {
        public RecipeCard Card { get; set; }
        public BorderData BorderData { get; set; }
    }

    public class ConnectionCreatedEvent : IEvent
    {
        public ConnectionInfo Connection { get; set; }
    }

    public class DataLoadedEvent : IEvent
    {
        public List<FactorioRecipe> Recipes { get; set; }
    }

    public class RecipeSpawnedEvent : IEvent
    {
        public FactorioRecipe Recipe { get; set; }
    }
}
