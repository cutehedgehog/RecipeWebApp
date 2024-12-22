using RecipeApp.Models;

namespace RecipeApp.Services.Interfaces
{
    public interface IIngredientService
    {
        public Task<List<Ingredient>> GetAllAsync();
        public Task<Ingredient> GetByIdAsync(int id);
        public Task<Ingredient> GetByNameAsync(string name);
    }
}
