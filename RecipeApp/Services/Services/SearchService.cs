using RecipeApp.Services.Interfaces;
using RecipeApp.Repositories.Interfaces;
using RecipeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace RecipeApp.Services.Services
{
    public class SearchService : ISearchService
    {
        private readonly IRecipeRepository _repository;
        public SearchService(IRecipeRepository repository) 
        {
            _repository = repository;
        }
        public async Task<List<Recipe>> GetRecipesByIngredientSubsetAsync(List<string> ingredientNames)
        {
            var recipes = await _repository.GetRecipesByIngredientSubsetAsync(ingredientNames);
            return recipes;
        }
        public async Task<List<Recipe>> GetByCountryAsync(string country)
        {
            var recipes = await _repository.GetByCountryAsync(country);
            return recipes;
        }
    }
}
