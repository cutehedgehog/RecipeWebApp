using Microsoft.AspNetCore.Mvc;
using RecipeApp.Services.Interfaces;

namespace RecipeApp.Controllers
{
    [Route("api/ingredient")]
    [ApiController]
    public class IngredientController: Controller
    {

        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _ingredientService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();
            
            return Ok(ingredient);
        }

        [HttpGet("name/{name}")]
       
        public async Task<IActionResult> GetByName(string name)
        {
            var ingredient = await _ingredientService.GetByNameAsync(name);
            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }
    }
}

