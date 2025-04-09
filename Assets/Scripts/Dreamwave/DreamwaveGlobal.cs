using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DreamwaveGlobal
{
    public static List<ModSong> ModSongs = new List<ModSong>(); // store the freaky mods
    public static ModSong LoadedModSong; // the chosen freaky mod

    public static void LoadMod(int index) => LoadedModSong = ModSongs[index]; // assign the freaky mod

    public static string NormalisePath(string toNorm)
    {
        return toNorm.Replace("/", "\\");
    }
}

// make the freaky mods :)))
public class ModSong
{
    public string name;
    public string creator;

    public string backgroundSprite;
    public string playerSprites;
    public string enemySprites;

    public string chartSettings;
    public string playerChart;
    public string enemyChart;
    public string eventChart;

    public string music;
}
