using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TypeOfEngineBuild
{
    CustomBuild,
    Dreamwave
}

public class DreamwaveBootstrapper : MonoBehaviour
{
    [Header("Bootstrap Setup")]
    [SerializeField] private bool _completedBootstrap = false;
    [SerializeField] private TypeOfEngineBuild _typeOfEngineBuild;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _bootstrapProgressText;

    #region References

    [SerializeField] private GameObject _discordRpc;
    
    #endregion

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Bootstrap.txt");

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split("=");

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (int.TryParse(value, out int valueStr))
                        {
                            SetupGameSettings(key, valueStr);
                        }
                    }
                }
            }

            _completedBootstrap = true;
        }
    }

    private void SetupGameSettings(string key, int value)
    {
        switch (key)
        {
            case "isCustomEngine":
                if (value == 1) _typeOfEngineBuild = TypeOfEngineBuild.CustomBuild;
                else _typeOfEngineBuild = TypeOfEngineBuild.Dreamwave;

                _bootstrapProgressText.text = $"Build Type: {_typeOfEngineBuild.ToString()}";

                StartCoroutine(InitEngineBuildType());
                break;
            case "useDiscordRpc":
                if (value == 1) _discordRpc.SetActive(true);
                else _discordRpc.SetActive(false);
                break;
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("firstTime")))
        {
            PlayerPrefs.SetInt("fps", 120);
            PlayerPrefs.SetFloat("ffps", 120);
            PlayerPrefs.SetInt("quality", 0);
            PlayerPrefs.SetString("chartPos", "upScroll");
            PlayerPrefs.SetInt("opponentEnabled", 1);
            PlayerPrefs.SetInt("ghostTapping", 1);
            PlayerPrefs.SetInt("freeAnimate", 0);
            PlayerPrefs.SetInt("incomingNoteWarning", 1);
            PlayerPrefs.SetInt("autoPause", 0);
            PlayerPrefs.SetInt("noteSplashes", 1);

            PlayerPrefs.SetString("firstTime", "False");
        }
    }

    private IEnumerator InitEngineBuildType()
    {
        yield return new WaitUntil(() => _completedBootstrap);
        
        yield return new WaitForSecondsRealtime(2f);

        _bootstrapProgressText.text = "Completed! Now loading...";

        yield return new WaitForSecondsRealtime(1f);

        switch (_typeOfEngineBuild)
        {
            case TypeOfEngineBuild.CustomBuild:
                SceneManager.LoadSceneAsync("CustomMenu");
                break;
            case TypeOfEngineBuild.Dreamwave:
                StartCoroutine(DreamwaveSceneLoad.IDreamwaveSceneLoad.LoadRoutine("Menu"));
                break;
        }
    }
}
