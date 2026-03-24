using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace CompositeBuildables
{
    public static class ScienceBench2
    {
        public static PrefabInfo Info { get; } =
            PrefabInfo.WithTechType(
                    "ScienceBench2",
                    "Science Bench 2",
                    "Lab Bench with sample analyzer.")
                .WithIcon(IconHelper.LoadIconOrFallback("ScienceBench2.png", TechType.StarshipDesk));

        public static bool registered = false;

        public static IEnumerator ModifyPrefabAsync(GameObject obj)
        {
            yield return CoroutineHost.StartCoroutine(PrefabFactory.ensureInitialized());

            ConstructableFlags constructableFlags = (ConstructableFlags)153;

            GameObject counterModel = obj.transform.Find("biodome_lab_counter_01").gameObject;

            SceneHelper.AddBoxCollider(obj, new Vector3(0f, 0f, 0.5f), new Vector3(2.1433f, 1.02f, 0.7673f));
            SceneHelper.AddConstructableBounds(obj, new Vector3(0f, 0.52f, 0f), new Vector3(2.2433f, 1.02f, 0.8673f));

            Transform 
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"3fd9050b-4baf-4a78-a883-e774c648887c",
                counterModel.transform.position + new Vector3(0.5f, 1.0203f, -0.1f),
                Quaternion.Euler(0f, 0f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"7f601dd4-0645-414d-bb62-5b0b62985836",
                counterModel.transform.position + new Vector3(-0.28f, 1.0203f, 0f),
                Quaternion.Euler(0f, -45f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"7f601dd4-0645-414d-bb62-5b0b62985836",
                counterModel.transform.position + new Vector3(-0.17f, 1.0203f, -0.2f),
                Quaternion.Euler(0f, -40f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"a7519acf-6dec-429e-82ed-bbcf7a616c50",
                counterModel.transform.position + new Vector3(-0.75f, 1.0203f, 0.1f),
                Quaternion.Euler(0f, 280f, 0f) * Quaternion.Euler(-90f, 0f, 0f));

            SceneHelper.EnsureSkyApplier(counterModel);

            PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, counterModel);

            yield return obj;
        }

        public static void UpdateRecipe()
        {
            if (!registered)
                return;

            CraftDataHandler.SetRecipeData(
                Info.TechType,
                RecipeJsonHelper.GetRecipe(nameof(ScienceBench2), Plugin.config.RecipeComplexity));
        }

        public static void Register()
        {
            BuildableRegisterHelper.RegisterBuildable(
                Info,
                "42eae67f-f31a-45a0-95bf-27e189de65a0",
                ModifyPrefabAsync,
                TechGroup.Miscellaneous,
                TechCategory.Misc,
                TechType.StarshipDesk);

            registered = true;
            UpdateRecipe();
        }
    }
}