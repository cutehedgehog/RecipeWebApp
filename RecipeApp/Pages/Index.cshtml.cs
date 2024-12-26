using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;
using RecipeApp.Repositories;

namespace RecipeApp.Pages;
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly IIngredientRepository _repository;
    private readonly IRecipeRepository _recipeRepository;
    private const string SelectedItemsSessionKey = "SelectedItems";


    public IndexModel(IIngredientRepository repository, IRecipeRepository recipeRepository)
    {
        _repository = repository;
        _recipeRepository = recipeRepository;
    }

    public List<Ingredient> AutoCompleteItems { get; set; } = new List<Ingredient>();
    [BindProperty]
    public List<string> SelectedItems { get; set; } = new List<string>();

    public async Task<IActionResult> OnGet()
    {
        var sessionData = HttpContext.Session.GetString(SelectedItemsSessionKey);
        SelectedItems = string.IsNullOrEmpty(sessionData)
            ? new List<string>()
            : sessionData.Split(',').ToList();
        AutoCompleteItems = (await _repository.GetAllAsync()).ToList();
        return Page();
    }

    public IActionResult OnPostUpdateSelected(string sItem)
    {
        
        if (!string.IsNullOrEmpty(sItem))
        {
            var sessionData = HttpContext.Session.GetString(SelectedItemsSessionKey);
            SelectedItems = string.IsNullOrEmpty(sessionData)
                ? new List<string>()
                : sessionData.Split(',').ToList();

            if (!SelectedItems.Contains(sItem, StringComparer.OrdinalIgnoreCase))
            {
                SelectedItems.Add(sItem);
                HttpContext.Session.SetString(SelectedItemsSessionKey, string.Join(",", SelectedItems));
            }
        }
        return new OkResult();


    }
    public IActionResult OnPostRemoveSelected(string sItem)
    {
        if (!string.IsNullOrEmpty(sItem))
        {
            var sessionData = HttpContext.Session.GetString(SelectedItemsSessionKey);
            SelectedItems = string.IsNullOrEmpty(sessionData)
                ? new List<string>()
                : sessionData.Split(',').ToList();

            SelectedItems.RemoveAll(item => item.Equals(sItem, StringComparison.OrdinalIgnoreCase));
            HttpContext.Session.SetString(SelectedItemsSessionKey, string.Join(",", SelectedItems));
        }
        return new OkResult();
    }
    public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    public async Task<IActionResult> OnPostGetRecipes([FromBody] IngredientRequest request)
    {
        Recipes = (await _recipeRepository.GetAllAsync()).ToList();
        var ingredients = await _recipeRepository.GetIngredientsByRecipeIdAsync(Recipes[1].Id);
        foreach (var i in ingredients)
        {
            Console.WriteLine(i.Name);
        }
        
        if (request.Ingredients != null && request.Ingredients.Any())
        {
            Recipes = await _recipeRepository.GetRecipesByIngredientSubsetAsync(request.Ingredients);
        }
        

        return new JsonResult(Recipes.Select(r => new { r.Id, r.Title, r.Instructions }));
    }

    public class IngredientRequest
    {
        public List<string> Ingredients { get; set; } = new List<string>();
    }

}