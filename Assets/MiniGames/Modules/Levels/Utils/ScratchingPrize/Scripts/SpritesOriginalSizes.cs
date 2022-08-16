using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MiniGames.Modules.Level.Utils
{

    public class SpritesOriginalSizes : MonoBehaviour
    {
        [HideInInspector]
        [Serializable]
        public class SpriteSizesData
        {
            
            public KeyValuePair<string, Vector2>[] spriteSizes;
        }


        [SerializeField] private List<Sprite> sprites;
        public Dictionary<string, Vector2> spriteSizes;


#if UNITY_EDITOR
        private void Awake()
        {
            if (sprites.Count==0)
            {
                return;
            }
            spriteSizes = new Dictionary<string, Vector2>();
            foreach (var item in sprites)
            {
                spriteSizes[item.texture.name] = GetOriginalTextureSize(item.texture);
            }

            SpriteSizesData data = new SpriteSizesData { spriteSizes = new KeyValuePair<string, Vector2>[1] 
            {
              new KeyValuePair<string, Vector2>("yo",Vector2.zero)
            }};
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(Application.dataPath + "/SpritesSizeData.txt", json);
            
        }

        private Vector2 GetOriginalTextureSize(Texture2D asset)
        {
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer == null)
                {
                    Debug.LogWarning("Can't get original texture size, importer is null");
                    return Vector2.zero;
                }
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                int width = (int)args[0];
                int height = (int)args[1];

                return new Vector2(width,height);
            }
            Debug.LogWarning("Can't get original texture size, asset is null");
            return Vector2.zero;
        }
#else
        




#endif
    }
}

