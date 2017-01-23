using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StorageManager
{
    internal static class ContentManager
    {
        private static string STORAGE_ENABLE_ICON_PATH = @"Mods\Textures\StorageEnable.png";
        private static string STORAGE_DISABLE_ICON_PATH = @"Mods\Textures\StorageDisable.png";
        public static Texture2D StorageEnableIcon { get; private set; }
        public static Texture2D StorageDisableIcon { get; private set; }

        private static float greyScaleBrightness = 180f / 255f;

        internal static Dictionary<ResourceType, IconPair> LoadContent(ResourceType[] resourceTypes)
        {
            STORAGE_ENABLE_ICON_PATH = Path.Combine(Util.getFilesFolder(), STORAGE_ENABLE_ICON_PATH);
            STORAGE_DISABLE_ICON_PATH = Path.Combine(Util.getFilesFolder(), STORAGE_DISABLE_ICON_PATH);

            StorageEnableIcon = LoadTexture(STORAGE_ENABLE_ICON_PATH);
            StorageDisableIcon = LoadTexture(STORAGE_DISABLE_ICON_PATH);

            List<IconPair> icons = resourceTypes.Select(x => new IconPair(x.getIcon(), ApplyColorFix(x.getIcon()))).ToList();

            return resourceTypes.Select((k, i) => new { k, v = icons[i] }).ToDictionary(x => x.k, x => x.v);
        }

        private static Texture2D LoadTexture(string filepath)
        {
            if (File.Exists(filepath))
            {
                byte[] iconBytes = File.ReadAllBytes(filepath);
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(iconBytes);
                return Util.applyColor(tex);
            }

            return new Texture2D(1, 1);
        }
        public static Texture2D ApplyColorFix(Texture2D texture)
        {
            var pixels = texture.GetPixels();
            var greyscalePixels = pixels.Select(p => new Color(greyScaleBrightness, greyScaleBrightness, greyScaleBrightness, p.a)).ToArray();
            texture.SetPixels(greyscalePixels);
            return Util.applyColor(texture);
        }
    }
}
