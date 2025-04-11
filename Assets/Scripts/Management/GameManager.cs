using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Focus
{
    LeftPlayer,
    RightPlayer,
    Centre
}

public enum Ratings
{
    MFC,
    PFC,
    GFC,
    FC,
    SDCB,
    S,
    A,
    B,
    C,
    D,
    F
}

public enum PlayerCount
{
    One,
    Two
}

public class GameManager : MonoBehaviour
{
    [Header("Instancing")]
    [SerializeField] public static GameManager Instance;

    [Header("Scripts")]
    [SerializeField] public ScrollManager scrollManager;
    [SerializeField] private FanfareEvent _fanfareEventScript;
    [SerializeField] public DreamwaveVideoStreamer DreamwaveVideoStreamer;
    [SerializeField] public DreamwaveCharacter _playerScript;
    [SerializeField] public DreamwaveAICharacter _aiScript;

    [Header("States")]
    [SerializeField] public Focus focus;
    [SerializeField] public bool canSongStart = false;
    [SerializeField] public bool canGhostTap = true;
    [SerializeField] public bool canFreeAnimate = true;
    [SerializeField] public bool shouldDisplayIncomingNoteWarning = true;
    [SerializeField] public bool shouldAutoPause = true;
    [SerializeField] public bool shouldDrawNoteSplashes = true;
    [SerializeField] public bool BpmBump = true;

    [Header("Keybinds")]
    [SerializeField] public KeyCode left;
    [SerializeField] public KeyCode down;
    [SerializeField] public KeyCode up;
    [SerializeField] public KeyCode right;

    [Header("Game")]
    [SerializeField] public int health = 50;
    [SerializeField] public int score = 0;
    [SerializeField] public int combo = 0;
    [SerializeField] public int misses = 0;
    [SerializeField] public float accuracy = 100f;
    [SerializeField] public Ratings _playerRating;
    [SerializeField] public int[] scores;
    [SerializeField] public int totalNotes;
    [SerializeField] public int totalNotesHitCorrect;

    [Space(10)]
    [SerializeField] public int shits = 0;
    [SerializeField] public int bads = 0;
    [SerializeField] public int cools = 0;
    [SerializeField] public int sicks = 0;
    [SerializeField] public int dreamys = 0;
    [SerializeField] public int maxScore;

    [Header("Settings")]
    [SerializeField] public int SongStartStep = 0;
    [SerializeField] private Vector3 leftPlayerSideFocus;
    [SerializeField] private Vector3 rightPlayerSideFocus;
    [SerializeField] private Vector3 centreFocus;
    [SerializeField] private Vector3[] mobileFocus;
    [SerializeField] private string[] _unfocusedMessages;
    [SerializeField] public float CameraFovBumpOnBeatValue;
    [SerializeField] public float CameraFov;
    [SerializeField] public float CameraFovSpeed;

    [Header("DiscordRPC Settings")]
    [SerializeField] public string SongName;
    [SerializeField] public string SongCreatorName;
    [SerializeField] public string SongImageName;
    [SerializeField] public float SongDuration;
    [SerializeField] public float SongPlaybackPosition;

