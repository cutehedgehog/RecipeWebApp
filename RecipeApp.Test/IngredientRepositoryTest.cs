using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;
using RecipeApp.Repositories.Repositories;
using System.Data.Common;
using System.Linq.Expressions;

namespace RecipeApp.Tests
{
    public class IngredientRepositoryTest
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public IngredientRepositoryTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
            context.Ingredients.AddRange(
                new Ingredient { Id = 1, Name = "apple" },
                new Ingredient { Id = 2, Name = "tomato" },
                new Ingredient { Id = 3, Name = "cucumber" });
            context.SaveChanges();

        }
        public AppDbContext CreateContext() => new(_contextOptions);
        public void Dispose() => _connection.Dispose();

        [Fact]
        public async Task GetByIdAsync_ShouldReturnIngredient_WhenIngredientExists()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            var result = await repository.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("apple", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIngredientDoesNotExist()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);
            var result = await repository.GetByIdAsync(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddIngredient()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);
            var ingredient = new Ingredient { Id = 4, Name = "salt" };

            await repository.CreateAsync(ingredient);
            var result = await repository.GetByIdAsync(4);

            Assert.NotNull(result);
            Assert.Equal("salt", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateIngredient()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            var ingredient = new Ingredient { Id = 3, Name = "pepper" };

            var isUpdated = await repository.UpdateAsync(ingredient);
            var updatedIngredient = await repository.GetByIdAsync(3);

            Assert.True(isUpdated);
            Assert.Equal("pepper", updatedIngredient.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveIngredient()
        {
            var ingredient = new Ingredient { Id = 2, Name = "tomato" };

            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            var isDeleted = await repository.DeleteAsync(ingredient);
            var result = await repository.GetByIdAsync(2);

            Assert.True(isDeleted);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnIngredient_WhenIngredientExists()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            var result = await repository.GetByNameAsync("apple");

            Assert.NotNull(result);
            Assert.Equal("apple", result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenIngredientDoesNotExist()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);
            var result = await repository.GetByNameAsync("garlic");
            Assert.Null(result);
        }

       [Fact]
        public async Task GetAllAsync_ShouldReturnAllIngredients()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnIngredient_WhenMatchingName()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            Expression<Func<Ingredient, bool>> filter = ingredient => ingredient.Name == "apple";

            var result = await repository.FirstOrDefaultAsync(filter);

            Assert.NotNull(result); 
            Assert.Equal("apple", result.Name);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnNull_WhenNoMatchingName()
        {
            using var context = CreateContext();
            var repository = new IngredientRepository(context);

            Expression<Func<Ingredient, bool>> filter = ingredient => ingredient.Name == "banana";

            var result = await repository.FirstOrDefaultAsync(filter);

            Assert.Null(result); // Результат должен быть null, так как ингредиент не найден
        }
    }
}