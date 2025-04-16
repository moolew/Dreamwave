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
        DontDestroyOnLoad(gameObject);
        IDreamwaveSceneLoad = this;
        _loadAnim = GetComponent<Animator>();
        _loadSource.ignoreListenerPause = true;
    }

    public void Load(bool load)
    {
        if (load) _loadAnim.CrossFade("Load", 0.05f);
        else _loadAnim.CrossFade("Unload", 0.05f);
    }

    private bool _sceneLoaded;
    private string _loadingFrom;
    public IEnumerator LoadRoutine(string scene)
    {
        _sceneLoaded = false;
        Load(true);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(scene);

        yield return null;

        yield return new WaitUntil(() => _sceneLoaded);

        yield return new WaitForSeconds(1f);

        Load(false);
        yield return new WaitForSeconds(0.35f);

        GameManager.Instance._fanfareEventScript.StartCoroutine("WaitForCooldown");

        yield return new WaitForSeconds(1f);
    }
}