    [Header("GameObjects")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject _gameUnfocusedUi;
    [SerializeField] private GameObject noteUi;
    [SerializeField] public GameObject noteUiSidePlayer;
    [SerializeField] public GameObject noteUiSidePlayerMiddle;
    [SerializeField] public GameObject noteUiSideOpponent;
    [SerializeField] public GameObject noteUiSideOpponentMiddle;
    [SerializeField] private GameObject noteRendererPlayerMobile;
    [SerializeField] private GameObject noteRendererPlayer;
    [SerializeField] private GameObject noteRendererOpponent;

    [Header("Shaders")]
    [SerializeField] private Material psychShader;
    [SerializeField] private float psychShaderSmoothing = 5f;
    [SerializeField] private float noteUiSmoothing = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _gameUnfocusedText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI missesText;
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI ffpsText;
    [SerializeField] private TextMeshProUGUI bpmText;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private TextMeshProUGUI overallStepText;
    [SerializeField] private Slider _healthSlider;

    [Header("Animation")]
    [SerializeField] private Animator _healthSliderAnim;

    [Header("Prefabs")]
    [SerializeField] public GameObject[] ratings;

    private void Awake()
    {
        Instance = this;
        FanfareStart();
    }

    public void FanfareStart()
    {
        canSongStart = false;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void FanfareEnd()
    {
        canSongStart = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Start()
    {
        psychShader.SetFloat("_Scale", -2.8f);
#if !UNITY_EDITOR
        DiscordController.instance.largeImage = SongImageName;
#endif
        SongPlaybackPosition = (float)AudioSettings.dspTime;
        SongDuration = TempoManager.instance.audioSource.clip.length;

        DreamwaveVideoStreamer = GameObject.Find("Dreamwave Video").GetComponent<DreamwaveVideoStreamer>();
        
        var allNotes = GameObject.FindGameObjectsWithTag("Note");
        for (int i = 0; i < allNotes.Length; i++)
        {
            totalNotes++;
        }

        health = 50;
        accuracy = 100.00f;
        CalculatePlayerRating();
        ConfigureDeviceSettings();
    }

    private void ConfigureDeviceSettings()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                noteUiSideOpponent.SetActive(false);
                noteUiSidePlayer.SetActive(false);
                noteRendererOpponent.SetActive(false);
                noteRendererPlayerMobile.SetActive(true);
                break;
            case RuntimePlatform.WindowsPlayer:
                noteUiSideOpponent.SetActive(true);
                noteRendererPlayerMobile.SetActive(false);
                break;
            default:
                noteUiSideOpponent.SetActive(true);
                noteRendererPlayerMobile.SetActive(false);
                break;
        }
    }

    private void Update()
    {
        fpsText.text = "Fps - " + (1.0f / Time.unscaledDeltaTime).ToString("F0");
        ffpsText.text = "FFps - " + PlayerPrefs.GetFloat("ffps").ToString("F0");
        bpmText.text = "Bpm - " + TempoManager.instance.beatsPerMinute.ToString();
        health = Mathf.Clamp(health, 0, 101);
        _healthSlider.value = Mathf.Lerp(_healthSlider.value, health, 10f * Time.deltaTime);

        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, CameraFov, Time.deltaTime * CameraFovSpeed);

        if (PauseMenu.instance._isPaused) return;
        AnimateBackground();
        PositionNoteUi();
        ShouldDisplayHealthSlider();

        if (TempoManager.instance.audioSource.clip != null &&
        TempoManager.instance.audioSource.time >= TempoManager.instance.audioSource.clip.length - 0.1f)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void LateUpdate() => PreviousHealthValue();

    private void OnEnable()
    {
        NoteHitbox.NoteHit += OnNoteHit;
        TempoManager.OnStep += OnStepHandler;
    }

    private void OnDisable()
    {
        NoteHitbox.NoteHit -= OnNoteHit;
        TempoManager.OnStep -= OnStepHandler;
    }

    private void OnDestroy()
    {
        NoteHitbox.NoteHit -= OnNoteHit;
        TempoManager.OnStep -= OnStepHandler;
    }

    private int totalHits = 0;

    private void OnNoteHit(string newScore, float msDelay, float noteDistance, string direction)
    {
        int hitScore = 0;
        float hitAccuracy = 100.0f; // Start with a perfect hit score for accuracy weighting

        switch (newScore)
        {
            case "Missed":
                hitScore = scores[5];
                misses++;
                combo = 0;
                shits++;
                hitAccuracy = 0.0f;

                var missed = Instantiate(ratings[5], ratings[5].transform.position, Quaternion.identity);
                missed.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MS: " + msDelay.ToString("F4");
                missed.transform.SetParent(canvas.transform);
                missed.transform.localScale = new Vector3(0.1338828f, 0.1338828f, 0.1338828f);
                missed.SetActive(true);
                break;
            case "Shit":
                hitScore = scores[4];
                combo = 0;
                shits++;
                hitAccuracy = 40.0f;

                var shit = Instantiate(ratings[4], ratings[4].transform.position, Quaternion.identity);
                shit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MS: " + msDelay.ToString("F4");
                shit.transform.SetParent(canvas.transform);
                shit.transform.localScale = new Vector3(0.1338828f, 0.1338828f, 0.1338828f);
                shit.SetActive(true);
                break;
            case "Bad":
                hitScore = scores[3];
                combo++;
                bads++;
                hitAccuracy = 60.0f;

                var bad = Instantiate(ratings[3], ratings[3].transform.position, Quaternion.identity);
                bad.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MS: " + msDelay.ToString("F4");
                bad.transform.SetParent(canvas.transform);
                bad.transform.localScale = new Vector3(0.1338828f, 0.1338828f, 0.1338828f);
                bad.SetActive(true);
                break;
            case "Cool":
                hitScore = scores[2];
                combo++;
                cools++;
                totalNotesHitCorrect++;
                hitAccuracy = 80.0f;

                var cool = Instantiate(ratings[2], ratings[2].transform.position, Quaternion.identity);
                cool.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MS: " + msDelay.ToString("F4");
                cool.transform.SetParent(canvas.transform);
                cool.transform.localScale = new Vector3(0.1338828f, 0.1338828f, 0.1338828f);
                cool.SetActive(true);
                break;
            case "Sick":
                hitScore = scores[1];
                combo++;
                sicks++;
                totalNotesHitCorrect++;
                hitAccuracy = 90.0f;

                var sick = Instantiate(ratings[1], ratings[1].transform.position, Quaternion.identity);
                sick.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MS: " + msDelay.ToString("F4");
                sick.transform.SetParent(canvas.transform);
                sick.transform.localScale = new Vector3(0.1338828f, 0.1338828f, 0.1338828f);
                sick.SetActive(true);
                break;
            case "Dreamy":
                hitScore = scores[0];
                combo++;
                dreamys++;
                totalNotesHitCorrect++;
                hitAccuracy = 100.0f;

                var dreamy = Instantiate(ratings[0], ratings[0].transform.position, Quaternion.identity);
                dreamy.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MS: " + msDelay.ToString("F4");
                dreamy.transform.SetParent(canvas.transform);
                dreamy.transform.localScale = new Vector3(0.1338828f, 0.1338828f, 0.1338828f);
                dreamy.SetActive(true);
                break;
        }

        // Adjust score and health
        score += hitScore;
        UpdatePlayerHealth(hitScore > 0 ? 2 : -5);

        // Update total hits and calculate weighted accuracy
        totalHits++;
        accuracy = ((accuracy * (totalHits - 1)) + hitAccuracy) / totalHits;
        accuracy = Mathf.Clamp(accuracy, 0f, 100f);

        // Update UI
        comboText.text = "Combo: " + combo.ToString();
        missesText.text = "Misses: " + misses.ToString();
        accuracyText.text = "Accuracy: " + accuracy.ToString("F0") + "%";
        scoreText.text = "Score: " + score.ToString();

        CalculatePlayerRating();
    }

    public void CalculatePlayerRating()
    {
        if (misses == 0 && dreamys > 0 && sicks == 0 && cools == 0 && bads == 0 && shits == 0)
            _playerRating = Ratings.MFC; // Marvelous Full Combo
        else if (misses == 0 && sicks > 0 && cools == 0 && bads == 0 && shits == 0)
            _playerRating = Ratings.PFC; // Perfect Full Combo
        else if (misses == 0 && cools > 0 && bads == 0 && shits == 0)
            _playerRating = Ratings.GFC; // Good Full Combo
        else if (misses == 0 && (bads > 0 || shits > 0))
            _playerRating = Ratings.FC; // Full Combo
        else if (misses >= 9)
            _playerRating = Ratings.SDCB; // Single Digit Combo Break
        else if (accuracy >= 80f)
            _playerRating = Ratings.S;
        else if (accuracy >= 70f)
            _playerRating = Ratings.A;
        else if (accuracy >= 60f)
            _playerRating = Ratings.B;
        else if (accuracy >= 50f)
            _playerRating = Ratings.C;
        else if (accuracy >= 40f)
            _playerRating = Ratings.D;
        else
            _playerRating = Ratings.F;

        ratingText.text = "Rating: " + _playerRating.ToString();
    }

    #region Player Health

    private int _lastHealthValue;
    private bool _runningRoutine = false;
    private void ShouldDisplayHealthSlider()
    {
        if (!_runningRoutine && _lastHealthValue != health)
        {
            StartCoroutine(SwitchHealthSliderState());
            _runningRoutine = true;
        }
    }
    private void PreviousHealthValue() => _lastHealthValue = health;

    private IEnumerator SwitchHealthSliderState()
    {
        _runningRoutine = true;
        _healthSliderAnim.CrossFade("In", 0.1f);

        yield return new WaitUntil(() => _lastHealthValue == health);
        yield return new WaitForSecondsRealtime(5f);

        _healthSliderAnim.CrossFade("Out", 0.1f);
        _runningRoutine = false;
    }
    private void UpdatePlayerHealth(int healthToChange)
    {
        health += healthToChange;
    }


    #endregion

    private void PositionNoteUi()
    {
        //noteUi.transform.localPosition = mobileFocus[0];

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                noteUi.transform.localPosition = mobileFocus[0];
                break;
            case RuntimePlatform.WindowsPlayer:
                switch (focus)
                {
                    case Focus.LeftPlayer:
                        noteUi.transform.localPosition = Vector3.Lerp(noteUi.transform.localPosition, leftPlayerSideFocus, noteUiSmoothing * Time.deltaTime);
                        break;
                    case Focus.RightPlayer:
                        noteUi.transform.localPosition = Vector3.Lerp(noteUi.transform.localPosition, rightPlayerSideFocus, noteUiSmoothing * Time.deltaTime);
                        break;
                    case Focus.Centre:
                        noteUi.transform.localPosition = Vector3.Lerp(noteUi.transform.localPosition, centreFocus, noteUiSmoothing * Time.deltaTime);
                        break;
                }
                break;
            default:
                switch (focus)
                {
                    case Focus.LeftPlayer:
                        noteUi.transform.localPosition = Vector3.Lerp(noteUi.transform.localPosition, leftPlayerSideFocus, noteUiSmoothing * Time.deltaTime);
                        break;
                    case Focus.RightPlayer:
                        noteUi.transform.localPosition = Vector3.Lerp(noteUi.transform.localPosition, rightPlayerSideFocus, noteUiSmoothing * Time.deltaTime);
                        break;
                    case Focus.Centre:
                        noteUi.transform.localPosition = Vector3.Lerp(noteUi.transform.localPosition, centreFocus, noteUiSmoothing * Time.deltaTime);
                        break;
                }
                break;
        }
    }

