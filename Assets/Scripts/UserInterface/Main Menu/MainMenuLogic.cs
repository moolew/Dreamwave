using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DreamwaveGlobal;

public enum CurrentActiveMenu
{
    Title,
    Selection,
    SongSelection
}

public class MainMenuLogic : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator menuAnim;

    [Header("States")]
    [SerializeField] private CurrentActiveMenu _currentActiveMenu;
    [SerializeField] private bool _canChangeMenu = true;

    [Header("State Settings")]
    [SerializeField] private float _cooldownTime; 

    private void Awake()
    {
        StartCoroutine("DisableMenuSelection");
    }

    private void Update()
    {
        if (Input.anyKeyDown) ChangeMenus();
    }

    private void ChangeMenus()
    {
        if (menuAnim.IsInTransition(0)) return;
        if (!_canChangeMenu) return;

        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Backspace))
        {
            switch (_currentActiveMenu)
            {
                case CurrentActiveMenu.Selection:
                    _cooldownTime = 0.7f;
                    menuAnim.CrossFade("Outro", 0.1f, 0);
                    _currentActiveMenu = CurrentActiveMenu.Title;
                    break;
                case CurrentActiveMenu.SongSelection:
                    _cooldownTime = 0.5f;
                    menuAnim.CrossFade("Back To Selection", 0.1f, 0);
                    _currentActiveMenu = CurrentActiveMenu.Selection;
                    break;
            }

            StartCoroutine("DisableMenuSelection");
        }

        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKeyDown(KeyCode.Backspace))
        {
            switch (_currentActiveMenu)
            {
                case CurrentActiveMenu.Title:
                    _cooldownTime = 0.7f;
                    menuAnim.CrossFade("Intro", 0.1f, 0);
                    _currentActiveMenu = CurrentActiveMenu.Selection;
                    break;
            }

            StartCoroutine("DisableMenuSelection");
        }
    }

    private IEnumerator DisableMenuSelection()
    {
        _canChangeMenu = false;
        yield return new WaitForSecondsRealtime(_cooldownTime);
        _canChangeMenu = true;
    }

    public void ViewGitHub()
    {
        Application.OpenURL("https://github.com/Iouander/Dreamwave");
    }

    public void SongSelection()
    {
        if (!_canChangeMenu) return;

        _cooldownTime = 0.5f;
        _currentActiveMenu = CurrentActiveMenu.SongSelection;
        menuAnim.CrossFade("Song Selection", 0.2f, 0);
    }

    public void MenuSelection()
    {
        if (!_canChangeMenu) return;

        _cooldownTime = 0.5f;
        _currentActiveMenu = CurrentActiveMenu.Selection;
        menuAnim.CrossFade("Back To Selection", 0.2f, 0);
    }

    public void SetMod(int i)
    {
        LoadedModSong = ModSongs[i];
    }

    public void LoadSong(string songName)
    {
        SceneManager.LoadScene(songName); // cannot use async because of how charts are loaded and such
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
