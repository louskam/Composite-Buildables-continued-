using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Nautilus.Crafting;

namespace CompositeBuildables
{
    internal static class RecipeJsonHelper
    {
        private static readonly string RecipeFilePath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "recipes.json");

        private static Dictionary<string, RecipeComplexitySet> _recipes;

        internal static void Initialize()
        {
            EnsureRecipeFileExists();
            LoadRecipes();
        }

        internal static void Reload()
        {
            LoadRecipes();
        }

        internal static RecipeData GetRecipe(string buildableKey, RecipeComplexityEnum complexity)
        {
            if (_recipes == null)
                Initialize();

            RecipeComplexitySet set;
            if (!_recipes.TryGetValue(buildableKey, out set))
                throw new Exception("Missing recipe entry for buildable: " + buildableKey);

            List<RecipeIngredientEntry> entries;
            switch (complexity)
            {
                case RecipeComplexityEnum.Simple:
                    entries = set.Simple;
                    break;

                case RecipeComplexityEnum.Standard:
                    entries = set.Standard;
                    break;

                default:
                    entries = set.Complex;
                    break;
            }

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1
            };

            for (int i = 0; i < entries.Count; i++)
            {
                TechType techType;
                if (!Enum.TryParse(entries[i].TechType, true, out techType))
                {
                    if (Plugin.Log != null)
                        Plugin.Log.LogWarning("[RecipeJsonHelper] Unknown TechType in recipes.json: " + entries[i].TechType);

                    continue;
                }

                recipe.Ingredients.Add(new Ingredient(techType, entries[i].Amount));
            }

            return recipe;
        }

        private static void EnsureRecipeFileExists()
        {
            if (File.Exists(RecipeFilePath))
                return;

            File.WriteAllText(RecipeFilePath, GetDefaultRecipeJson(), new UTF8Encoding(false));

            if (Plugin.Log != null)
                Plugin.Log.LogInfo("[RecipeJsonHelper] Created default recipes.json");
        }

        private static void LoadRecipes()
        {
            try
            {
                using (FileStream stream = File.OpenRead(RecipeFilePath))
                {
                    DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(RecipeFileRoot));

                    RecipeFileRoot root = (RecipeFileRoot)serializer.ReadObject(stream);

                    _recipes = new Dictionary<string, RecipeComplexitySet>(StringComparer.Ordinal);

                    if (root != null && root.Buildables != null)
                    {
                        for (int i = 0; i < root.Buildables.Count; i++)
                        {
                            RecipeEntry entry = root.Buildables[i];
                            if (entry == null || string.IsNullOrEmpty(entry.Key) || entry.Recipes == null)
                                continue;

                            _recipes[entry.Key] = entry.Recipes;
                        }
                    }
                }

                if (Plugin.Log != null)
                    Plugin.Log.LogInfo("[RecipeJsonHelper] Loaded recipes.json");
            }
            catch (Exception ex)
            {
                if (Plugin.Log != null)
                    Plugin.Log.LogError("[RecipeJsonHelper] Failed to load recipes.json: " + ex);

                throw;
            }
        }

        private static string GetDefaultRecipeJson()
        {
            return
@"{
  ""Buildables"": [
    {
      ""Key"": ""ScienceBench1"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 3 },
          { ""TechType"": ""Glass"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""Glass"", ""Amount"": 5 }
        ]
      }
    },
    {
      ""Key"": ""ScienceBench2"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 3 },
          { ""TechType"": ""Glass"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""Glass"", ""Amount"": 3 }
        ]
      }
    },
    {
      ""Key"": ""LabShelving"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 1 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 3 },
          { ""TechType"": ""Glass"", ""Amount"": 1 },
          { ""TechType"": ""Computerchip"", ""Amount"": 1 },
          { ""TechType"": ""Polyaniline"", ""Amount"": 1 },
          { ""TechType"": ""Benzene"", ""Amount"": 1 },
          { ""TechType"": ""HydrochloricAcid"", ""Amount"": 1 },
          { ""TechType"": ""Lubricant"", ""Amount"": 1 },
          { ""TechType"": ""Bleach"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 8 },
          { ""TechType"": ""Glass"", ""Amount"": 7 },
          { ""TechType"": ""Advancedwiringkit"", ""Amount"": 1 },
          { ""TechType"": ""Polyaniline"", ""Amount"": 1 },
          { ""TechType"": ""Benzene"", ""Amount"": 3 },
          { ""TechType"": ""HydrochloricAcid"", ""Amount"": 2 },
          { ""TechType"": ""Lubricant"", ""Amount"": 3 },
          { ""TechType"": ""Bleach"", ""Amount"": 3 }
        ]
      }
    },
    {
      ""Key"": ""TubularShelfSmall"", 
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 2 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 4 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""EnameledGlass"", ""Amount"": 2 },
          { ""TechType"": ""LabEquipment2"", ""Amount"": 1 },
        ]
      }
    },
    {
      ""Key"": ""FlowerTerrariumSmall"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 1 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 2 },
          { ""TechType"": ""PinkFlowerSeed"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 4 },
          { ""TechType"": ""PinkFlowerSeed"", ""Amount"": 4 }
        ]
      }
    },
    {
      ""Key"": ""MushroomTerrariumSmall"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 1 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 1 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 1 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 2 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 2 },
          { ""TechType"": ""Glass"", ""Amount"": 1 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 4 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 4 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 8 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 4 }
        ]
      }
    },
    {
      ""Key"": ""MushroomTerrariumLarge"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""Glass"", ""Amount"": 2 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""Glass"", ""Amount"": 2 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 1 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 2 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""Glass"", ""Amount"": 2 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 4 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 4 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 8 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 4 }
        ]
      }
    },
    {
      ""Key"": ""DegasiPlanter"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""HangingFruit"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""HangingFruit"", ""Amount"": 1 },
          { ""TechType"": ""FernPalmSeed"", ""Amount"": 2 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 1 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkFlowerSeed"", ""Amount"": 1 }
        ]
      }
    },
    {
      ""Key"": ""DegasiPlanter2"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""FernPalmSeed"", ""Amount"": 2 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""FernPalmSeed"", ""Amount"": 2 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 1 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkFlowerSeed"", ""Amount"": 1 }
        ]
      }
    },
    {
      ""Key"": ""DegasiPlanterRound"",
      ""Recipes"": {
        ""Simple"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 }
        ],
        ""Standard"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""HangingFruit"", ""Amount"": 1 }
        ],
        ""Complex"": [
          { ""TechType"": ""Titanium"", ""Amount"": 4 },
          { ""TechType"": ""HangingFruit"", ""Amount"": 1 },
          { ""TechType"": ""FernPalmSeed"", ""Amount"": 2 },
          { ""TechType"": ""OrangePetalsPlantSeed"", ""Amount"": 1 },
          { ""TechType"": ""OrangeMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkMushroomSpore"", ""Amount"": 1 },
          { ""TechType"": ""PurpleRattleSpore"", ""Amount"": 1 },
          { ""TechType"": ""PinkFlowerSeed"", ""Amount"": 1 }
        ]
      }
    }
  ]
}";
        }
    }
}