using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class NoteHitbox : MonoBehaviour
{
    public WhichSide side;

    public List<MsNote> notesWithinHitBox = new();

    // Timing windows (ms)
    public float[] ratingThresholds = new float[5];

    public delegate void HitNote(string scoreType, float msDelay, float dummy, string direction);
    public static event HitNote NoteHit;

    public KeyCode keyForSide;
    public string buttonForSide;
    public GameObject NoteHitParticle;

    private void Awake()
    {
        ratingThresholds[0] = 40f;   // Dreamy
        ratingThresholds[1] = 60f;   // Sick
        ratingThresholds[2] = 80f;   // Cool
        ratingThresholds[3] = 100f;  // Bad
        ratingThresholds[4] = 120f;  // Shit / Miss cutoff
    }

    private void Start()
    {
        buttonForSide = side.ToString();
    }

    private void Update()
    {
        if (PauseMenu.instance._isPaused)
            return;

        float songTime = StrumManager.SM_Instance.JudgementTimeMs;

        // MISS HANDLING
        for (int i = notesWithinHitBox.Count - 1; i >= 0; i--)
        {
            var note = notesWithinHitBox[i];

            if (note == null || note.wasJudged)
            {
                notesWithinHitBox.RemoveAt(i);
                continue;
            }

            float delta = songTime - note.noteTimeMs;

            if (delta > ratingThresholds[4])
            {
                Judge(note, "Missed", delta, true);
                notesWithinHitBox.RemoveAt(i);
            }
        }

        if (notesWithinHitBox.Count == 0)
            return;

        if (Input.GetKeyDown(keyForSide) ||
            MobileControls.instance.GetButtonsPressed(buttonForSide))
        {
            TryHit();
        }
    }

    private void TryHit()
    {
        float songTime = StrumManager.SM_Instance.JudgementTimeMs;

        MsNote best = null;
        float bestDelta = float.MaxValue;

        foreach (var note in notesWithinHitBox)
        {
            if (note == null || note.wasJudged)
                continue;

            float delta = Mathf.Abs(songTime - note.noteTimeMs);

            if (delta < bestDelta)
            {
                bestDelta = delta;
                best = note;
            }
        }

        if (best == null)
            return;

        string rating = GetRating(bestDelta);

        Judge(best, rating, bestDelta, false);
        notesWithinHitBox.Remove(best);
    }

    private void Judge(MsNote note, string rating, float delta, bool isMiss)
    {
        note.wasJudged = true;
        note.enabled = false;

        var sr = note.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        NoteHit?.Invoke(rating, delta, 0f, keyForSide.ToString());

        if (!isMiss &&
            (rating == "Dreamy" || rating == "Sick" || rating == "Cool") &&
            GameManager.Instance.shouldDrawNoteSplashes)
        {
            Instantiate(
                NoteHitParticle,
                NoteHitParticle.transform.position,
                Quaternion.identity
            ).SetActive(true);
        }
    }

    private string GetRating(float delta)
    {
        if (delta <= ratingThresholds[0]) return "Dreamy";
        if (delta <= ratingThresholds[1]) return "Sick";
        if (delta <= ratingThresholds[2]) return "Cool";
        if (delta <= ratingThresholds[3]) return "Bad";
        if (delta <= ratingThresholds[4]) return "Shit";
        return "Missed";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var note = collision.GetComponent<MsNote>();
        if (note == null || note.wasJudged)
            return;

        if (!notesWithinHitBox.Contains(note))
            notesWithinHitBox.Add(note);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var note = collision.GetComponent<MsNote>();
        if (note == null || note.wasJudged)
            return;

        notesWithinHitBox.Remove(note);
    }
}
