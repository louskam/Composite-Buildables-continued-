using System.IO;
using System.Reflection;
using Nautilus.Utility;
using UnityEngine;

namespace CompositeBuildables
{
    internal static class IconHelper
    {
        internal static Sprite LoadIconOrFallback(string fileName, TechType fallbackTechType)
        {
            string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(pluginFolder, "Assets", fileName);

            if (File.Exists(fullPath))
            {
                Sprite sprite = ImageUtils.LoadSpriteFromFile(fullPath, TextureFormat.BC7);
                if (sprite != null)
                    return sprite;
            }

            if (Plugin.Log != null)
                Plugin.Log.LogWarning("[IconHelper] Missing icon, using fallback for " + fileName);

            return SpriteManager.Get(fallbackTechType);
        }
    }
}