    private void AnimateBackground()
    {
        /*switch (onBeat)
        {
            case true:
                psychShader.SetFloat("_WarpScale", Mathf.Lerp(psychShader.GetFloat("_WarpScale"), -20f, psychShaderSmoothing * Time.deltaTime));
                //psychShader.SetFloat("_RotationAngle", Mathf.Lerp(psychShader.GetFloat("_RotationAngle"), -10f * Mathf.Deg2Rad, psychShaderSmoothing * Time.deltaTime));
                break;
            case false:
                psychShader.SetFloat("_WarpScale", Mathf.Lerp(psychShader.GetFloat("_WarpScale"), 20f, psychShaderSmoothing * Time.deltaTime));
                //psychShader.SetFloat("_RotationAngle", Mathf.Lerp(psychShader.GetFloat("_RotationAngle"), 10f * Mathf.Deg2Rad, psychShaderSmoothing * Time.deltaTime));
                break;
        }*/
    }

    private bool onBeat = false;
    public bool start = false;
    public int stepCount = 0;
    private void OnStepHandler(int step) 
    {
        stepCount++;
        //Debug.Log(stepCount);
        if (stepCount >= SongStartStep) start = true;

        switch (onBeat)
        {
            case true:
                if (step == 1) onBeat = false;
                break;
            case false:
                if (step == 1) onBeat = true;
                break;
        }

        if (BpmBump)
        {
            if (step == 1)
            {
                StopCoroutine(StepCameraBop());
                StartCoroutine(StepCameraBop());
            }
            else if (step == 3 && TempoManager.instance.beatsPerMinute <= 200)
            {
                StopCoroutine(StepCameraBop());
                StartCoroutine(StepCameraBop());
            }
        }

        stepText.text = "Step - " + step.ToString();
        overallStepText.text = "Steps - " + stepCount.ToString();
    }

    private IEnumerator StepCameraBop()
    {
        CameraFov -= CameraFovBumpOnBeatValue * 2;

        yield return new WaitForSecondsRealtime(0.05f);

        CameraFov += CameraFovBumpOnBeatValue * 2;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (shouldAutoPause && !PauseMenu.instance._isPaused) StartCoroutine(OnUnfocus(focus));
    }

    private IEnumerator OnUnfocus(bool focus)
    {
        yield return new WaitUntil(() => !_fanfareEventScript.InFanfare);

        switch (focus)
        {
            case false:
                Time.timeScale = 0;
                AudioListener.pause = true;
                _gameUnfocusedText.text = _unfocusedMessages[Random.Range(0, _unfocusedMessages.Length)];
                _gameUnfocusedUi.SetActive(true);
                break;
            case true:
                Time.timeScale = 1;
                AudioListener.pause = false;
                _gameUnfocusedUi.SetActive(false);
                _fanfareEventScript.StopSong();
                break;
        }

        yield break;
    }

    public void ResetSettings()
    {
        Time.fixedDeltaTime = 1f / 60f;
        PlayerPrefs.SetInt("_preferredFixedFrameRate", 60);
    }
}
