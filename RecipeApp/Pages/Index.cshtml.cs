using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;

namespace RecipeApp.Pages;
public class IndexModel : PageModel
{
    private readonly IIngredientRepository _repository;

    public IndexModel(IIngredientRepository repository)
    {
        _repository = repository;
    }

    public List<Ingredient> AutoCompleteItems { get; set; } = new List<Ingredient>();
    [BindProperty]
    public string SearchTerm { get; set; } = string.Empty;
    [BindProperty]
    public List<string> SelectedItems { get; set; } = new List<string>();

    public async Task<IActionResult> OnGet()
    {
        AutoCompleteItems = (await _repository.GetAllAsync()).ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!string.IsNullOrEmpty(SearchTerm) && AutoCompleteItems.Select(x => x.Name).Contains(SearchTerm))
        {
            SelectedItems.Add(SearchTerm);
        }

        return Page();
    }
}