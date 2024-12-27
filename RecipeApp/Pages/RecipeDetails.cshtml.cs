using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;

namespace RecipeApp.Pages
{
    public class RecipeDetails : PageModel
    {
        private readonly IRecipeRepository _recipeRepository;
        public Recipe Recipe { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public RecipeDetails(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        public async Task<IActionResult> OnGetAsync(int recipeId)
        {
            Recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (Recipe == null)
            {
                return NotFound($"Recipe with ID {recipeId} not found.");
            }
            Ingredients = await _recipeRepository.GetIngredientsByRecipeIdAsync(recipeId);
            return Page();
        }
    }
}
