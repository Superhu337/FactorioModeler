using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Models
{
    internal class RecipeCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RecipeIngredient Ingredient { get; set; }
        public RecipeProduct Product { get; set; }
        public ConnectionInfo Conection { get; set; }
    }
}
