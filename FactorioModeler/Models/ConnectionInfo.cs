using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Models
{
    internal class ConnectionInfo
    {
        public RecipeCard FromCard { get; set; }
        public RecipeCard ToCard { get; set; }
        public string ItemName { get; set; }
        public string FromBorder { get; set; }
        public string ToBorder { get; set; }
    }
}
