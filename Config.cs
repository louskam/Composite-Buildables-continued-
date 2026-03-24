using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace CompositeBuildables
{
    [Menu("Composite Buildables")]
    public class Config : ConfigFile
    {
        private void UpdateRecipes()
        {
            RecipeJsonHelper.Reload();

            if (Plugin.Log != null)
                Plugin.Log.LogDebug("Choice value RecipeComplexity was changed to " + RecipeComplexity);

            DegasiPlanter.UpdateRecipe();
            DegasiPlanterRound.UpdateRecipe();
            ScienceBench1.UpdateRecipe();
            ScienceBench2.UpdateRecipe();
            LabShelving.UpdateRecipe();
            MushroomTerrariumSmall.UpdateRecipe();
            MushroomTerrariumLarge.UpdateRecipe();
            TubularShelfSmall.UpdateRecipe();
            // později sem můžeš doplnit i další buildables, pokud mají UpdateRecipe()
        }

        private void UpdateLanternTreesSpawnFruited()
        {
            if (Plugin.Log != null)
                Plugin.Log.LogDebug("Bool value LanternTreesSpawnFruited was changed to " + LanternTreesSpawnFruited);
        }

        private void UpdateLanternFruitSpawnTime()
        {
            if (Plugin.Log != null)
                Plugin.Log.LogDebug("Float value LanternFruitSpawnTime was changed to " + LanternFruitSpawnTime);
        }

        private void UpdateMushroomProductionSpeedFactor()
        {
            if (Plugin.Log != null)
                Plugin.Log.LogDebug("Float value MushroomProductionSpeedFactor was changed to " + MushroomProductionSpeedFactor);
        }

        [Choice("Recipe Complexity", new[] { "Simple", "Standard", "Complex" },
        Tooltip = "Changes the crafting costs of Composite Buildables.")]
        [OnChange(nameof(UpdateRecipes))]
        public RecipeComplexityEnum RecipeComplexity = RecipeComplexityEnum.Standard;

        [Toggle("Newly placed Lantern Trees are fruited", 
        Tooltip = "If enabled, newly built lantern trees start with fruit already grown.")]
        [OnChange(nameof(UpdateLanternTreesSpawnFruited))]
        public bool LanternTreesSpawnFruited = false;

        [Slider("Lantern Fruit Spawn Time (s)", 0f, 100f, DefaultValue = 50f, Format = "{0:F2}")]
        [OnChange(nameof(UpdateLanternFruitSpawnTime))]
        public float LanternFruitSpawnTime = 50f;

        [Toggle("Mushroom Terrariums overflow into Bioreactors",
        Tooltip = "If terrarium storage is full, finished mushrooms can be sent into bioreactors in the same base.")]
        public bool MushroomsToReactors = false;

        [Slider("Mushroom Terrarium Production Speed Factor", 0f, 2f, DefaultValue = 0.25f, Format = "{0:F2}",
        Tooltip = "0 disables production. Higher values make mushrooms grow faster.")]
        [OnChange(nameof(UpdateMushroomProductionSpeedFactor))]
        public float MushroomProductionSpeedFactor = 0.25f;

        [Toggle("Chemical shelves synthesize chemicals",
        Tooltip = "Lab Shelving synthesizes chemicals and stores them locally or in nearby Chemical Storage.")]
        public bool ChemicalShelvesSynthesize = false;
    }
}