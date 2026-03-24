using System;
using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace CompositeBuildables
{
    internal static class BuildableRegisterHelper
    {
        internal static void RegisterBuildable(
            PrefabInfo info,
            string cloneSourceId,
            Func<GameObject, IEnumerator> modifyPrefabAsync,
            TechGroup group,
            TechCategory category,
            TechType unlockTechType)
        {
            CustomPrefab customPrefab = new CustomPrefab(info);

            CloneTemplate cloneTemplate = new CloneTemplate(info, cloneSourceId);
            cloneTemplate.ModifyPrefabAsync += modifyPrefabAsync;

            customPrefab.SetGameObject(cloneTemplate);

            GadgetExtensions.SetPdaGroupCategory(customPrefab, group, category);
            GadgetExtensions.SetUnlock(customPrefab, unlockTechType, 1);

            customPrefab.Register();
        }
    }
}