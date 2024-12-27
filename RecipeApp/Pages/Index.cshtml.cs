using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;
using System.ComponentModel;

namespace RecipeApp.Pages;
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IRecipeRepository _recipeRepository;
    private const string SelectedItemsSessionKey = "SelectedItems";

    public IndexModel(IIngredientRepository ringredientRepository, IRecipeRepository recipeRepository)
    {
        _ingredientRepository = ringredientRepository;
        _recipeRepository = recipeRepository;
    }

    public List<Ingredient> AutoCompleteItems { get; set; } = new List<Ingredient>();
    public List<string> Countries { get; set; } = new List<string>();
    [BindProperty]
    public List<string> StockSelectedItems { get; set; } = new List<string>();
    public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    public async Task<IActionResult> OnGet()
    {
        StockSelectedItems = this.GetListFromSession(SelectedItemsSessionKey);
        AutoCompleteItems = (await _ingredientRepository.GetAllAsync()).ToList();
        var recipes = (await _recipeRepository.GetAllAsync()).ToList();
        Countries.Add("All");
        foreach (var recipe in recipes)
        {
            if (!Countries.Contains(recipe.AreaCategory))
            {
                Countries.Add(recipe.AreaCategory);
            }
        }
        return Page();
    }

    public IActionResult OnPostUpdateSelected(string sItem)
    {

        if (!string.IsNullOrEmpty(sItem))
        {
            StockSelectedItems = this.GetListFromSession(SelectedItemsSessionKey);

            if (!StockSelectedItems.Contains(sItem, StringComparer.OrdinalIgnoreCase))
            {
                StockSelectedItems.Add(sItem);
                this.AddListToSession(SelectedItemsSessionKey, StockSelectedItems);
            }
        }
        return new OkResult();

    }
    public IActionResult OnPostRemoveSelectedFromStock(string sItem)
    {
        if (!string.IsNullOrEmpty(sItem))
        {
            StockSelectedItems = this.GetListFromSession(SelectedItemsSessionKey);
            StockSelectedItems.RemoveAll(item => item.Equals(sItem, StringComparison.OrdinalIgnoreCase));
            this.AddListToSession(SelectedItemsSessionKey, StockSelectedItems);
        }
        return new OkResult();
    }

    public List<string> GetListFromSession(string key)
    {
        var sessionData = HttpContext.Session.GetString(key);
        var list = string.IsNullOrEmpty(sessionData)
               ? new List<string>()
               : sessionData.Split(',').ToList();
        return list;
    }

    public void AddListToSession(string key, List<string> list)
    {
        HttpContext.Session.SetString(key, string.Join(",", list));
    }
    public async Task<IActionResult> OnPostGetRecipes()
    {
        StockSelectedItems = this.GetListFromSession(SelectedItemsSessionKey);
        if ( StockSelectedItems!= null && StockSelectedItems.Any())
        {
            Recipes = await _recipeRepository.GetRecipesByIngredientSubsetAsync(StockSelectedItems);
        }
        return new JsonResult(Recipes.Select(r => new { r.Id, r.Title, r.Instructions }));
    }
    public async Task<IActionResult> OnPostFilterRecipesAsync(string country)
    {
        StockSelectedItems = this.GetListFromSession(SelectedItemsSessionKey);
        if (StockSelectedItems != null && StockSelectedItems.Any())
        {
            Recipes = await _recipeRepository.GetRecipesByIngredientSubsetAsync(StockSelectedItems);
        }
        if (!country.Equals("all"))
        {
            Recipes = Recipes.Where(e => e.AreaCategory.ToLower() == country).ToList();
        }
       
        return new JsonResult(Recipes.Select(r => new { r.Id, r.Title, r.Instructions }));
    }

    public class IngredientRequest
    {
        public List<string> Ingredients { get; set; } = new List<string>();
    }

    public class RecipeRequest
    {
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }

}