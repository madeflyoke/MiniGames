using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MiniGames.Modules.Level.Utils
{
    /// <summary>
    /// Overall currently serving only one purpose - to support scratching when textures have multiple-mode 
    /// i.e. sprite sheets  
    /// </summary>
    public class SpritesOriginalSizes : MonoBehaviour
    {
        [HideInInspector]
        [Serializable]
        public class SpriteSizesData
        {          
            public Dictionary<string,SerializableVector2> spriteSizes;
        }

        public class SerializableVector2
        {
            public int x;
            public int y;
        }

        [SerializeField] private List<Sprite> sprites;
        private Dictionary<string, SerializableVector2> spriteSizes;


#if UNITY_EDITOR
        private void Awake()
        {         
            if (sprites.Count != 0)
            {
                UpdateAndSaveTexturesData();
            }          
        }

        private SerializableVector2 GetOriginalTextureSize(Texture2D asset)
        {
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer == null)
                {
                    Debug.LogWarning("Can't get original texture size, importer is null");
                    return null;
                }
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                int width = (int)args[0];
                int height = (int)args[1];

                return new SerializableVector2 { x = width, y = height };
            }
            Debug.LogWarning("Can't get original texture size, asset is null");
            return  null;
        }

        /// <summary>
        ///  Original texture size isn't available at runtime, 
        ///  so caching data into json file, then reading it while gameplay
        /// </summary>
        private void UpdateAndSaveTexturesData() 
        {
            spriteSizes = new Dictionary<string, SerializableVector2>();
            foreach (var item in sprites)
            {
                spriteSizes[item.texture.name] = GetOriginalTextureSize(item.texture);
            }

            SpriteSizesData data = new SpriteSizesData { spriteSizes = spriteSizes };
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(Application.dataPath + "/SpritesSizeData.json", json);
        }

        public Vector2 GetOriginalWidthHeight(Texture2D texture)
        {
            return new Vector2(spriteSizes[texture.name].x, spriteSizes[texture.name].y);
        }

#else
        public Vector2 GetOriginalWidthHeight(Texture2D texture)
        {
            string text = File.ReadAllText(Application.dataPath + "/SpritesSizeData.json");

            SpriteSizesData data = JsonConvert.DeserializeObject<SpriteSizesData>(text);
            return new Vector2(data.spriteSizes[texture.name].x, data.spriteSizes[texture.name].y);
        }

#endif
    }
}

