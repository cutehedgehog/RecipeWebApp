using Microsoft.AspNetCore.Mvc;
using RecipeApp.Models;
using RecipeApp.Services.Interfaces;

namespace RecipeApp.Controllers
{
    [Route("api/recipe")]
    [ApiController]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound();

            return Ok(recipe);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWithIngredients([FromBody] RecipeWithIngredientsRequest request)
        {
            await _recipeService.CreateWithIngredientsAsync(request.Recipe, request.Ingredients);
            return CreatedAtAction(nameof(GetById), new { id = request.Recipe.Id }, request.Recipe);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _recipeService.GetAllAsync());
        }

        [HttpGet("{recipeId}/ingredients")]
        public async Task<IActionResult> GetIngredientsByRecipeId(int recipeId)
        {
            var ingredients = await _recipeService.GetIngredientsByRecipeIdAsync(recipeId);
            if (ingredients == null || !ingredients.Any())
                return NotFound();
            return Ok(ingredients);
        }

        public class RecipeWithIngredientsRequest
        {
            public Recipe Recipe { get; set; }
            public List<Ingredient> Ingredients { get; set; }
        }
    }
}

