using RecipeApp.Models;
using System.Linq.Expressions;

namespace RecipeApp.Repositories.Interfaces;

public interface IIngredientRepository
{
    public Task<Ingredient> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<Ingredient> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    public Task CreateAsync(Ingredient entity, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(Ingredient entity, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(Ingredient entity, CancellationToken cancellationToken = default);
    public Task<Ingredient> FirstOrDefaultAsync(Expression<Func<Ingredient, bool>> filters, CancellationToken cancellationToken = default);
    public Task<Ingredient> FirstOrDefaultAsync(Expression<Func<Ingredient, bool>> filters, CancellationToken cancellationToken = default, params Expression<Func<Ingredient, object>>[] includeProperties);
    public Task<IEnumerable<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
