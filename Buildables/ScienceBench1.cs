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
    public static class ScienceBench1
    {
        public static PrefabInfo Info { get; } =
            PrefabInfo.WithTechType(
                    "ScienceBench1",
                    "Science Bench 1",
                    "Lab Bench with microscope.")
                .WithIcon(IconHelper.LoadIconOrFallback("ScienceBench1.png", TechType.StarshipDesk));

        public static bool registered = false;

        public static IEnumerator ModifyPrefabAsync(GameObject obj)
        {
            yield return CoroutineHost.StartCoroutine(PrefabFactory.ensureInitialized());

            ConstructableFlags constructableFlags = (ConstructableFlags)153;

            GameObject counterModel = obj.transform.Find("biodome_lab_counter_01").gameObject;

            SceneHelper.AddBoxCollider(obj, new Vector3(0f, 0f, 0.5f), new Vector3(2.0433f, 1.02f, 0.6673f));
            SceneHelper.AddConstructableBounds(obj, new Vector3(0f, 0.52f, 0f), new Vector3(2.2433f, 1.02f, 0.8673f));

            Transform 
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"2cee55bc-6136-47c5-a1ed-14c8f3203856",
                counterModel.transform.position + new Vector3(0f, 1.0203f, -0.1f),
                Quaternion.Euler(-90f, 0f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"7f601dd4-0645-414d-bb62-5b0b62985836",
                counterModel.transform.position + new Vector3(-0.58f, 1.0203f, 0.1f),
                Quaternion.Euler(0f, -45f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"e7f9c5e7-3906-4efd-b239-28783bce17a5",
                counterModel.transform.position + new Vector3(-0.77f, 1.0203f, -0.1f),
                Quaternion.Euler(0f, -40f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"e3e00261-92fc-4f52-bad2-4f0e5802a43d",
                counterModel.transform.position + new Vector3(-0.97f, 1.0203f, 0.12f),
                Quaternion.Euler(0f, -40f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"a7519acf-6dec-429e-82ed-bbcf7a616c50",
                counterModel.transform.position + new Vector3(0.75f, 1.0203f, 0.1f),
                Quaternion.Euler(0f, 235f, 0f) * Quaternion.Euler(-90f, 0f, 0f));
            model = ModelHelper.AttachWithWorldTransform(counterModel.transform,"fab9bc63-1916-4434-a9c6-231f421ffbb5",
                counterModel.transform.position + new Vector3(0.5f, 1.0203f, -0.25f),
                Quaternion.Euler(0f, -110f, 0f) * Quaternion.Euler(0f, -20f, 0f));

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
                RecipeJsonHelper.GetRecipe(nameof(ScienceBench1), Plugin.config.RecipeComplexity));
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