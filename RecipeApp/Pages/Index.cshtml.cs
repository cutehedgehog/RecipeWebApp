using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;

namespace RecipeApp.Pages;
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly IIngredientRepository _repository;
    private const string SelectedItemsSessionKey = "SelectedItems";


    public IndexModel(IIngredientRepository repository)
    {
        _repository = repository;
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
}