﻿using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace RecipeApp.Repositories.Repositories;

public class RecipeRepository: IRecipeRepository
{
    protected readonly AppDbContext _context;

    protected readonly DbSet<Recipe> _dbSet;

    protected RecipeRepository(AppDbContext context)
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
    public async Task CreateAsync(Recipe entity, CancellationToken cancellationToken = default)
    {
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
}
