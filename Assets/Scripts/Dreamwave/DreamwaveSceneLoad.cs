using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DreamwaveSceneLoad : MonoBehaviour
{
    public static DreamwaveSceneLoad IDreamwaveSceneLoad;

    [SerializeField] private AudioSource _loadSource;
    private Animator _loadAnim;

    private void Awake()
    {
        if (IDreamwaveSceneLoad != null && IDreamwaveSceneLoad != this)
        {
            Destroy(gameObject);
            return;
        }

        IDreamwaveSceneLoad = this;
        DontDestroyOnLoad(gameObject);

        _loadAnim = GetComponent<Animator>();
        _loadSource.ignoreListenerPause = true;
    }

    public void Load(bool load)
    {
        if (load) _loadAnim.CrossFade("Load", 0.05f);
        else _loadAnim.CrossFade("Unload", 0.05f);
    }

    public bool Loaded = false;
    private string _loadingFrom;
    public IEnumerator LoadRoutine(string scene)
    {
        Loaded = false;
        Load(true);
        DiscordController.instance.UpdateState($"Loading", "");

        yield return new WaitForSecondsRealtime(2f);

        if (SceneManager.GetActiveScene().name == "MainScene" && scene == "MainScene")
        {
            yield return new WaitUntil(() => StrumManager.SM_Instance != null);
            yield return new WaitUntil(() => TempoManager.instance != null);

            yield return new WaitForSecondsRealtime(1f);

            yield return new WaitUntil(() => GameObject.FindObjectOfType<CustomAssetLoader>().CompletedLoading);
            yield return new WaitUntil(() => GameObject.FindObjectOfType<FanfareEvent>().InFanfare);
        }

        Debug.Log("Loading scene: " + scene);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
