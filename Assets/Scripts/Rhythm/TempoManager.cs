using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AnimationStep
{
    OneOverFour,
    TwoOverFour,
    FourOverFour
}

public class TempoManager : MonoBehaviour
{
    [SerializeField] private bool _dontInstance;
    public static TempoManager instance;

    public AnimationStep animStep;

    public AudioSource audioSource;
    public AudioClip _metronome;

    public float beatsPerMinute = 120;
    public double secondsPerBeat;
    public double nextBeatTime;

    public int stepsPerBeat = 4;
    public int currentStep = 1;
    
    public delegate void BeatEventHandler();
    public static event BeatEventHandler OnBeat;

    public delegate void StepEventHandler(int currentStep);
    public static event StepEventHandler OnStep;

    private void Awake() { if (!_dontInstance) { instance = this; } audioSource = GetComponent<AudioSource>(); }

    private void OnEnable()
    {
        PauseMenu.Pause += OnPauseHandler;
    }

    private void OnDisable()
    {
        PauseMenu.Pause -= OnPauseHandler;
    }

    private void OnDestroy()
    {
        PauseMenu.Pause -= OnPauseHandler;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            _metronomeS = new GameObject().AddComponent<AudioSource>();
            _metronomeS.clip = _metronome;
        }

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource attached to the TempoManager.");
            return;
        }

        beatsPerMinute = UniBpmAnalyzer.AnalyzeBpm(audioSource.clip) * audioSource.pitch;

        secondsPerBeat = 60.0 / beatsPerMinute;

        // Calculate the initial beat time using AudioSettings.dspTime
        nextBeatTime = AudioSettings.dspTime + secondsPerBeat;
    }

    AudioSource _metronomeS;
    private void Update()
    {
        // Check if it's time for the next beat based on both audio and system time
        if (audioSource.isPlaying && AudioSettings.dspTime >= nextBeatTime)
        {
            OnBeat?.Invoke();

            // Calculate the next beat time based on the updated step count
            nextBeatTime += secondsPerBeat;

            // Increment the step count
            currentStep = (currentStep % stepsPerBeat) + 1;

            if (PauseMenu.instance != null && PauseMenu.instance._isPaused) return;

            // Trigger the OnStep event only when it's on the correct step
            switch (animStep)
            {
                case AnimationStep.OneOverFour:
                    if (currentStep == 1)
                    {
                        OnStep?.Invoke(currentStep);
                    }
                    break;
                case AnimationStep.TwoOverFour:
                    if (currentStep == 2 || currentStep == 4)
                    {
                        OnStep?.Invoke(currentStep);
                    }
                    break;
                case AnimationStep.FourOverFour:
                    OnStep?.Invoke(currentStep);
                    break;
            }

            /*if (SceneManager.GetActiveScene().name == "MainScene" && currentStep == 1 || currentStep == 3) _metronomeS.PlayOneShot(_metronome);*/
        }
    }

    private void OnPauseHandler(bool paused)
    {
        if (paused)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }
}
