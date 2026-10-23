using FactorioModeler.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Services
{
    public interface ICardDataExtractor
    {
        string GetItemName(RecipeCard card, string panelName);
        double GetItemAmount(RecipeCard card, string panelName);
        List<BorderData> GetAllBordersData(RecipeCard card);
        void ExportCardToJson(RecipeCard card, string filePath);
    }
}
