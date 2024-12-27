using RecipeApp.Models;

namespace RecipeApp.Services.Interfaces
{
    public interface IRecipeService
    {
        public Task<Recipe> GetByIdAsync(int id);
        public Task CreateWithIngredientsAsync(Recipe entity, List<Ingredient> ingredients);
        public Task<List<Recipe>> GetAllAsync();
        public Task<List<Ingredient>> GetIngredientsByRecipeIdAsync(int recipeId);
    }
}
