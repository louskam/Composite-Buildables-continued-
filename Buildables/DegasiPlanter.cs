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
    public static class DegasiPlanter
    {
        public static PrefabInfo Info { get; } =
            PrefabInfo.WithTechType(
                    "DegasiPlanter",
                    "Degasi Planter",
                    "Bart Torgal's planter from Degasi Base 1-a.")
                .WithIcon(IconHelper.LoadIconOrFallback("DegasiPlanter.png", TechType.PlanterBox));

        public static bool registered = false;

        public static IEnumerator ModifyPrefabAsync(GameObject obj)
        {
            PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Near);

            yield return CoroutineHost.StartCoroutine(PrefabFactory.ensureInitialized());

            ConstructableFlags constructableFlags = (ConstructableFlags)153;

            GameObject planterModel = obj.transform.Find("model").gameObject;
            planterModel.transform.Rotate(new Vector3(0f, 180f, 0f), Space.World);

            GameObject lanternTreeObj = PrefabFactory.InstantiatePrefabInactive("8fa4a413-57fa-47a3-828d-de2255dbce4f");
            Transform lanternTreeModel = lanternTreeObj.transform.Find(PrefabFactory.GetModelName("8fa4a413-57fa-47a3-828d-de2255dbce4f"));
            lanternTreeModel.SetParent(planterModel.transform, false);
            lanternTreeModel.localPosition = new Vector3(0f, 0.3757f, 0f);

            DestroyFruitColliderIfPresent(lanternTreeModel, "Fruit_15");
            DestroyFruitColliderIfPresent(lanternTreeModel, "Fruit_16");
            DestroyFruitColliderIfPresent(lanternTreeModel, "Fruit_17");
            DestroyFruitColliderIfPresent(lanternTreeModel, "Fruit_18");
            DestroyFruitColliderIfPresent(lanternTreeModel, "Fruit_19");
            DestroyFruitColliderIfPresent(lanternTreeModel, "Fruit_20");

            FruitPlantClone fruitPlant = obj.AddComponent<FruitPlantClone>();
            FruitPlant oldFruitPlant = lanternTreeObj.GetComponent<FruitPlant>();

            if (oldFruitPlant != null && oldFruitPlant.fruits != null)
            {
                fruitPlant.fruits = new PickPrefab[oldFruitPlant.fruits.Length];
                for (int i = 0; i < oldFruitPlant.fruits.Length; i++)
                {
                    fruitPlant.fruits[i] = oldFruitPlant.fruits[i];
                }
            }

            UnityEngine.Object.Destroy(lanternTreeObj);

            Transform model = PrefabFactory.AttachModelFromPrefabTo("1d6d89dd-3e49-48b7-90e4-b521fbc3d36f", planterModel.transform);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.localPosition = new Vector3(0.185f, 0.3757f, -1f);

            model = PrefabFactory.AttachModelFromPrefabTo("523879d5-3241-4a94-8588-cb3b38945119", planterModel.transform);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.localPosition = new Vector3(-0.71f, 0.3757f, -0.481f);

            model = PrefabFactory.AttachModelFromPrefabTo("154a88c1-6c7f-44e4-974e-c52d2f48fa28", planterModel.transform);
            model.localScale = new Vector3(0.3503f, 0.3503f, 0.3503f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.localPosition = new Vector3(0.509f, 0.3757f, 0.611f);

            model = PrefabFactory.AttachModelFromPrefabTo("75ab087f-9934-4e2a-b025-02fc333a5c99", planterModel.transform);
            model.localScale = new Vector3(0.3122f, 0.3122f, 0.3122f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.localPosition = new Vector3(0.053f, 0.3757f, -0.009f);

            model = PrefabFactory.AttachModelFromPrefabTo("28ec1137-da13-44f3-b76d-bac12ab766d1", planterModel.transform);
            model.localPosition = new Vector3(-0.4871f, 0.3757f, 0.35f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

            model = PrefabFactory.AttachModelFromPrefabTo("35056c71-5da7-4e73-be60-3c22c5c9e75c", planterModel.transform);
            model.localScale = new Vector3(0.4366f, 0.4366f, 0.4366f);
            model.localPosition = new Vector3(1.0079f, 0.2805f, 0.627f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);

            model = PrefabFactory.AttachModelFromPrefabTo("28c73640-a713-424a-91c6-2f5d4672aaea", planterModel.transform);
            model.localScale = new Vector3(0.4366f, 0.4366f, 0.4366f);
            model.localPosition = new Vector3(0.578f, 0.3757f, -1.028f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

            model = PrefabFactory.AttachModelFromPrefabTo("559fe0c7-1754-40f5-9453-b537900b3ac4", planterModel.transform);
            model.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            model.localPosition = new Vector3(-0.578f, 0.2805f, -0.9f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

            model = PrefabFactory.AttachModelFromPrefabTo("98be0944-e0b3-4fba-8f08-ca5d322c22f6", planterModel.transform);
            model.localScale = new Vector3(0.02f, 0.02f, 0.028f);
            model.localPosition = new Vector3(-0.4871f, 0.271f, -0.05f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

            model = PrefabFactory.AttachModelFromPrefabTo("c7faff7e-d9ff-41b4-9782-98d2e09d29c1", planterModel.transform);
            model.localPosition = new Vector3(0.6871f, 0.471f, -0.05f);
            model.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            model.GetComponent<Renderer>().material.SetColor("_Scale", new Color(0f, 0f, 0f, 0f));

            SceneHelper.EnsureSkyApplier(planterModel);

            PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, planterModel);

            SceneHelper.DestroyIfPresent(obj.GetComponent<Planter>());
            SceneHelper.DestroyIfPresent(obj.GetComponent<StorageContainer>());
            SceneHelper.DestroyChildIfPresent(obj.transform, "slots");
            SceneHelper.DestroyChildIfPresent(obj.transform, "slots_big");
            SceneHelper.DestroyChildIfPresent(obj.transform, "StorageRoot");
            SceneHelper.DestroyChildIfPresent(obj.transform, "grownPlants");

            yield return obj;
        }

        public static void UpdateRecipe()
        {
            if (!registered)
                return;

            CraftDataHandler.SetRecipeData(
                Info.TechType,
                RecipeJsonHelper.GetRecipe(nameof(DegasiPlanter), Plugin.config.RecipeComplexity));
        }

        public static void Register()
        {
            BuildableRegisterHelper.RegisterBuildable(
                Info,
                "87f5d3e6-e00b-4cf3-be39-0a9c7e951b84",
                ModifyPrefabAsync,
                TechGroup.InteriorModules,
                TechCategory.InteriorModule,
                TechType.PlanterBox);

            registered = true;
            UpdateRecipe();
        }

        private static void DestroyFruitColliderIfPresent(Transform root, string childName)
        {
            if (root == null)
                return;

            Transform child = root.Find(childName);
            if (child == null)
                return;

            SphereCollider collider = child.GetComponent<SphereCollider>();
            if (collider != null)
            {
                UnityEngine.Object.Destroy(collider);
            }
        }
    }
}