using UnityEngine;

namespace CompositeBuildables
{
    internal static class SceneHelper
    {
        internal static BoxCollider AddBoxCollider(GameObject obj, Vector3 size)
        {
            return AddBoxCollider(obj, Vector3.zero, size);
        }

        internal static BoxCollider AddBoxCollider(GameObject obj, Vector3 center, Vector3 size)
        {
            BoxCollider box = obj.AddComponent<BoxCollider>();
            box.center = center;
            box.size = size;
            return box;
        }

        internal static ConstructableBounds AddConstructableBounds(GameObject obj, Vector3 position, Vector3 size)
        {
            ConstructableBounds cb = obj.AddComponent<ConstructableBounds>();
            cb.bounds.position = position;
            cb.bounds.size = size;
            return cb;
        }

        internal static void EnsureSkyApplier(GameObject obj)
        {
            SkyApplier skyApplier = obj.GetComponent<SkyApplier>();
            if (skyApplier == null)
                skyApplier = obj.AddComponent<SkyApplier>();

            skyApplier.anchorSky = Skies.BaseInterior;
            skyApplier.renderers = obj.GetComponentsInChildren<Renderer>(true);
        }

        internal static void DestroyChildIfPresent(Transform parent, string childName)
        {
            if (parent == null)
                return;

            Transform child = parent.Find(childName);
            if (child != null)
                UnityEngine.Object.Destroy(child.gameObject);
        }

        internal static void DestroyIfPresent(UnityEngine.Object obj)
        {
            if (obj != null)
                UnityEngine.Object.Destroy(obj);
        }
        internal static void EnsureLightingRefreshHelper(GameObject obj)
        {
            if (obj == null)
                return;

            if (obj.GetComponent<LightingRefreshHelper>() == null)
                obj.AddComponent<LightingRefreshHelper>();
        }
        internal static void RemoveSkyAppliersInChildren(GameObject obj, bool includeRoot = false)
        {
            if (obj == null)
                return;

            SkyApplier[] skyAppliers = obj.GetComponentsInChildren<SkyApplier>(true);
            for (int i = 0; i < skyAppliers.Length; i++)
            {
                SkyApplier skyApplier = skyAppliers[i];
                if (skyApplier == null)
                    continue;

                if (!includeRoot && skyApplier.gameObject == obj)
                    continue;

                UnityEngine.Object.Destroy(skyApplier);
            }
        }

        internal static void NormalizeSkyApplier(GameObject root)
        {
            if (root == null)
                return;

            RemoveSkyAppliersInChildren(root, false);

            SkyApplier skyApplier = root.GetComponent<SkyApplier>();
            if (skyApplier == null)
                skyApplier = root.AddComponent<SkyApplier>();

            skyApplier.anchorSky = Skies.BaseInterior;
            skyApplier.emissiveFromPower = true;
            skyApplier.dynamic = false;
            skyApplier.renderers = root.GetComponentsInChildren<Renderer>(true);

            skyApplier.RefreshDirtySky();
        }
    }
}