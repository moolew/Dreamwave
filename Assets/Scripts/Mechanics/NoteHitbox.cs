using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using static GameManager;

public enum Side
{
    Left,
    Down,
    Up,
    Right
}

public class NoteHitbox : MonoBehaviour
{
    public Side side;
    public List<GameObject> notesWithinHitBox = new List<GameObject>();
    public GameObject centre;
    public float delayInMs;
    public float[] ratingThresholds;
    public delegate void HitNote(string scoreType, float msDelay, float noteDistance, string direction);
    public static event HitNote NoteHit;
    public KeyCode keyForSide;
    public string buttonForSide;
    public GameObject NoteHitParticle;

    private Stopwatch stopwatch;
    private bool noteHit = false;

    private void Start()
    {
        switch (side)
        {
            case Side.Left:
                buttonForSide = "Left";
                break;
            case Side.Down:
                buttonForSide = "Down";
                break;
            case Side.Up:
                buttonForSide = "Up";
                break;
            case Side.Right:
                buttonForSide = "Right";
                break;
        }

        stopwatch = new Stopwatch();
    }

    void Update()
    {
        NoteInput();
    }

    private void NoteInput()
    {
        if (PauseMenu.instance._isPaused) return;
        if (notesWithinHitBox.Count <= 0) return;

        var fn = notesWithinHitBox[0].gameObject;
        var dist = Vector2.Distance(centre.transform.position, fn.transform.position);

        // Detect key press or button press for the side
        if (Input.GetKeyDown(keyForSide) || MobileControls.instance.GetButtonsPressed(buttonForSide))
        {
            stopwatch.Stop();
            delayInMs = (float)stopwatch.Elapsed.TotalMilliseconds;

            // Determine the score type based on distance thresholds
            if (dist >= ratingThresholds[4])
            {
                NoteHit("Shit", delayInMs, dist, keyForSide.ToString());
            }
            else if (dist >= ratingThresholds[3])
            {
                NoteHit("Bad", delayInMs, dist, keyForSide.ToString());
            }
            else if (dist >= ratingThresholds[2])
            {
                NoteHit("Cool", delayInMs, dist, keyForSide.ToString());
                if (GameManager.Instance.shouldDrawNoteSplashes)
                {
                    Instantiate(NoteHitParticle, NoteHitParticle.transform.position, Quaternion.identity).SetActive(true);
                }
            }
            else if (dist >= ratingThresholds[1])
            {
                NoteHit("Sick", delayInMs, dist, keyForSide.ToString());
                if (GameManager.Instance.shouldDrawNoteSplashes)
                {
                    Instantiate(NoteHitParticle, NoteHitParticle.transform.position, Quaternion.identity).SetActive(true);
                }
            }
            else if (dist >= ratingThresholds[0])
            {
                NoteHit("Dreamy", delayInMs, dist, keyForSide.ToString());
                if (GameManager.Instance.shouldDrawNoteSplashes)
                {
                    Instantiate(NoteHitParticle, NoteHitParticle.transform.position, Quaternion.identity).SetActive(true);
                }
            }
            else if (dist <= ratingThresholds[0])
            {
                NoteHit("Shit", delayInMs, dist, keyForSide.ToString());
            }

            // Reset stopwatch and handle note visibility
            stopwatch.Reset();
            noteHit = true;  // Mark note as hit
            HandleNoteVisibility(fn);
            notesWithinHitBox.Remove(fn);  // Remove the hit note from the list
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            notesWithinHitBox.Add(collision.gameObject);
            stopwatch.Restart();
            noteHit = false;  // Reset note hit flag
        }

        if (collision.gameObject.CompareTag("Note Hold") || collision.gameObject.CompareTag("Note Hold End"))
        {
            if (Input.GetKey(keyForSide) && !GameManager.Instance._playerScript._isSinging)
            {
                NoteHit("Dreamy", delayInMs, 0, keyForSide.ToString());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            if (!noteHit)
            {
                NoteHit("Missed", delayInMs, 2, keyForSide.ToString() + "miss");
            }

            notesWithinHitBox.Remove(collision.gameObject);
            HandleNoteVisibility(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Note Hold")) // not gonna check for the ends because i love you
        {
            if (!Input.GetKey(keyForSide)) NoteHit("Missed", delayInMs, 2, keyForSide.ToString() + "miss");
        }
    }

    private void HandleNoteVisibility(GameObject noteObject)
    {
        noteObject.SetActive(false);
    }
}
