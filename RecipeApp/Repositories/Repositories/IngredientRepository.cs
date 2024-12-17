using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;
using System.Linq.Expressions;


namespace RecipeApp.Repositories.Repositories;

public class IngredientRepository : IIngredientRepository
{
    protected readonly AppDbContext _context;

    protected readonly DbSet<Ingredient> _dbSet;

    protected IngredientRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Ingredient>();
    }
    public async Task<Ingredient> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();
        return await query.FirstOrDefaultAsync(el => el.Id == id, cancellationToken);
    }
    public async Task<Ingredient> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();
        return await query.FirstOrDefaultAsync(el => el.Name == name, cancellationToken);
    }
    public async Task CreateAsync(Ingredient entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> UpdateAsync(Ingredient entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        var updatedRows = await _context.SaveChangesAsync(cancellationToken);
        return updatedRows > 0;
    }
    public async Task<bool> DeleteAsync(Ingredient entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        var deletedRows = await _context.SaveChangesAsync(cancellationToken);
        return deletedRows > 0;
    }
    public async Task<Ingredient> FirstOrDefaultAsync(Expression<Func<Ingredient, bool>> filters, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filters, cancellationToken);
    }
    public async Task<Ingredient> FirstOrDefaultAsync(Expression<Func<Ingredient, bool>> filters, CancellationToken cancellationToken = default, params Expression<Func<Ingredient, object>>[] includeProperties)
    {
        var query = Include(includeProperties);

        return await query.FirstOrDefaultAsync(filters, cancellationToken);
    }
    public async Task<IEnumerable<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Ingredient> Include(params Expression<Func<Ingredient, object>>[] includeProperties)
    {
        IQueryable<Ingredient> query = _dbSet.AsNoTracking();
        return includeProperties
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }
}
