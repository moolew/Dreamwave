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

    [SerializeField] private float unitsPerSecond = 400f; // Visual spacing scale
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

        _visualSongTime = 0f;

        _songDspStart = AudioSettings.dspTime;
        _audioSource.Play();
    }

    private void Update()
    {
        SongTimeMs = (float)((AudioSettings.dspTime - _songDspStart) * 1000.0);
        JudgementTimeMs = SongTimeMs;
        _visualSongTime = Mathf.Lerp(_visualSongTime, SongTimeMs, 1f - Mathf.Exp(-Time.deltaTime * 30f)); // interp those notes cause its so fucking jitty otherwise
        // some shit to do with unity transform caching, shader based note scrolling? this shit is so niche ill never figure it out :sob:
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

            float y = strumLineY - (note.noteTimeMs - songTime) * ScrollSpeed;

            note.cachedTransform.localPosition = new Vector3(
                note.cachedTransform.localPosition.x,
                y,
                note.cachedTransform.localPosition.z
            );
        }
    }
}
