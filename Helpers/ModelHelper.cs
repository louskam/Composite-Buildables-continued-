using UnityEngine;

namespace CompositeBuildables
{
    internal static class ModelHelper
    {
        internal static Transform AttachWithWorldTransform(
            Transform parent,
            string prefabId,
            Vector3 worldPosition,
            Quaternion worldRotation)
        {
            Transform model = PrefabFactory.AttachModelFromPrefabTo(prefabId, parent);

            if (model == null)
                return null;

            model.rotation = worldRotation;
            model.position = worldPosition;

            return model;
        }
    }
}