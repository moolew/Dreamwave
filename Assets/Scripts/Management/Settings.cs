using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour
{
    #region Variables

    public static Settings instance;

    [Header("Player Preferences")]
    #region UI Navigation
    public KeyCode back;
    public KeyCode navUp;
    public KeyCode navDown;
    public KeyCode navLeft;
    public KeyCode navRight;
    #endregion
    #region Gameplay
    public KeyCode gameUp;
    public KeyCode gameDown;
    public KeyCode gameLeft;
    public KeyCode gameRight;
    #endregion

    [Header("GameObjects")]
    [SerializeField] private GameObject _settingsObj;

    [Header("Scripts")]
    [SerializeField] private ChunkScroller _chunkScroller;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI[] settingsTextAssets;

    #endregion

    private void OnEnable() => instance = this;
    private void OnDisable() => instance = null;
    private void OnDestroy() => instance = null;

    private void Awake()
    {
        PlayerPrefs.SetInt("fps", -1);
        localFps = PlayerPrefs.GetInt("fps");
        localFfps = PlayerPrefs.GetFloat("ffps");
        _quality = PlayerPrefs.GetInt("quality");
        _chartPos = PlayerPrefs.GetString("chartPos");
        _opponentNotesEnabled = PlayerPrefs.GetInt("opponentEnabled");
        _ghostTapping = PlayerPrefs.GetInt("ghostTapping");
        _freeAnimate = PlayerPrefs.GetInt("freeAnimate");
        _incomingNoteWarning = PlayerPrefs.GetInt("incomingNoteWarning");
        _autoPause = PlayerPrefs.GetInt("autoPause");
        _noteSplashes = PlayerPrefs.GetInt("noteSplashes");

        settingsTextAssets[0].text = localFps.ToString("F0");
        settingsTextAssets[1].text = localFfps.ToString();

        #region Setup Switches

        switch (_quality)
        {
            case 0:
                settingsTextAssets[2].text = "High";
                break;
            case 1:
                settingsTextAssets[2].text = "Medium";
                break;
            case 2:
                settingsTextAssets[2].text = "Low";
                break;
        }

        switch (_chartPos)
        {
            case "upScroll":
                _chartInt = 0;
                settingsTextAssets[3].text = "Up Scroll";
                break;
            case "downScroll":
                _chartInt = 1;
                settingsTextAssets[3].text = "Down Scroll";
                break;
            case "middleScroll":
                _chartInt = 2;
                settingsTextAssets[3].text = "Middle Top Scroll";
                break;
            case "middleBottomScroll":
                _chartInt = 3;
                settingsTextAssets[3].text = "Middle Bottom Scroll";
                break;
        }

        switch (_opponentNotesEnabled)
        {
            case 0:
                settingsTextAssets[4].text = "False";
                break;
            case 1:
                settingsTextAssets[4].text = "True";
                break;
        }

        switch (_ghostTapping)
        {
            case 0:
                settingsTextAssets[5].text = "False";
                break;
            case 1:
                settingsTextAssets[5].text = "True";
                break;
        }

        switch (_freeAnimate)
        {
            case 0:
                settingsTextAssets[6].text = "False";
                break;
            case 1:
                settingsTextAssets[6].text = "True";
                break;
        }

        switch (_incomingNoteWarning)
        {
            case 0:
                settingsTextAssets[7].text = "False";
                break;
            case 1:
                settingsTextAssets[7].text = "True";
                break;
        }

        switch (_autoPause)
        {
            case 0:
                settingsTextAssets[8].text = "False";
                break;
            case 1:
                settingsTextAssets[8].text = "True";
                break;
        }

        switch (_noteSplashes)
        {
            case 0:
                settingsTextAssets[9].text = "False";
                break;
            case 1:
                settingsTextAssets[9].text = "True";
                break;
        }

        #endregion
    }

    private void Update()
    {
        if (!PauseMenu.instance._inModEditor && PauseMenu.instance._isPaused && PauseMenu.instance._inSettings) SwitchScrollIndex();
    }

    private void SwitchScrollIndex()
    {
        switch (_chunkScroller._scrollIndex)
        {
            case 0:
                UpdateSavedFrameRate();
                break;
            case 1:
                UpdateSavedFixedFrameRate();
                break;
            case 2:
                SwitchGraphicsProfile();
                break;
            case 3:
                SwitchChartPlacement();
                break;
            case 4:
                SwitchOpponentNotesEnabled();
                break;
            case 5:
                EnableGhostTapping();
                break;
            case 6:
                EnableFreeAnimate();
                break;
            case 7:
                EnableIncomingNoteWarning();
                break;
            case 8:
                EnableAutoPause();
                break;
            case 9:
                EnableNoteSplashes();
                break;
        }
    }

    #region Frame Rate Setting

    private int localFps;
    public void UpdateSavedFrameRate()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (localFps > 30) localFps -= 10;
            Application.targetFrameRate = localFps;
            settingsTextAssets[0].text = localFps.ToString("F0");
            PlayerPrefs.SetInt("fps", localFps);
            PlayerPrefs.Save();
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (localFps < 990) localFps += 10;
            Application.targetFrameRate = localFps;
            settingsTextAssets[0].text = localFps.ToString("F0");
            PlayerPrefs.SetInt("fps", localFps);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Fixed Frame Rate Setting

    public float localFfps;
    public void UpdateSavedFixedFrameRate()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (localFfps > 30) localFfps -= 10;
            //Time.fixedDeltaTime = (1 / localFfps); - I used to use Fixed time for chart scrolling
            settingsTextAssets[1].text = localFfps.ToString("F0");
            PlayerPrefs.SetFloat("ffps", localFfps);
            PlayerPrefs.Save();
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (localFfps < 240) localFfps += 10;
            //Time.fixedDeltaTime = (1 / localFfps); - I used to use Fixed time for chart scrolling
            settingsTextAssets[1].text = localFfps.ToString("F0");
            PlayerPrefs.SetFloat("ffps", localFfps);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Graphics Section

    private int _quality = 0;
    public void SwitchGraphicsProfile()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_quality > 0) _quality--;
            QualitySettings.SetQualityLevel(_quality);

            switch (_quality)
            {
                case 0:
                    settingsTextAssets[2].text = "High";
                    break;
                case 1:
                    settingsTextAssets[2].text = "Medium";
                    break;
                case 2:
                    settingsTextAssets[2].text = "Low";
                    break;
            }

            SaveSettingsOfBool("quality", _quality);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_quality < 2) _quality++;
            QualitySettings.SetQualityLevel(_quality);

            switch (_quality)
            {
                case 0:
                    settingsTextAssets[2].text = "High";
                    break;
                case 1:
                    settingsTextAssets[2].text = "Medium";
                    break;
                case 2:
                    settingsTextAssets[2].text = "Low";
                    break;
            }

            SaveSettingsOfBool("quality", _quality);
        }
    }

    #endregion

    #region Chart Placement Section

    private string _chartPos;
    private int _chartInt;
    public void SwitchChartPlacement()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_chartInt > 0) _chartInt--;

            switch (_chartInt)
            {
                case 0:
                    _chartPos = "upScroll";
                    settingsTextAssets[3].text = "Up Scroll";
                    break;
                case 1:
                    _chartPos = "downScroll";
                    settingsTextAssets[3].text = "Down Scroll";
                    break;
                case 2:
                    _chartPos = "middleScroll";
                    settingsTextAssets[3].text = "Middle Top Scroll";
                    break;
                case 3:
                    _chartPos = "middleBottomScroll";
                    settingsTextAssets[3].text = "Middle Bottom Scroll";
                    break;
            }

            PlayerPrefs.SetString("chartPos", _chartPos);
            PlayerPrefs.Save();
            DreamwaveUserSetup.instance.LoadUserSettings();
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_chartInt < 3) _chartInt++;

            switch (_chartInt)
            {
                case 0:
                    _chartPos = "upScroll";
                    settingsTextAssets[3].text = "Up Scroll";
                    break;
                case 1:
                    _chartPos = "downScroll";
                    settingsTextAssets[3].text = "Down Scroll";
                    break;
                case 2:
                    _chartPos = "middleScroll";
                    settingsTextAssets[3].text = "Middle Top Scroll";
                    break;
                case 3:
                    _chartPos = "middleBottomScroll";
                    settingsTextAssets[3].text = "Middle Bottom Scroll";
                    break;
            }

            PlayerPrefs.SetString("chartPos", _chartPos);
            PlayerPrefs.Save();
            DreamwaveUserSetup.instance.LoadUserSettings();
        }
    }

    #endregion

    #region Opponent Notes Section

    private int _opponentNotesEnabled;
    public void SwitchOpponentNotesEnabled()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_opponentNotesEnabled > 0) _opponentNotesEnabled--;

            switch (_opponentNotesEnabled)
            {
                case 0:
                    settingsTextAssets[4].text = "False";
                    break;
                case 1:
                    settingsTextAssets[4].text = "True";
                    break;
            }

            SaveSettingsOfBool("opponentEnabled", _opponentNotesEnabled);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_opponentNotesEnabled < 1) _opponentNotesEnabled++;

            switch (_opponentNotesEnabled)
            {
                case 0:
                    settingsTextAssets[4].text = "False";
                    break;
                case 1:
                    settingsTextAssets[4].text = "True";
                    break;
            }

            SaveSettingsOfBool("opponentEnabled", _opponentNotesEnabled);
        }
    }

    #endregion

    #region Ghost Tapping Section

    private int _ghostTapping;
    public void EnableGhostTapping()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_ghostTapping > 0) _ghostTapping--;

            switch (_ghostTapping)
            {
                case 0:
                    settingsTextAssets[5].text = "False";
                    break;
                case 1:
                    settingsTextAssets[5].text = "True";
                    break;
            }

            SaveSettingsOfBool("ghostTapping", _ghostTapping);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_ghostTapping < 1) _ghostTapping++;

            switch (_ghostTapping)
            {
                case 0:
                    settingsTextAssets[5].text = "False";
                    break;
                case 1:
                    settingsTextAssets[5].text = "True";
                    break;
            }

            SaveSettingsOfBool("ghostTapping", _ghostTapping);
        }
    }

    #endregion

    #region Free Animate Section

    private int _freeAnimate;
    public void EnableFreeAnimate()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_freeAnimate > 0) _freeAnimate--;

            switch (_freeAnimate)
            {
                case 0:
                    settingsTextAssets[6].text = "False";
                    break;
                case 1:
                    settingsTextAssets[6].text = "True";
                    break;
            }

            SaveSettingsOfBool("freeAnimate", _freeAnimate);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_freeAnimate < 1) _freeAnimate++;

            switch (_freeAnimate)
            {
                case 0:
                    settingsTextAssets[6].text = "False";
                    break;
                case 1:
                    settingsTextAssets[6].text = "True";
                    break;
            }

            SaveSettingsOfBool("freeAnimate", _freeAnimate);
        }
    }

    #endregion

    #region Incoming Note Warning Section

    private int _incomingNoteWarning;
    public void EnableIncomingNoteWarning()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_incomingNoteWarning > 0) _incomingNoteWarning--;

            switch (_incomingNoteWarning)
            {
                case 0:
                    settingsTextAssets[7].text = "False";
                    break;
                case 1:
                    settingsTextAssets[7].text = "True";
                    break;
            }

            SaveSettingsOfBool("incomingNoteWarning", _incomingNoteWarning);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_incomingNoteWarning < 1) _incomingNoteWarning++;

            switch (_incomingNoteWarning)
            {
                case 0:
                    settingsTextAssets[7].text = "False";
                    break;
                case 1:
                    settingsTextAssets[7].text = "True";
                    break;
            }

            SaveSettingsOfBool("incomingNoteWarning", _incomingNoteWarning);
        }
    }

    #endregion

    #region Auto Pause Section

    private int _autoPause;
    public void EnableAutoPause()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_autoPause > 0) _autoPause--;

            switch (_autoPause)
            {
                case 0:
                    settingsTextAssets[8].text = "False";
                    break;
                case 1:
                    settingsTextAssets[8].text = "True";
                    break;
            }

            SaveSettingsOfBool("autoPause", _autoPause);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_autoPause < 1) _autoPause++;

            switch (_autoPause)
            {
                case 0:
                    settingsTextAssets[8].text = "False";
                    break;
                case 1:
                    settingsTextAssets[8].text = "True";
                    break;
            }

            SaveSettingsOfBool("autoPause", _autoPause);
        }
    }

    #endregion

    #region Note Splashes Section

    private int _noteSplashes;
    public void EnableNoteSplashes()
    {
        if (Input.GetKeyDown(navLeft))
        {
            if (_noteSplashes > 0) _noteSplashes--;

            switch (_noteSplashes)
            {
                case 0:
                    settingsTextAssets[9].text = "False";
                    break;
                case 1:
                    settingsTextAssets[9].text = "True";
                    break;
            }

            SaveSettingsOfBool("noteSplashes", _noteSplashes);
        }
        else if (Input.GetKeyDown(navRight))
        {
            if (_noteSplashes < 1) _noteSplashes++;

            switch (_noteSplashes)
            {
                case 0:
                    settingsTextAssets[9].text = "False";
                    break;
                case 1:
                    settingsTextAssets[9].text = "True";
                    break;
            }

            SaveSettingsOfBool("noteSplashes", _noteSplashes);
        }
    }

    #endregion

    #region Keybinds Section

    #endregion


    #region Settings Applier

    private void SaveSettingsOfBool(string prefsName, int value)
    {
        PlayerPrefs.SetInt(prefsName, value);
        PlayerPrefs.Save();
        DreamwaveUserSetup.instance.LoadUserSettings();
    }
    
    #endregion

    // ignore this shit

    #region String To Sprite Converter

    private void ConvertStringToSpriteText(TextMeshProUGUI textAsset, string input)
    {
        char[] splitInput = input.ToCharArray();

        textAsset.text = string.Empty;

        foreach (char c in splitInput)
        {
            textAsset.text += $"<sprite name=\"{c}\">";
        }
    }

    #endregion
}
