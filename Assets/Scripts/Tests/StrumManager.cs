using System.Collections.Generic;
using UnityEngine;

public class StrumManager : MonoBehaviour
{
    public static StrumManager SM_Instance;

    [SerializeField] private AudioSource _audioSource;

    public float JudgementTimeMs { get; private set; }
    public float SongTimeMs;
    public float ScrollSpeed;
    public float strumLineY;

    [SerializeField] private float unitsPerSecond = 400f;
    private float _playerScrollMultiplier = 1f;

    private double _songDspStart;
    private float _visualSongTime;

    public List<MsNote> activeNotes = new();

    private void Awake()
    {
        SM_Instance = this;
    }

    private void Start()
    {
        _playerScrollMultiplier = PlayerPrefs.GetFloat("scrollSpeed", 1f);
        ScrollSpeed = (unitsPerSecond / 1000f) * _playerScrollMultiplier;

        activeNotes.AddRange(FindObjectsOfType<MsNote>());

        _songDspStart = AudioSettings.dspTime;
        _audioSource.Play();
    }

    private void Update()
    {
        SongTimeMs = (float)((AudioSettings.dspTime - _songDspStart) * 1000.0 * _audioSource.pitch);
        JudgementTimeMs = SongTimeMs;

        // Visual smoothing ONLY (no gameplay logic depends on this)
        _visualSongTime = Mathf.Lerp(
            _visualSongTime,
            SongTimeMs,
            1f - Mathf.Exp(-Time.deltaTime * 50f)
        );
    }

    private void LateUpdate()
    {
        float songTime = _visualSongTime;

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];

            if (note == null || note.cachedTransform == null)
            {
                activeNotes.RemoveAt(i);
                continue;
            }

            if (note.wasJudged && !note.isEvent)
            {
                activeNotes.RemoveAt(i);
                continue;
            }

            float y = strumLineY - (note.noteTimeMs - songTime) * ScrollSpeed;

            note.cachedTransform.localPosition = new Vector3(
                note.cachedTransform.localPosition.x,
                y,
                note.cachedTransform.localPosition.z
            );
        }
    }
}
