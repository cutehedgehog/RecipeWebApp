using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using System.Text.Json;

namespace RecipeApp.Data;

public class DbInitializer
{
    public static async Task SeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
        if (!context.Database.GetAppliedMigrations().Any())
        {
            await context.Database.MigrateAsync(); 
        }
        var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
        if (context.Recipes.Any())
        {
            Console.WriteLine("Database already initialized.");
            return;
        }

        Console.WriteLine("Database is empty. Fetching data from TheMealDB...");
        await FetchRecipesFromApiAsync(httpClient, context);
        Console.WriteLine("Database initialized successfully.");
    }

    private static async Task FetchRecipesFromApiAsync(HttpClient httpClient, AppDbContext context)
    {

        var responseIngredients = await httpClient.GetStringAsync("https://www.themealdb.com/api/json/v1/1/list.php?i=list");
        var jsonIngredientsDocument = JsonDocument.Parse(responseIngredients);

        foreach (var meal in jsonIngredientsDocument.RootElement.GetProperty("meals").EnumerateArray())
        {
            var newIngredient = new Ingredient { Name = meal.GetProperty("strIngredient").GetString().ToLower() };
            context.Ingredients.Add(newIngredient);
            await context.SaveChangesAsync();
        }

        for (int j = 0; j < 40; j++)
        {
            var responseRecipes = await httpClient.GetStringAsync("https://www.themealdb.com/api/json/v1/1/random.php");
            var jsonRecipesDocument = JsonDocument.Parse(responseRecipes);

            foreach (var meal in jsonRecipesDocument.RootElement.GetProperty("meals").EnumerateArray())
            {
                var recipe = new Recipe
                {
                    Title = meal.GetProperty("strMeal").GetString(),
                    Category = meal.GetProperty("strCategory").GetString(),
                    AreaCategory = meal.GetProperty("strArea").GetString(),
                    Instructions = meal.GetProperty("strInstructions").GetString(),
                    ImageUrl = meal.GetProperty("strMealThumb").GetString(),
                    VideoSourceUrl = meal.GetProperty("strYoutube").GetString(),
                };

                context.Recipes.Add(recipe);
                await context.SaveChangesAsync();
                int lastAddedRecipeId = recipe.Id;

                var ingredients = new List<RecipeIngredient>();

                for (int i = 1; i <= 20; i++)
                {
                    var ingredientKey = $"strIngredient{i}";
                    if (meal.TryGetProperty(ingredientKey, out var ingredient) && !string.IsNullOrEmpty(ingredient.GetString()))
                    {
                        var ingredientName = ingredient.GetString().ToLower();
                        var existingIngredient = await context.Ingredients
                            .FirstOrDefaultAsync(i => i.Name.ToLower() == ingredientName);

                        if (existingIngredient != null)
                        {
                            if (!ingredients.Any(ri => ri.RecipeId == lastAddedRecipeId && ri.IngredientId == existingIngredient.Id))
                            {
                                ingredients.Add(new RecipeIngredient
                                {
                                    RecipeId = lastAddedRecipeId,
                                    IngredientId = existingIngredient.Id,
                                });
                            }
                        }
                    }
                }
                context.RecipeIngredients.AddRange(ingredients);
                await context.SaveChangesAsync();
            }
        }
    }
}