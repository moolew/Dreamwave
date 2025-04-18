using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DreamwaveSceneLoad;

public class FanfareEvent : MonoBehaviour
{
    [SerializeField] private Animator _fanfareAnim;
    [SerializeField] private AudioSource _fanfareSource;
    [SerializeField] private AudioClip[] _fanfareClips;
    [SerializeField] private CanvasGroup _gameUI;
    [SerializeField] private TextMeshProUGUI _songNameText;
    [SerializeField] private TextMeshProUGUI _songCreatorText;
    [SerializeField] private TextMeshProUGUI _songDurationText;
    public bool InFanfare = false;
    public bool CanStart = false;

    private void Awake()
    {
        _fanfareSource.ignoreListenerPause = true;
    }

    private void Start()
    {
        _songNameText.text = $"Song: {GameManager.Instance.SongName}";
        _songCreatorText.text = $"By: {GameManager.Instance.SongCreatorName}";

        float songDuration = TempoManager.instance.audioSource.clip.length;
        int totalSecs = Mathf.FloorToInt(songDuration);
        int mins = totalSecs / 60;
        int secs = totalSecs % 60;
        string timeStamp = string.Format("{0}:{1:00}", mins, secs);
        _songDurationText.text = $"Time: {timeStamp}";
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoaded;
    }

    public void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine("WaitForCooldown");
    }

    public IEnumerator WaitForCooldown()
    {
        CanStart = true;
        yield return new WaitForSecondsRealtime(0.5f);

        _fanfareAnim.SetTrigger("loaded");
        IDreamwaveSceneLoad.Load(false);

        while (_gameUI.alpha != 1)
        {
            _gameUI.alpha = Mathf.Lerp(_gameUI.alpha, 1, 2f * Time.deltaTime);

            if (_gameUI.alpha == 1) break; // saftey

            yield return null;
        }

        yield break;
    }

    public void StartSong()
    {
        InFanfare = false;
        firstShow = false;
        GameManager.Instance.FanfareEnd();
    }

    bool firstShow = true;
    public void StopSong()
    {
        InFanfare = true;
        if (!firstShow)
        {
            _songNameText.gameObject.SetActive(false);
            _songCreatorText.gameObject.SetActive(false);
            _songDurationText.gameObject.SetActive(false);
        }
        GameManager.Instance.FanfareStart();
        _fanfareAnim.ResetTrigger("loaded");
        _fanfareAnim.SetTrigger("loaded");
    }

    public void Ready()
    {
        _fanfareSource.PlayOneShot(_fanfareClips[0]);
    }

    public void Set()
    {
        _fanfareSource.PlayOneShot(_fanfareClips[1]);
    }

    public void Go()
    {
        _fanfareSource.PlayOneShot(_fanfareClips[2]);
    }
}
