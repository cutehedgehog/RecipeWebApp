using RecipeApp.Models;
using System.Linq.Expressions;

namespace RecipeApp.Repositories.Interfaces;
public interface IRecipeRepository
{
    public Task<Recipe> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<List<Recipe>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
    public Task CreateAsync(Recipe entity, CancellationToken cancellationToken = default);
    public Task CreateWithIngredientsAsync(Recipe entity, List<Ingredient> ingredients, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(Recipe entity, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(Recipe entity, CancellationToken cancellationToken = default);
    public Task<Recipe> FirstOrDefaultAsync(Expression<Func<Recipe, bool>> filters, CancellationToken cancellationToken = default);
    public Task<Recipe> FirstOrDefaultAsync(Expression<Func<Recipe, bool>> filters, CancellationToken cancellationToken = default, params Expression<Func<Recipe, object>>[] includeProperties);
    public Task<IEnumerable<Recipe>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public Task<List<Recipe>> GetRecipesByIngredientSubsetAsync(List<string> ingredientNames);
    public Task<List<Ingredient>> GetIngredientsByRecipeIdAsync(int recipeId, CancellationToken cancellationToken = default);
}
