using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace RecipeApp.Repositories.Repositories;

public class RecipeRepository : IRecipeRepository
{
    protected readonly AppDbContext _context;

    protected readonly DbSet<Recipe> _dbSet;

    public RecipeRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Recipe>();
    }

    public async Task<List<Recipe>> GetRecipesByIngredientSubsetAsync(List<string> ingredientNames)
    {
        return await _context.Recipes
            .Where(recipe => recipe.RecipeIngredients
                .All(ri => ingredientNames.Contains(ri.Ingredient.Name)))
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .ToListAsync();
    }
    public async Task<Recipe> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();
        return await query.FirstOrDefaultAsync(el => el.Id == id, cancellationToken);
    }

    public async Task<List<Recipe>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        var recipes = await _context.Recipes.Where(r => r.AreaCategory == country).ToListAsync(cancellationToken);
        return recipes;
    }
    public async Task CreateAsync(Recipe entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateWithIngredientsAsync(Recipe entity, List<Ingredient> ingredients, CancellationToken cancellationToken = default)
    {
        foreach (var ingredient in ingredients)
        {
            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredient.Id, cancellationToken);

            if (existingIngredient != null)
            {
                entity.RecipeIngredients.Add(new RecipeIngredient
                {
                    Recipe = entity,
                    Ingredient = existingIngredient
                });
            }
            else
            {
                throw new InvalidOperationException($"Ingredient with ID {ingredient.Id} does not exist.");
            }
        }
        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> UpdateAsync(Recipe entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        var updatedRows = await _context.SaveChangesAsync(cancellationToken);
        return updatedRows > 0;
    }
    public async Task<bool> DeleteAsync(Recipe entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        var deletedRows = await _context.SaveChangesAsync(cancellationToken);
        return deletedRows > 0;
    }
    public async Task<Recipe> FirstOrDefaultAsync(Expression<Func<Recipe, bool>> filters, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filters, cancellationToken);
    }
    public async Task<Recipe> FirstOrDefaultAsync(Expression<Func<Recipe, bool>> filters, CancellationToken cancellationToken = default, params Expression<Func<Recipe, object>>[] includeProperties)
    {
        var query = Include(includeProperties);

        return await query.FirstOrDefaultAsync(filters, cancellationToken);
    }
    public async Task<IEnumerable<Recipe>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Recipe> Include(params Expression<Func<Recipe, object>>[] includeProperties)
    {
        IQueryable<Recipe> query = _dbSet.AsNoTracking();
        return includeProperties
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    public async Task<List<Ingredient>> GetIngredientsByRecipeIdAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        if (recipeId <= 0)
            throw new ArgumentException("Invalid recipe ID", nameof(recipeId));

        return await _context.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId)
            .Select(ri => ri.Ingredient)
            .ToListAsync(cancellationToken);
    }
}
