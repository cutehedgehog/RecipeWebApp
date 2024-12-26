using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Repositories.Interfaces;

namespace RecipeApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class CreateRecipeModel : PageModel
    {
        private const string AddItemsSessionKey = "AddItems";
        private const string TitleSessionKey = "Title";
        private const string AreaCategorySessionKey = "AreaCategory";
        private const string InstructionsSessionKey = "Instruction";

        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public CreateRecipeModel(IIngredientRepository ringredientRepository, IRecipeRepository recipeRepository)
        {
            _ingredientRepository = ringredientRepository;
            _recipeRepository = recipeRepository;
        }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string AreaCategory { get; set; }

        [BindProperty]
        public string Instructions { get; set; }

        public List<Ingredient> AutoCompleteItems { get; set; } = new List<Ingredient>();
        public List<string> SelectedItems { get; set; } = new();


        public async Task<IActionResult> OnGet()
        {
            SelectedItems = this.GetListFromSession(AddItemsSessionKey);
            Title = this.GetStringFromSession(TitleSessionKey);
            AreaCategory = this.GetStringFromSession(AreaCategorySessionKey);
            Instructions = this.GetStringFromSession(InstructionsSessionKey);
            AutoCompleteItems = (await _ingredientRepository.GetAllAsync()).ToList();
            return Page();
        }

        public IActionResult OnPostUpdateSelected(string sItem)
        {
            if (!string.IsNullOrEmpty(sItem))
            {
                SelectedItems = this.GetListFromSession(AddItemsSessionKey);

                if (!SelectedItems.Contains(sItem, StringComparer.OrdinalIgnoreCase))
                {
                    SelectedItems.Add(sItem);
                    this.AddListToSession(AddItemsSessionKey, SelectedItems);
                }
            }
            return new OkResult();
        }

        public IActionResult OnPostRemoveFromSelected(string sItem)
        {
            if (!string.IsNullOrEmpty(sItem))
            {
                SelectedItems = this.GetListFromSession(AddItemsSessionKey);
                SelectedItems.RemoveAll(item => item.Equals(sItem, StringComparison.OrdinalIgnoreCase));
                this.AddListToSession(AddItemsSessionKey, SelectedItems);
            }
            return new OkResult();
        }

        public async Task<IActionResult> OnPostCreate()
        {
            SelectedItems = this.GetListFromSession(AddItemsSessionKey);
            Recipe recipe = new Recipe
            {
                Title = this.Title,
                AreaCategory = this.AreaCategory,
                Instructions = this.Instructions,
            };
            List<Ingredient> ingredients = new List<Ingredient>();

            foreach (var item in SelectedItems)
            {
                ingredients.Add(await _ingredientRepository.GetByNameAsync(item));
            }
            await _recipeRepository.CreateWithIngredientsAsync(recipe, ingredients);
            return RedirectToPage("/Index");
        }
        public List<string> GetListFromSession(string key)
        {
            var sessionData = HttpContext.Session.GetString(key);
            var list = string.IsNullOrEmpty(sessionData)
                   ? new List<string>()
                   : sessionData.Split(',').ToList();
            return list;
        }
        public string GetStringFromSession(string key)
        {
            var sessionData = HttpContext.Session.GetString(key);
            var str = string.IsNullOrEmpty(sessionData)
                   ? string.Empty
                   : sessionData;
            return str;
        }
        public void AddListToSession(string key, List<string> list)
        {
            HttpContext.Session.SetString(key, string.Join(",", list));
        }
        public void AddStringToSession(string key, string str)
        {
            HttpContext.Session.SetString(key, str);
        }

        public void OnPostSaveForm(string title, string category, string instructions)
        {
            this.AddStringToSession(TitleSessionKey, title);
            this.AddStringToSession(AreaCategorySessionKey, category);
            this.AddStringToSession(InstructionsSessionKey, instructions);
        }
    }

    public class AutoCompleteItem
    {
        public string Name { get; set; }
    }

}
