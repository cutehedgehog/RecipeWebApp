using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;
using RecipeApp.Services.Interfaces;

namespace RecipeApp.Services.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;

        public IngredientService(IIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Ingredient>> GetAllAsync()
        {
            var ingredients = await _repository.GetAllAsync();
            return ingredients.ToList();
        }
        public async Task<Ingredient> GetByIdAsync(int id)
        {
            var ingredient = await _repository.GetByIdAsync(id);
            return ingredient;
        }
        public async Task<Ingredient> GetByNameAsync(string name)
        {
            var ingredient = await _repository.GetByNameAsync(name);
            return ingredient;
        }
    }
}
