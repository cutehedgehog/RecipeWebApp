using Microsoft.AspNetCore.Mvc;
using RecipeApp.Services.Interfaces;

namespace RecipeApp.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("recipes-by-ingredients")]
        public async Task<IActionResult> GetRecipesByIngredientSubset([FromBody] List<string> ingredientNames)
        {
            if (ingredientNames == null || !ingredientNames.Any())
                return BadRequest("Ingredient names list cannot be null or empty.");

            return Ok(await _searchService.GetRecipesByIngredientSubsetAsync(ingredientNames));
        }
    }
}

