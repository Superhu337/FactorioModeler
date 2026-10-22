using FactorioModeler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Services
{
    public interface IDataService
    {
        List<FactorioRecipe> GetRecipes();
        FactorioItem GetItemByName(string name);
        FactorioRecipe GetRecipeByName(string name);
        FactorioMachine GetMachineByName(string name);
        FactorioTechnology GetTechnologyByName(string name);
        List<FactorioRecipe> GetRecipesProducingByProducts(string Products);
        List<FactorioMachine> GetMachinesForRecipeByRecipeName(string recipeName);
        List<string> GetTechnologyDependencyChainByTechName(string techName);
        List<FactorioRecipe> GetRecipesByCategoryByCategory(string category);
        string GetRecipeIcon(string recipeName);
        void StartDataCollection();
    }
}
