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

    [SerializeField] private float visibleWindowMs = 8000f; // only update notes within this window around current song time
    [SerializeField] private float positionEpsilon = 0.01f; // only write transform if y changes by more than this
    [SerializeField] private float physicsSimWindowMs = 500f; // only simulate physics for notes within this smaller window

    [SerializeField] private float unitsPerSecond = 200f;
    public float _playerScrollMultiplier = 1f;

    private double _songDspStart;
    private float _visualSongTime;

    public List<MsNote> activeNotes = new();
    private List<Rigidbody2D> _noteRigidbodies = new();
    private List<Collider2D> _noteColliders = new();

    private void Awake()
    {
        SM_Instance = this;
    }

    public void ReSetSpeed()
    {
        _playerScrollMultiplier = PlayerPrefs.GetFloat("scrollSpeed", 1f);
        ScrollSpeed = (unitsPerSecond / 1000f) * _playerScrollMultiplier;
    }

    private void Start()
    {
        _playerScrollMultiplier = PlayerPrefs.GetFloat("scrollSpeed", 1f);
        ScrollSpeed = (unitsPerSecond / 1000f) * _playerScrollMultiplier;

        activeNotes.AddRange(FindObjectsOfType<MsNote>());

        // Cache Rigidbody2D and Collider2D for each note to avoid repeated GetComponent calls
        _noteRigidbodies.Capacity = activeNotes.Count;
        _noteColliders.Capacity = activeNotes.Count;
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var n = activeNotes[i];
            if (n == null)
            {
                _noteRigidbodies.Add(null);
                _noteColliders.Add(null);
                continue;
            }

            _noteRigidbodies.Add(n.GetComponent<Rigidbody2D>());
            _noteColliders.Add(n.GetComponent<Collider2D>());
        }

        _songDspStart = AudioSettings.dspTime;
        _audioSource.Play();
    }

    private void Update()
    {
        SongTimeMs = (float)((AudioSettings.dspTime - _songDspStart) * 1000.0 * _audioSource.pitch);
        JudgementTimeMs = SongTimeMs;

        /*Debug.Log(SongTimeMs);*/

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

        // Pre-calc bounds so we only update notes that are reasonably close to the strum line.
        float lowerBound = songTime - visibleWindowMs;
        float upperBound = songTime + visibleWindowMs;

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];

            if (note == null || note.cachedTransform == null)
            {
                activeNotes.RemoveAt(i);
                // Keep cached lists in sync
                if (_noteRigidbodies.Count > i) _noteRigidbodies.RemoveAt(i);
                if (_noteColliders.Count > i) _noteColliders.RemoveAt(i);
                continue;
            }

            if (note.wasJudged && !note.isEvent)
            {
                activeNotes.RemoveAt(i);
                if (_noteRigidbodies.Count > i) _noteRigidbodies.RemoveAt(i);
                if (_noteColliders.Count > i) _noteColliders.RemoveAt(i);
                continue;
            }

            // Physics simulation enable/disable based on a tighter window around the strum line.
            float physicsLower = songTime - physicsSimWindowMs;
            float physicsUpper = songTime + physicsSimWindowMs;
            bool inPhysicsWindow = note.noteTimeMs >= physicsLower && note.noteTimeMs <= physicsUpper;

            Rigidbody2D rb = (_noteRigidbodies.Count > i) ? _noteRigidbodies[i] : null;
            Collider2D col = (_noteColliders.Count > i) ? _noteColliders[i] : null;

            if (rb != null)
            {
                if (rb.simulated != inPhysicsWindow)
                    rb.simulated = inPhysicsWindow;
            }

            if (col != null)
            {
                if (col.enabled != inPhysicsWindow)
                    col.enabled = inPhysicsWindow;
            }

            // If the note is far outside the visible window, skip position updates.
            if (note.noteTimeMs < lowerBound || note.noteTimeMs > upperBound)
            {
                continue;
            }

            float y = strumLineY - (note.noteTimeMs - songTime) * ScrollSpeed;

            // Only write the transform when the Y position actually changed beyond a small epsilon.
            var t = note.cachedTransform;
            Vector3 cur = t.localPosition;
            if (Mathf.Abs(cur.y - y) > positionEpsilon)
            {
                cur.y = y;
                t.localPosition = cur;
            }
        }
    }
}
