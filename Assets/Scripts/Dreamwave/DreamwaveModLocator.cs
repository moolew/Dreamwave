using System.IO;
using TMPro;
using UnityEngine;
using TagLib;
using UnityEngine.UI;

public class DreamwaveModLocator : MonoBehaviour
{
    [SerializeField] private Transform _songsListParent;
    [SerializeField] private GameObject _song;

    private void Awake()
    {
        FindAllMods();
    }

    private void FindAllMods()
    {
        string modsDir = Path.Combine(Application.streamingAssetsPath, "Mods");

        // does the user have their moddies folder!??!?!?
        if (!Directory.Exists(modsDir)) return;

        string[] subFolders = Directory.GetDirectories(modsDir);

        foreach (string subFolder in subFolders)
        {
            string dataPath = Path.Combine(subFolder, "Dreamwave", "dreamwavedata.txt");

            if (System.IO.File.Exists(dataPath))
            {
                string[] data = System.IO.File.ReadAllLines(dataPath); // snort all da lines X)
                CreateModLoaderUi(data, subFolder);
            }
        }
    }

    private void CreateModLoaderUi(string[] data, string modPath)
    {
        GameObject localSong = Instantiate(_song, _songsListParent);
        localSong.SetActive(true);

        string song = "", difficulty = "", time = "", score = "", accuracy = "", ranking = "", bannerLocation = "banner.png";

        TextMeshProUGUI songName = localSong.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI creatorName = localSong.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI songInformation = localSong.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Image banner = localSong.transform.GetChild(4).GetComponent<Image>();

        foreach (string line in data)
        {
            if (!line.Contains("=")) continue;

            string[] parts = line.Split('=');
            if (parts.Length < 2) continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case "songName":
                    song = value;
                    songName.text = value;
                    break;
                case "creator":
                    creatorName.text = value;
                    break;
                case "difficulty":
                    difficulty = value;
                    break;
                case "score":
                    score = value;
                    break;
                case "accuracy":
                    accuracy = value;
                    break;
                case "ranking":
                    ranking = value;
                    break;
                case "bannerLocation":
                    bannerLocation = value;
                    break;
            }
        }

        #region MP3 LOADING WE GONNA CHECK DAT SONG LENGTH BABYYYYYY
        string mp3Path = Path.Combine(modPath, "Music", "Inst.mp3").Replace("/", "\\");
        if (System.IO.File.Exists(mp3Path))
        {
            var tagFile = TagLib.File.Create(mp3Path);
            var duration = tagFile.Properties.Duration;
            time = $"{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
        else
        {
            Debug.LogWarning($"MP3 not found at: {mp3Path}");
            time = "??:??";
        }
        #endregion

        // Banner loading
        string bannerPath = Path.Combine(modPath, "Dreamwave", bannerLocation);
        banner.sprite = LoadStreamedSprite(bannerPath);

        // Final UI text
        songInformation.text = $"{difficulty} ~ {time} / {ranking} ~ {score} {accuracy}%";
    }

    private Sprite LoadStreamedSprite(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogWarning("Banner not found: " + filePath);
            return null;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
        texture.LoadImage(fileData);

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
