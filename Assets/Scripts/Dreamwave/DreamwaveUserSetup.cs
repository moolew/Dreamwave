using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamwaveUserSetup : MonoBehaviour
{
    public static DreamwaveUserSetup instance;

    [SerializeField] private NoteHitbox[] _playerNoteHitbox;

    private void Awake() => instance = this;
    
    private void Start()
    {
        LoadUserSettings();
    }

    public void LoadUserSettings()
    {
        SetPlayerFps();
        SetPlayerGraphicSettings();
        CheckPlayerNoteRenderPreferences();
        ShouldOpponentNotesRender();
        ShouldAllowGhostTapping();
        ShouldAllowFreeAnimation();
        EnableIncomingNoteWarning();
        ShouldAllowAutoPause();
        ShouldAllowNoteSplashes();
        UpdateUserKeybinds();
    }

    private void SetPlayerFps()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt("fps");
        //Time.fixedDeltaTime = (1.0f / PlayerPrefs.GetFloat("ffps")); - I used to use Fixed time for chart scrolling
    }

    private void CheckPlayerNoteRenderPreferences()
    {
        if (PlayerPrefs.GetString("chartPos") == "middleScroll")
        {
            GameManager.Instance.noteUiSidePlayer.SetActive(false);
            GameManager.Instance.noteUiSideOpponent.SetActive(false);
            GameManager.Instance.noteUiSidePlayerMiddle.SetActive(true);
            GameManager.Instance.noteUiSideOpponentMiddle.SetActive(true);
            GameManager.Instance.noteUiSidePlayerMiddleDown.SetActive(false);
            GameManager.Instance.noteUiSideOpponentMiddleDown.SetActive(false);
        }
        else if (PlayerPrefs.GetString("chartPos") == "middleBottomScroll")
        {
            GameManager.Instance.noteUiSidePlayer.SetActive(false);
            GameManager.Instance.noteUiSideOpponent.SetActive(false);
            GameManager.Instance.noteUiSidePlayerMiddle.SetActive(false);
            GameManager.Instance.noteUiSideOpponentMiddle.SetActive(false);
            GameManager.Instance.noteUiSidePlayerMiddleDown.SetActive(true);
            GameManager.Instance.noteUiSideOpponentMiddleDown.SetActive(true);
        }
        else if (PlayerPrefs.GetString("chartPos") == "downScroll")
        {
            GameManager.Instance.noteUiSidePlayer.transform.rotation = Quaternion.Euler(180f, 0f, 0);
            GameManager.Instance.noteUiSideOpponent.transform.rotation = Quaternion.Euler(180f, 0f, 0);
            GameManager.Instance.noteUiSidePlayer.SetActive(true);
            GameManager.Instance.noteUiSideOpponent.SetActive(true);
            GameManager.Instance.noteUiSidePlayerMiddle.SetActive(false);
            GameManager.Instance.noteUiSideOpponentMiddle.SetActive(false);
            GameManager.Instance.noteUiSidePlayerMiddleDown.SetActive(false);
            GameManager.Instance.noteUiSideOpponentMiddleDown.SetActive(false);
        }
        else if (PlayerPrefs.GetString("chartPos") == "upScroll")
        {
            GameManager.Instance.noteUiSidePlayer.transform.rotation = Quaternion.Euler(0f, 0, 0);
            GameManager.Instance.noteUiSideOpponent.transform.rotation = Quaternion.Euler(0f, 0, 0);
            GameManager.Instance.noteUiSidePlayer.SetActive(true);
            GameManager.Instance.noteUiSideOpponent.SetActive(true);
            GameManager.Instance.noteUiSidePlayerMiddle.SetActive(false);
            GameManager.Instance.noteUiSideOpponentMiddle.SetActive(false);
            GameManager.Instance.noteUiSidePlayerMiddleDown.SetActive(false);
            GameManager.Instance.noteUiSideOpponentMiddleDown.SetActive(false);
        }
    }

    private void SetPlayerGraphicSettings()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));
    }

    private void ShouldOpponentNotesRender()
    {
        if (PlayerPrefs.GetString("chartPos") == "middleScroll" | PlayerPrefs.GetString("chartPos") == "middleBottomScroll") return;

        var _pref = PlayerPrefs.GetInt("opponentEnabled");

        if (_pref == 1) GameManager.Instance.noteUiSideOpponent.SetActive(true);
        else GameManager.Instance.noteUiSideOpponent.SetActive(false);
    }

    private void ShouldAllowGhostTapping()
    {
        var _pref = PlayerPrefs.GetInt("ghostTapping");

        if (_pref == 1) GameManager.Instance.canGhostTap = true;
        else GameManager.Instance.canGhostTap = false;
    }

    private void ShouldAllowFreeAnimation()
    {
        var _pref = PlayerPrefs.GetInt("freeAnimate");

        if (_pref == 1) GameManager.Instance.canFreeAnimate = true;
        else GameManager.Instance.canFreeAnimate = false;
    }

    private void EnableIncomingNoteWarning()
    {
        var _pref = PlayerPrefs.GetInt("incomingNoteWarning");

        if (_pref == 1) GameManager.Instance.shouldDisplayIncomingNoteWarning = true;
        else GameManager.Instance.shouldDisplayIncomingNoteWarning = false;
    }

    private void ShouldAllowAutoPause()
    {
        var _pref = PlayerPrefs.GetInt("autoPause");

        if (_pref == 1) GameManager.Instance.shouldAutoPause = true;
        else GameManager.Instance.shouldAutoPause = false;
    }

    private void ShouldAllowNoteSplashes()
    {
        var _pref = PlayerPrefs.GetInt("noteSplashes");

        if (_pref == 1) GameManager.Instance.shouldDrawNoteSplashes = true;
        else GameManager.Instance.shouldDrawNoteSplashes = false;
    }

    private void UpdateUserKeybinds()
    {
        _playerNoteHitbox[0].keyForSide = GameManager.Instance.left;
        _playerNoteHitbox[1].keyForSide = GameManager.Instance.down;
        _playerNoteHitbox[2].keyForSide = GameManager.Instance.up;
        _playerNoteHitbox[3].keyForSide = GameManager.Instance.right;
    }

    void Update()
    {
        
    }
}
