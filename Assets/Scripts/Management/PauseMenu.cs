using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static DreamwaveGlobal;

public class PauseMenu : MonoBehaviour
{
    #region Variables

    [Header("Instancing")]
    [SerializeField] public static PauseMenu instance;

    [Header("States")]
    [SerializeField] public bool _isPaused = false;
    [SerializeField] public bool _inSettings = false;
    [SerializeField] public bool _inModEditor = false;

    [Header("GameObjects")]
    [SerializeField] private GameObject _pauseObj;
    [SerializeField] private GameObject _settingsObj;

    [Header("Scripts")]
    [SerializeField] private Settings _settings;

    [Header("Animation")]
    [SerializeField] private Animator _pauseAnim;

    [Header("Audio")]
    [SerializeField] public AudioSource _pauseAudioSource;

    public delegate void PauseCallback(bool pausedState);
    public static event PauseCallback Pause;

    #endregion

    #region Processes

    private void Awake() => instance = this;
    private void OnDisable() => instance = null;
    private void OnDestroy() => instance = null;

    private void Start() => _pauseAudioSource.ignoreListenerPause = true;

    void Update()
    {
        if (Input.GetKeyDown(_settings.back))
        {
            if (_inSettings)
            {
                CloseSettings();
            }
            else if (_inModEditor)
            {
                CloseModEditor();
            }
            else
            {
                PauseInput();
            }
        }
    }

    #endregion

    #region Pause Input and Applying certain states

    private void PauseInput()
    {
        switch (_isPaused)
        {
            case false:
                PauseGame();
                break;
            case true:
                UnpauseGame();
                break;
        }
    }

    #endregion

    #region Pausing and Unpausing game

    private void PauseGame()
    {
        if (!GameManager.Instance.canSongStart) return;

        AudioListener.pause = true;
        _pauseAnim.CrossFade("Paused", 0.1f);
        _isPaused = true;
        _pauseObj.SetActive(true);
        _settingsObj.SetActive(false);
        Time.timeScale = 0f;
        Pause(true);
        DiscordController.instance.UpdateState($"Song: {LoadedModSong.name} ({LoadedModSong.songDifficulty}) ~ [By: {LoadedModSong.creator}]", "Paused");
    }

    public void UnpauseGame()
    {
        AudioListener.pause = false;
        _pauseAnim.CrossFade("Unpaused", 0.1f);
        _isPaused = false;
        _pauseObj.SetActive(false);
        _settingsObj.SetActive(false);
        Time.timeScale = 1f;
        Pause(false);
        DiscordController.instance.UpdateState($"Song: {LoadedModSong.name} ({LoadedModSong.songDifficulty}) ~ [By: {LoadedModSong.creator}]", "Slapping their keyboard");
    }

    #endregion

    #region Opening and Closing menus

    public void OpenSettings()
    {
        if (!_isPaused) return;

        _pauseAnim.CrossFade("OpenSettings", 0.1f);
        _inSettings = true;
        DiscordController.instance.UpdateState($"Song: {LoadedModSong.name} ({LoadedModSong.songDifficulty}) ~ [By: {LoadedModSong.creator}]", "In the Settings");
    }

    public void CloseSettings()
    {
        _pauseAnim.CrossFade("CloseSettings", 0.1f);
        _inSettings = false;
        DiscordController.instance.UpdateState($"Song: {LoadedModSong.name} ({LoadedModSong.songDifficulty}) ~ [By: {LoadedModSong.creator}]", "Paused");
    }

    public void OpenModEditor()
    {
        _pauseAnim.CrossFade("OpenModEditor", 0.1f);
        _inModEditor = true;
        DiscordController.instance.UpdateState($"Song: {LoadedModSong.name} ({LoadedModSong.songDifficulty}) ~ [By: {LoadedModSong.creator}]", "In the Mod Editor");
    }

    public void CloseModEditor()
    {
        _pauseAnim.CrossFade("CloseModEditor", 0.1f);
        _inModEditor = false;
        DiscordController.instance.UpdateState($"Song: {LoadedModSong.name} ({LoadedModSong.songDifficulty}) ~ [By: {LoadedModSong.creator}]", "Paused");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        _pauseAudioSource.mute = true;
        StartCoroutine(DreamwaveSceneLoad.IDreamwaveSceneLoad.LoadRoutine("Menu"));
    }

    #endregion
}
