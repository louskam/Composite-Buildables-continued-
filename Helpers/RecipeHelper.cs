using Nautilus.Crafting;
using System.Collections.Generic;

namespace CompositeBuildables
{
    internal static class RecipeHelper
    {
        internal static RecipeData CreateRecipe(params Ingredient[] ingredients)
        {
            RecipeData recipe = new RecipeData
            {
                craftAmount = 1
            };

            for (int i = 0; i < ingredients.Length; i++)
            {
                recipe.Ingredients.Add(ingredients[i]);
            }

            return recipe;
        }
    }
}