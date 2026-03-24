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
    public static class LabShelving
    {
        public static PrefabInfo Info { get; } =
            PrefabInfo.WithTechType(
                    "LabShelving",
                    "Lab Shelving Unit",
                    "Standard shelving unit with Chemical Synthesizer.")
                .WithIcon(IconHelper.LoadIconOrFallback("LabShelving.png", TechType.StarshipDesk));

        public static bool registered = false;

        public static IEnumerator ModifyPrefabAsync(GameObject obj)
        {
            yield return CoroutineHost.StartCoroutine(PrefabFactory.ensureInitialized());

            ConstructableFlags constructableFlags = (ConstructableFlags)153;

            GameObject mainModel = obj.transform.Find("biodome_lab_shelf_01").gameObject;
            mainModel.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            mainModel.name = "MainShelf";

            GameObject cubeModel = obj.transform.Find("Cube").gameObject;
            cubeModel.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);

            ChemicalSynthesizer synthesizer = obj.AddComponent<ChemicalSynthesizer>();
            synthesizer.storageContainer = PrefabUtils.AddStorageContainer(obj, "StorageRoot", "LabShelving", 2, 4, false);
            synthesizer.storageContainer.storageLabel = "CHEMICAL SYNTHESIZER";
            synthesizer.maxLubricant = 2;
            synthesizer.maxBleach = 2;
            synthesizer.maxBenzene = 2;
            synthesizer.maxHydrochloricAcid = 2;

            SceneHelper.AddBoxCollider(obj, new Vector3(0.8f, 0.9f, 0.5f));
            SceneHelper.AddConstructableBounds(obj, new Vector3(0f, 0.46f, 0f), new Vector3(0.8f, 0.9f, 0.5f));

            Transform model = PrefabFactory.AttachModelFromPrefabTo("33acd899-72fe-4a98-85f9-b6811974fbeb", mainModel.transform);
            model.localScale = new Vector3(0.25f, 1f, 1f);
            SceneHelper.DestroyChildIfPresent(model, "biodome_lab_shelf_01_thing");
            SceneHelper.DestroyChildIfPresent(model, "biodome_lab_shelf_01_thing_glass");
            model.rotation = Quaternion.Euler(0f, -90f, 0f);
            model.position = mainModel.transform.position + new Vector3(0.785f, 0.001f, 0.25f);
            model.name = "SecondShelf";

            model = PrefabFactory.AttachModelFromPrefabTo("33acd899-72fe-4a98-85f9-b6811974fbeb", mainModel.transform);
            model.localScale = new Vector3(0.25f, 1f, 1f);
            SceneHelper.DestroyChildIfPresent(model, "biodome_lab_shelf_01_thing");
            SceneHelper.DestroyChildIfPresent(model, "biodome_lab_shelf_01_thing_glass");
            model.rotation = Quaternion.Euler(0f, 90f, 0f);
            model.position = mainModel.transform.position + new Vector3(-0.785f, 0.001f, 0.25f);
            model.name = "ThirdShelf";

            for (int i = 0; i < 2; i++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("3fd9050b-4baf-4a78-a883-e774c648887c", mainModel.transform);
                model.localScale = new Vector3(2.05f, 2.05f, 2.05f);
                model.localPosition = new Vector3(1f - i * 2f, 2.45f, 0.75f);
                model.name = "SampleAnalyzer" + i;
            }

            model = PrefabFactory.AttachModelFromPrefabTo("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", mainModel.transform);
            model.localScale = new Vector3(2f, 2f, 2f);
            model.localPosition = new Vector3(0f, 2.45f, 0.75f);
            model.name = "Polyaniline1";

            for (int j = 0; j < 2; j++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("7f601dd4-0645-414d-bb62-5b0b62985836", mainModel.transform);
                model.localScale = new Vector3(2f, 2f, 2f);
                model.Rotate(new Vector3(0f, 90f, 0f), Space.World);
                model.localPosition = new Vector3(1.4f - j * 0.4f, 3.45f, 0.75f);
                model.name = "TestTube" + j;
            }

            for (int k = 0; k < 3; k++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("a227d6b6-d64c-4bf0-b919-2db02d67d037", mainModel.transform);
                model.localScale = new Vector3(2f, 2f, 2f);
                model.localPosition = new Vector3(-1.4f + k * 0.4f, 3.45f, 0.75f);
                model.name = "BellJar" + k;
            }

            for (int l = 0; l < 3; l++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", mainModel.transform);
                model.localScale = new Vector3(2.3f, 2.3f, 2.3f);
                model.localPosition = new Vector3(1.4f - l * 0.4f, 1.45f, 0.75f);
                model.name = "Benzene" + l;
            }

            for (int m = 0; m < 2; m++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("74912c22-a383-48c7-8e9e-34b515c6aebb", mainModel.transform);
                model.localScale = new Vector3(2f, 2f, 2f);
                model.localPosition = new Vector3(-1.4f + m * 0.4f, 1.45f, 0.75f);
                model.name = "HydrochloricAcid" + m;
            }

            for (int n = 0; n < 3; n++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("96b1b863-2ff7-451b-aa38-8b3a06e72d63", mainModel.transform);
                model.localScale = new Vector3(2f, 2f, 2f);
                model.Rotate(new Vector3(0f, 65f, 0f), Space.World);
                model.localPosition = new Vector3(1.4f - n * 0.4f, 0.65f, 0.75f);
                model.name = "Lubricant" + n;
            }

            for (int i2 = 0; i2 < 3; i2++)
            {
                model = PrefabFactory.AttachModelFromPrefabTo("fbfacd7b-32a8-4065-8c25-b0a703f2683b", mainModel.transform);
                model.localScale = new Vector3(2f, 2f, 2f);
                model.Rotate(new Vector3(0f, 135f, 0f), Space.World);
                model.localPosition = new Vector3(-1.4f + i2 * 0.4f, 0.65f, 0.75f);
                model.name = "Bleach" + i2;
            }

            SceneHelper.NormalizeSkyApplier(obj);
            SceneHelper.EnsureLightingRefreshHelper(obj);

            PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, mainModel);

            yield return obj;
        }

        public static void UpdateRecipe()
        {
            if (!registered)
                return;

            CraftDataHandler.SetRecipeData(
                Info.TechType,
                RecipeJsonHelper.GetRecipe(nameof(LabShelving), Plugin.config.RecipeComplexity));
        }

        public static void Register()
        {
            BuildableRegisterHelper.RegisterBuildable(
                Info,
                "33acd899-72fe-4a98-85f9-b6811974fbeb",
                ModifyPrefabAsync,
                TechGroup.Miscellaneous,
                TechCategory.Misc,
                TechType.StarshipDesk);

            registered = true;
            UpdateRecipe();
        }
    }
}