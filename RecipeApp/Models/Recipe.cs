using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string AreaCategory { get; set; }
    public string Instructions { get; set; }
    public string ImageUrl { get; set; }
    public string VideoSourceUrl { get; set; }
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
}
