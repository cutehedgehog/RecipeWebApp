using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Models;
using RecipeApp.Repositories.Repositories;
using System.Data.Common;


namespace RecipeApp.Tests
{
    public class RecipeRepositoryTest
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        public RecipeRepositoryTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();

            var ingredients = new List<Ingredient>
            {
                new Ingredient { Id = 1, Name = "apple" },
                new Ingredient { Id = 2, Name = "tomato" },
                new Ingredient { Id = 3, Name = "cucumber" },
                new Ingredient { Id = 4, Name = "salt" },
                new Ingredient { Id = 5, Name = "sugar" }
            };
            context.Ingredients.AddRange(ingredients);

            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Apple Salad",
                    Category = "Salad",
                    AreaCategory = "American",
                    Instructions = "Chop the apples and cucumbers, mix with salt.",
                    ImageUrl = "https://example.com/apple-salad.jpg",
                    VideoSourceUrl = "https://example.com/apple-salad-video.mp4"
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Tomato Soup",
                    Category = "Soup",
                    AreaCategory = "Italian",
                    Instructions = "Blend tomatoes, add salt, and simmer.",
                    ImageUrl = "https://example.com/tomato-soup.jpg",
                    VideoSourceUrl = "https://example.com/tomato-soup-video.mp4"
                },
                new Recipe
                {
                    Id = 3,
                    Title = "Cucumber Sandwich",
                    Category = "Snack",
                    AreaCategory = "British",
                    Instructions = "Slice cucumbers and place between slices of bread with sugar.",
                    ImageUrl = "https://example.com/cucumber-sandwich.jpg",
                    VideoSourceUrl = "https://example.com/cucumber-sandwich-video.mp4"
                }
            };
            context.Recipes.AddRange(recipes);

            var recipeIngredients = new List<RecipeIngredient>
            {
                new RecipeIngredient { RecipeId = 1, IngredientId = 1 }, 
                new RecipeIngredient { RecipeId = 1, IngredientId = 3 }, 
                new RecipeIngredient { RecipeId = 1, IngredientId = 4 }, 

                new RecipeIngredient { RecipeId = 2, IngredientId = 2 }, 
                new RecipeIngredient { RecipeId = 2, IngredientId = 4 }, 

                new RecipeIngredient { RecipeId = 3, IngredientId = 3 }, 
                new RecipeIngredient { RecipeId = 3, IngredientId = 5 }  
            };
            context.RecipeIngredients.AddRange(recipeIngredients);
            context.SaveChanges();
        }

        public AppDbContext CreateContext() => new(_contextOptions);
        public void Dispose() => _connection.Dispose();


        [Fact]
        public async Task GetRecipesByIngredientSubsetAsync_ShouldReturnRecipes_WhenIngredientsMatchSubset_1()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context); 
            var ingredientNames = new List<string> { "apple","cucumber", "tomato", "salt"};


            var result = await repository.GetRecipesByIngredientSubsetAsync(ingredientNames);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count); 

            var salad = result.First(r => r.Title == "Apple Salad");
            Assert.Equal(3, salad.RecipeIngredients.Count); 

            var soup = result.First(r => r.Title == "Tomato Soup");
            Assert.Equal(2, soup.RecipeIngredients.Count); 
        }

        [Fact]
        public async Task GetRecipesByIngredientSubsetAsync_ShouldReturnRecipes_WhenIngredientsMatchSubset_2()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);
            var ingredientNames = new List<string> { "apple", "cucumber" };


            var result = await repository.GetRecipesByIngredientSubsetAsync(ingredientNames);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecipesByIngredientSubsetAsync_ShouldReturnRecipes_WhenIngredientsMatchSubset_3()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);
            var ingredientNames = new List<string> { "apple", "cucumber", "salt" };


            var result = await repository.GetRecipesByIngredientSubsetAsync(ingredientNames);

            Assert.NotNull(result);
            Assert.Single(result);

            var salad = result.First(r => r.Title == "Apple Salad");
            Assert.Equal(3, salad.RecipeIngredients.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRecipe_WhenRecipeExists()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var result = await repository.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Apple Salad", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRecipeDoesNotExist()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var result = await repository.GetByIdAsync(26);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddRecipe_WhenRecipeIsValid()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var recipe = new Recipe
            {
                Title = "New Recipe",
                Category = "Appetizer",
                AreaCategory = "Italian",
                Instructions = "Prepare and serve.",
                ImageUrl = "https://example.com/new-recipe.jpg",
                VideoSourceUrl = "https://example.com/new-recipe-video.mp4"
            };

            await repository.CreateAsync(recipe);
            var result = await repository.GetByIdAsync(recipe.Id);

            Assert.NotNull(result);
            Assert.Equal(recipe.Title, result.Title);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateRecipe_WhenRecipeExists()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var recipe = new Recipe
            {
                Id = 2,
                Title = "Tomato Soup New",
                Category = "Soup",
                AreaCategory = "Italian",
                Instructions = "Blend tomatoes, add salt, and simmer.",
                ImageUrl = "https://example.com/tomato-soup.jpg",
                VideoSourceUrl = "https://example.com/tomato-soup-video.mp4"
            };

            var result = await repository.UpdateAsync(recipe);

            Assert.True(result);
            var updatedRecipe = await repository.GetByIdAsync(recipe.Id);
            Assert.Equal("Tomato Soup New", updatedRecipe.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteRecipe_WhenRecipeExists()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var recipe = new Recipe
            {
                Id = 3,
                Title = "Cucumber Sandwich",
                Category = "Snack",
                AreaCategory = "British",
                Instructions = "Slice cucumbers and place between slices of bread with sugar.",
                ImageUrl = "https://example.com/cucumber-sandwich.jpg",
                VideoSourceUrl = "https://example.com/cucumber-sandwich-video.mp4"
            };

            var result = await repository.DeleteAsync(recipe);

            Assert.True(result);
            var deletedRecipe = await repository.GetByIdAsync(recipe.Id);
            Assert.Null(deletedRecipe);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnRecipe_WhenRecipeMatchesFilter()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var result = await repository.FirstOrDefaultAsync(r => r.Title == "Cucumber Sandwich");

            Assert.NotNull(result);
            Assert.Equal("Cucumber Sandwich", result.Title);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRecipes()
        {
            using var context = CreateContext();
            var repository = new RecipeRepository(context);

            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

    }
}
