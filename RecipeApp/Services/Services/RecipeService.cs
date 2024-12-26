using RecipeApp.Models;
using RecipeApp.Services.Interfaces;
using RecipeApp.Repositories.Interfaces;

namespace RecipeApp.Services.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repository;
        public RecipeService(IRecipeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Recipe> GetByIdAsync(int id)
        {
            var recipe = await _repository.GetByIdAsync(id);
            return recipe;
        }
        public async Task CreateWithIngredientsAsync(Recipe entity, List<Ingredient> ingredients)
        {
            await _repository.CreateWithIngredientsAsync(entity, ingredients);
        }
        public async Task<List<Recipe>> GetAllAsync()
        {
            var recipes = await _repository.GetAllAsync();
            return recipes.ToList();
        }
        public async Task<List<Ingredient>> GetIngredientsByRecipeIdAsync(int recipeId)
        {
            var ingredients = await _repository.GetIngredientsByRecipeIdAsync(recipeId);
            return ingredients.ToList();
        }
    }
}
