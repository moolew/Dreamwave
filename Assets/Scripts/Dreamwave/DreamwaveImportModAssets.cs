using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static GameManager;
using static TempoManager;
using static DreamwaveGlobal;

public static class DreamwaveImportModAssets
{
    public static Sprite LoadStreamedSprite(string filepath, string fileName, int width, int height)
    {
        string filePath = Path.GetFullPath(Path.Combine(filepath, fileName));

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.LoadImage(fileData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        else
        {
            Debug.LogError("Could not find any file with the given path " + filePath);
            return null;
        }
    }

    public static List<Sprite> LoadSpritesFromPath(string filePath, int width, int height, float rectX, float rectY, bool antiAliasing)
    {
        List<Sprite> sprites = new List<Sprite>();
        filePath = Path.GetFullPath(filePath);

        if (Directory.Exists(filePath))
        {
            // Get all .png files in the directory
            string[] imageFiles = Directory.GetFiles(filePath, "*.png");

            foreach (string file in imageFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (int.TryParse(fileName, out int index))
                {
                    byte[] fileData = File.ReadAllBytes(file);
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    if (texture.LoadImage(fileData))
                    {
                        texture.wrapMode = TextureWrapMode.Clamp;
                        texture.filterMode = antiAliasing ? FilterMode.Trilinear : FilterMode.Point;
                        texture.mipMapBias = 0;
                        texture.anisoLevel = 0;
                        texture.Apply();
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(rectX, rectY));
                        sprites.Add(sprite);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Directory does not exist: " + filePath);
        }

        return sprites;
    }

    public static List<Vector2> LoadSpriteOffsetsFromPath(string filePath)
    {
        List<Vector2> offsets = new List<Vector2>();
        filePath = Path.GetFullPath(filePath);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (values.Length == 2 &&
                    float.TryParse(values[0].Trim(), out float x) &&
                    float.TryParse(values[1].Trim(), out float y))
                {
                    Vector2 offset = new Vector2(x, y);
                    offsets.Add(offset);
                }
                else
                {
                    Debug.LogWarning("Invalid line in file: " + filePath + " Line: " + line);
                }
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }

        return offsets;
    }
}
