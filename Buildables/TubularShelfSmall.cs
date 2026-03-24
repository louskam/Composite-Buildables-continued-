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
    public static class TubularShelfSmall
    {
        public static PrefabInfo Info { get; } =
                PrefabInfo.WithTechType(
            "TubularshelfSmall",
            "Chemical Storage",
            "A tubular storage unit for synthetic chemicals. Can accept Lubricant, Bleach, Hydrochloric Acid and Benzene. Max 4 per chemical for safety reasons")
        .WithIcon(IconHelper.LoadIconOrFallback("TubularShelfSmall.png", TechType.PlanterBox));

        public static bool registered = false;

        public static IEnumerator ModifyPrefabAsync(GameObject obj)
        {
            ChemicalStorage chemicalStorage = obj.AddComponent<ChemicalStorage>();
            chemicalStorage.storageContainer = PrefabUtils.AddStorageContainer(obj, "StorageRoot", "ChemicalStorage", 4, 4, false);
            chemicalStorage.storageContainer.storageLabel = "CHEMICAL STORAGE";

            yield return CoroutineHost.StartCoroutine(PrefabFactory.ensureInitialized());

            ConstructableFlags constructableFlags = (ConstructableFlags)153;

            GameObject tubeShelfModel = obj.transform.Find("biodome_lab_tube_01").gameObject;
            obj.transform.Find("Capsule").localScale = new Vector3(0.6f, 0.55f, 0.6f);
            tubeShelfModel.transform.localScale = new Vector3(0.6f, 0.55f, 0.6f);

            ConstructableBounds cb = obj.AddComponent<ConstructableBounds>();
            cb.bounds.position = new Vector3(0f, 1.5f, 0f);
            cb.bounds.extents = new Vector3(0.3f, 0.7f, 0.3f);

            void AddShelfItemCircular(
                string modelId,
                float y,
                float angleDeg,
                float radius,
                float yOffset,
                Vector3 localScale,
                float rotationY,
                string objectName)
            {
                Transform model = PrefabFactory.AttachModelFromPrefabTo(modelId, tubeShelfModel.transform);

                SceneHelper.RemoveSkyAppliersInChildren(model.gameObject, true);

                float rad = angleDeg * Mathf.Deg2Rad;
                float x = Mathf.Sin(rad) * radius;
                float z = Mathf.Cos(rad) * radius;

                model.localPosition = new Vector3(x, y + yOffset, z);
                model.localScale = localScale;

                if (!Mathf.Approximately(rotationY, 0f))
                    model.Rotate(new Vector3(0f, rotationY, 0f), Space.World);

                model.name = objectName;
            }

            for (int i = 0; i < 4; i++)
            {
                float yPos;
                float baseAngle;

                switch (i)
                {
                    case 0:
                        yPos = 3.64f;
                        baseAngle = 0f;
                        break;

                    case 1:
                        yPos = 2.57f;
                        baseAngle = 60f;
                        break;

                    case 2:
                        yPos = 1.54f;
                        baseAngle = 160f;
                        break;

                    default:
                        yPos = 0.53f;
                        baseAngle = 220f;
                        break;
                }

                const float innerRadius = 0.35f;
                const float outerRadius = 0.55f;

                if (i == 0)
                {
                    AddShelfItemCircular("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", yPos, baseAngle + 0f, innerRadius, 0.01f, new Vector3(2f, 2f, 2f), 0f, "Polyaniline_A");
                    AddShelfItemCircular("a227d6b6-d64c-4bf0-b919-2db02d67d037", yPos, baseAngle + 40f, outerRadius, 0.01f, new Vector3(1.45f, 1.45f, 1.45f), 0f, "biodome_lab_containers_tube_02_A");
                    AddShelfItemCircular("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", yPos, baseAngle + 80f, innerRadius, 0.01f, new Vector3(2f, 2f, 2f), 0f, "Polyaniline_B");
                    AddShelfItemCircular("e7f9c5e7-3906-4efd-b239-28783bce17a5", yPos, baseAngle + 120f, outerRadius, 0.00f, new Vector3(1.35f, 1.35f, 1.35f), 20f, "biodome_lab_containers_close_01");
                    AddShelfItemCircular("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", yPos, baseAngle + 160f, innerRadius, 0.01f, new Vector3(2f, 2f, 2f), 0f, "Polyaniline_C");
                    AddShelfItemCircular("a227d6b6-d64c-4bf0-b919-2db02d67d037", yPos, baseAngle + 200f, outerRadius, 0.01f, new Vector3(1.45f, 1.45f, 1.45f), 0f, "biodome_lab_containers_tube_02_B");
                    AddShelfItemCircular("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", yPos, baseAngle + 240f, innerRadius, 0.01f, new Vector3(2f, 2f, 2f), 0f, "Polyaniline_D");
                    AddShelfItemCircular("e3e00261-92fc-4f52-bad2-4f0e5802a43d", yPos, baseAngle + 280f, outerRadius, 0.00f, new Vector3(1.35f, 1.35f, 1.35f), 30f, "biodome_lab_containers_close_02");
                    AddShelfItemCircular("7e164f67-f4e7-41fc-98a5-7a84ccaa1d09", yPos, baseAngle + 320f, innerRadius, 0.01f, new Vector3(2f, 2f, 2f), 0f, "Polyaniline_E");
                }
                else if (i == 1)
                {
                    AddShelfItemCircular("96b1b863-2ff7-451b-aa38-8b3a06e72d63", yPos, baseAngle + 0f, innerRadius, 0.25f, new Vector3(1.6f, 1.6f, 1.6f), 70f, "Lubricant_A");
                    AddShelfItemCircular("e3e00261-92fc-4f52-bad2-4f0e5802a43d", yPos, baseAngle + 40f, outerRadius, 0.00f, new Vector3(1.35f, 1.35f, 1.35f), 0f, "biodome_lab_containers_close_02_A");
                    AddShelfItemCircular("e7f9c5e7-3906-4efd-b239-28783bce17a5", yPos, baseAngle + 80f, outerRadius, 0.00f, new Vector3(1.4f, 1.4f, 1.4f), 30f, "biodome_lab_containers_close_01");
                    AddShelfItemCircular("96b1b863-2ff7-451b-aa38-8b3a06e72d63", yPos, baseAngle + 120f, outerRadius, 0.25f, new Vector3(1.6f, 1.6f, 1.6f), 70f, "Lubricant_B");
                    AddShelfItemCircular("96b1b863-2ff7-451b-aa38-8b3a06e72d63", yPos, baseAngle + 160f, innerRadius, 0.25f, new Vector3(1.6f, 1.6f, 1.6f), 70f, "Lubricant_C");
                    AddShelfItemCircular("96b1b863-2ff7-451b-aa38-8b3a06e72d63", yPos, baseAngle + 200f, outerRadius, 0.25f, new Vector3(1.6f, 1.6f, 1.6f), 70f, "Lubricant_D");
                    AddShelfItemCircular("96b1b863-2ff7-451b-aa38-8b3a06e72d63", yPos, baseAngle + 240f, innerRadius, 0.25f, new Vector3(1.6f, 1.6f, 1.6f), 70f, "Lubricant_E");
                    AddShelfItemCircular("e3e00261-92fc-4f52-bad2-4f0e5802a43d", yPos, baseAngle + 280f, outerRadius, 0.00f, new Vector3(1.4f, 1.4f, 1.4f), -20f, "biodome_lab_containers_close_02_B");                                        
                    AddShelfItemCircular("96b1b863-2ff7-451b-aa38-8b3a06e72d63", yPos, baseAngle + 320f, innerRadius, 0.25f, new Vector3(1.6f, 1.6f, 1.6f), 70f, "Lubricant_F");
                }
                else if (i == 2)
                {
                    AddShelfItemCircular("74912c22-a383-48c7-8e9e-34b515c6aebb", yPos, baseAngle + 0f, innerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Hydrochloric_acid_A");
                    AddShelfItemCircular("fab9bc63-1916-4434-a9c6-231f421ffbb5", yPos, baseAngle + 40f, outerRadius, 0.00f, new Vector3(1.8f, 1.8f, 1.8f), 180f, "Hatching_enzymes_A");
                    AddShelfItemCircular("74912c22-a383-48c7-8e9e-34b515c6aebb", yPos, baseAngle + 80f, innerRadius, 0.00f, new Vector3(2f, 2f, 2f), 30f, "Hydrochloric_acid_B");
                    AddShelfItemCircular("fab9bc63-1916-4434-a9c6-231f421ffbb5", yPos, baseAngle + 120f, outerRadius, 0.00f, new Vector3(1.8f, 1.8f, 1.8f), 180f, "Hatching_enzymes_B");
                    AddShelfItemCircular("fab9bc63-1916-4434-a9c6-231f421ffbb5", yPos, baseAngle + 160f, innerRadius, 0.00f, new Vector3(1.8f, 1.8f, 1.8f), 180f, "Hatching_enzymes_C");
                    AddShelfItemCircular("7f601dd4-0645-414d-bb62-5b0b62985836", yPos, baseAngle + 200f, outerRadius, 0.01f, new Vector3(1.5f, 1.5f, 1.5f), 0f, "biodome_lab_containers_tube_01");
                    AddShelfItemCircular("74912c22-a383-48c7-8e9e-34b515c6aebb", yPos, baseAngle + 240f, innerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Hydrochloric_acid_C");
                    AddShelfItemCircular("74912c22-a383-48c7-8e9e-34b515c6aebb", yPos, baseAngle + 280f, outerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Hydrochloric_acid_D");
                    AddShelfItemCircular("74912c22-a383-48c7-8e9e-34b515c6aebb", yPos, baseAngle + 320f, innerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Hydrochloric_acid_E");
                }
                else
                {                   
                    AddShelfItemCircular("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", yPos, baseAngle + 0f, outerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Benzene_A");
                    AddShelfItemCircular("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", yPos, baseAngle + 40f, innerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Benzene_B");
                    AddShelfItemCircular("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", yPos, baseAngle + 80f, outerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Benzene_C");
                    AddShelfItemCircular("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", yPos, baseAngle + 120f, innerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Benzene_D");
                    AddShelfItemCircular("986b31ea-3c9d-498c-9f38-2af8ffe86ed7", yPos, baseAngle + 160f, outerRadius, 0.00f, new Vector3(2f, 2f, 2f), 0f, "Benzene_E");
                    AddShelfItemCircular("9c5f22de-5049-48bb-ad1e-0d78c894210e", yPos, baseAngle + 200f, outerRadius, 0.00f, new Vector3(0.9f, 0.9f, 0.9f), baseAngle + 200f, "discovery_lab_props_02");
                    AddShelfItemCircular("fbfacd7b-32a8-4065-8c25-b0a703f2683b", yPos, baseAngle + 240f, innerRadius, 0.20f, new Vector3(1.20f, 1.20f, 1.20f), -20f, "Bleach_A");
                    AddShelfItemCircular("fbfacd7b-32a8-4065-8c25-b0a703f2683b", yPos, baseAngle + 280f, outerRadius, 0.20f, new Vector3(1.20f, 1.20f, 1.20f), -20f, "Bleach_B");
                    AddShelfItemCircular("fbfacd7b-32a8-4065-8c25-b0a703f2683b", yPos, baseAngle + 320f, innerRadius, 0.20f, new Vector3(1.20f, 1.20f, 1.20f), -20f, "Bleach_C");
                }
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
                RecipeJsonHelper.GetRecipe(nameof(TubularShelfSmall), Plugin.config.RecipeComplexity));
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