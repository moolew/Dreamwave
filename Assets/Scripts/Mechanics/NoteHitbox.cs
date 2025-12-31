using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class NoteHitbox : MonoBehaviour
{
    public WhichSide side;

    public List<MsNote> notesWithinHitBox = new();
    public float[] ratingThresholds;

    public delegate void HitNote(string scoreType, float msDelay, float dummy, string direction);
    public static event HitNote NoteHit;

    public KeyCode keyForSide;
    public string buttonForSide;
    public GameObject NoteHitParticle;

    private void Awake()
    {
        ratingThresholds[0] = 30;
        ratingThresholds[1] = 55;
        ratingThresholds[2] = 75;
        ratingThresholds[3] = 95;
        ratingThresholds[4] = 115;
    }

    private void Start()
    {
        buttonForSide = side.ToString();
    }

    private void Update()
    {
        if (PauseMenu.instance._isPaused) return;

        float songTime = StrumManager.SM_Instance.JudgementTimeMs;

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
                note.wasJudged = true;
                NoteHit?.Invoke("Missed", delta, 0f, keyForSide.ToString() + "miss");
                note.enabled = false;
                note.GetComponent<SpriteRenderer>().enabled = false;
                notesWithinHitBox.RemoveAt(i);
            }
        }

        if (notesWithinHitBox.Count == 0) return;

        if (Input.GetKeyDown(keyForSide) || MobileControls.instance.GetButtonsPressed(buttonForSide))
            TryHit();
    }

    private void TryHit()
    {
        float songTime = StrumManager.SM_Instance.JudgementTimeMs;

        MsNote best = null;
        float bestDelta = float.MaxValue;

        foreach (var note in notesWithinHitBox)
        {
            if (note == null) continue;

            float delta = Mathf.Abs(note.noteTimeMs - songTime);

            if (delta < bestDelta)
            {
                bestDelta = delta;
                best = note;
            }
        }

        if (best == null) return;

        string rating = GetRating(bestDelta);

        Debug.Log(GetRating(bestDelta) + " " + bestDelta);

        if (rating == "Missed") return;

        NoteHit?.Invoke(rating, bestDelta, 0f, keyForSide.ToString());

        if ((rating == "Dreamy" || rating == "Sick" || rating == "Cool") && GameManager.Instance.shouldDrawNoteSplashes)
        {
            Instantiate(NoteHitParticle, NoteHitParticle.transform.position, Quaternion.identity).SetActive(true);
        }

        best.wasJudged = true;
        best.GetComponent<SpriteRenderer>().enabled = false;
        best.enabled = false;
        notesWithinHitBox.Remove(best);
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
        if (note == null) return;

        notesWithinHitBox.Add(note);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var note = collision.GetComponent<MsNote>();
        if (note == null || note.wasJudged) return;

        float delta = StrumManager.SM_Instance.JudgementTimeMs - note.noteTimeMs;

        if (delta > ratingThresholds[4])
        {
            note.wasJudged = true;

            NoteHit?.Invoke("Missed", delta, 0f, keyForSide.ToString() + "miss");

            notesWithinHitBox.Remove(note);
            note.enabled = false;
            note.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
