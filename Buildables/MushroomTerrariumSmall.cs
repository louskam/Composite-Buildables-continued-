using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace CompositeBuildables
{
    public static class MushroomTerrariumSmall
    {
        public static PrefabInfo Info { get; } =
            PrefabInfo.WithTechType(
                    "MushroomTerrariumSmall",
                    "Mushroom Terrarium (Small)",
                    "A tubular terrarium for cultivating mushrooms. Sized for standard rooms.")
                .WithIcon(IconHelper.LoadIconOrFallback("MushroomTerrariumSmall.png", TechType.PlanterBox));

        public static bool registered = false;

        public static IEnumerator ModifyPrefabAsync(GameObject obj)
        {
            yield return CoroutineHost.StartCoroutine(PrefabFactory.ensureInitialized());

            MushroomGrower mg = obj.AddComponent<MushroomGrower>();
            mg.storageContainer = PrefabUtils.AddStorageContainer(obj, "StorageRoot", "MushroomTerrariumSmall", 6, 5, false);
            mg.storageContainer.storageLabel = "MUSHROOM TERRARIUM";

            ConstructableFlags constructableFlags = (ConstructableFlags)153;

            GameObject tubeShelfModel = obj.transform.Find("biodome_lab_tube_01").gameObject;
            obj.transform.Find("Capsule").localScale = new Vector3(0.6f, 0.55f, 0.6f);
            tubeShelfModel.transform.localScale = new Vector3(0.6f, 0.55f, 0.6f);

            ConstructableBounds cb = obj.AddComponent<ConstructableBounds>();
            cb.bounds.position = new Vector3(0f, 1.5f, 0f);
            cb.bounds.extents = new Vector3(0.3f, 0.7f, 0.3f);

            int i = 0;
            while (i < 4)
            {
                Transform planterModel = PrefabFactory.AttachModelFromPrefabTo("5c8cb04b-9f30-49e7-8687-0cbb338fc7fa", tubeShelfModel.transform);
                planterModel.localScale = new Vector3(1.8f, 0.6f, 1.8f);
                planterModel.Find("Base_interior_Planter_Pot_02").Find("pot_generic_plant_02").localScale = new Vector3(2f, 6f, 2f);

                float yPos;
                float theta;
                switch (i)
                {
                    case 0:
                        yPos = 3.65f;
                        theta = 0f;
                        break;
                    case 1:
                        yPos = 2.55f;
                        theta = 90f;
                        break;
                    case 2:
                        yPos = 1.55f;
                        theta = 180f;
                        break;
                    default:
                        yPos = 0.55f;
                        theta = 270f;
                        break;
                }

                Quaternion rot = Quaternion.AngleAxis(theta - 135f, Vector3.up);
                planterModel.localPosition = new Vector3(0f, yPos, 0f);

                Transform model = planterModel.transform.Find("Tropical_Plant_10a");
                model.gameObject.SetActive(true);
                model.parent = tubeShelfModel.transform;
                model.localPosition = new Vector3(0f, yPos + 0.225f, 0f);
                model.localScale = new Vector3(0.1f, 0.1f, 0.06f);
                model.parent = planterModel;

                model = PrefabFactory.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", tubeShelfModel.transform);
                model.localPosition = rot * new Vector3(0.25f, yPos + 0.125f, 0.25f);
                model.localScale = Vector3.one;
                model.Rotate(new Vector3(0f, theta + 180f, 0f), Space.World);
                model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));
                model.parent = planterModel;

                model = PrefabFactory.AttachModelFromPrefabTo("98be0944-e0b3-4fba-8f08-ca5d322c22f6", tubeShelfModel.transform);
                model.localPosition = rot * new Vector3(0.2f, yPos + 0.125f, -0.2f);
                model.localScale = new Vector3(0.025f, 0.025f, 0.03f);
                model.Rotate(new Vector3(0f, 160f + theta, 0f), Space.World);
                model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));
                model.parent = planterModel;

                model = PrefabFactory.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", tubeShelfModel.transform);
                model.localPosition = rot * new Vector3(-0.25f, yPos + 0.125f, -0.25f);
                model.localScale = Vector3.one;
                model.Rotate(new Vector3(0f, theta, 0f), Space.World);
                model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));
                model.parent = planterModel;

                model = PrefabFactory.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", tubeShelfModel.transform);
                model.localPosition = Quaternion.AngleAxis(10f, Vector3.up) * rot * new Vector3(-0.25f, yPos + 0.15f, 0.25f);
                model.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                model.Rotate(new Vector3(0f, 110f + theta, 0f), Space.World);
                model.parent = planterModel;

                model = PrefabFactory.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", tubeShelfModel.transform);
                model.localPosition = Quaternion.AngleAxis(-30f, Vector3.up) * rot * new Vector3(-0.27f, yPos + 0.2f, 0.27f);
                model.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                model.Rotate(new Vector3(0f, 90f + theta, 0f), Space.World);
                model.parent = planterModel;

                model = PrefabFactory.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", tubeShelfModel.transform);
                model.localPosition = Quaternion.AngleAxis(-10f, Vector3.up) * rot * new Vector3(-0.32f, yPos + 0.175f, 0.32f);
                model.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                model.Rotate(new Vector3(0f, 90f + theta, 0f), Space.World);
                model.parent = planterModel;

                i++;
            }

            SceneHelper.NormalizeSkyApplier(obj);
            SceneHelper.EnsureLightingRefreshHelper(obj);

            PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, tubeShelfModel);

            yield return obj;
        }

        public static void UpdateRecipe()
        {
            if (!registered)
                return;

            CraftDataHandler.SetRecipeData(
                Info.TechType,
                RecipeJsonHelper.GetRecipe(nameof(MushroomTerrariumSmall), Plugin.config.RecipeComplexity));
        }

        public static void Register()
        {
            BuildableRegisterHelper.RegisterBuildable(
                Info,
                "a36047b0-1533-4718-8879-d6ba9229c978",
                ModifyPrefabAsync,
                TechGroup.Miscellaneous,
                TechCategory.Misc,
                TechType.PlanterBox);

            registered = true;
            UpdateRecipe();
        }
    }
}