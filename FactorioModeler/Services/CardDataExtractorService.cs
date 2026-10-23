using FactorioModeler.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Services
{
    public class CardDataExtractorService : ICardDataExtractor
    {
        private readonly IDataService _dataService;

        public CardDataExtractorService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public string GetItemName(RecipeCard card, string panelName)
        {
            // Получаем данные БЕЗ привязки к MainWindow или другим окнам
            // Просто работаем с card и panelName
            // Можно переиспользовать в других местах!
        }

        public List<BorderData> GetAllBordersData(RecipeCard card)
        {
            var result = new List<BorderData>();
            // Парсим card и собираем все BorderData
            return result;
        }

        public void ExportCardToJson(RecipeCard card, string filePath)
        {
            var data = GetAllBordersData(card);
            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }
    }
}
