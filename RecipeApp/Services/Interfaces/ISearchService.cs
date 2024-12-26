using RecipeApp.Models;

namespace RecipeApp.Services.Interfaces
{
    public interface ISearchService
    {
        public Task<List<Recipe>> GetRecipesByIngredientSubsetAsync(List<string> ingredientNames);
        public Task<List<Recipe>> GetByCountryAsync(string country);
    }
}
