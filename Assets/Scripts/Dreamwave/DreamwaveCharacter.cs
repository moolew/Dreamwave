/*****************************************************************************
**                                         
**  Dreamverse Character
**
**  Name    :   DreamwaveCharacter.cs
**  Author  :   Lewis-Lee
** 
** 
**  Desc
**      A custom character animator for 2d chracters. Allows for easy
**      modding in-game without having to use the Unity Engine whatsoever.
**      Also better for this type of project than Unity's Animator Component.
**
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamwaveCharacter : DreamwaveAnimation
{
    [Header("Settings")]
    public bool IsCustom = false; // decides whether you're using a base character or a custom character

    [Header("States")]
    [SerializeField] public bool _isSinging = false;
    [SerializeField] protected bool _canAnimate = true;

    [Header("Rendering")]
    public SpriteRenderer Renderer;
    public float AnimationSpeed = 0.1f;
    public float AnimationHoldSpeed = 0.1f;
    public float SingAnimationHold = 0.3f;

    [Header("After Image Effect")]
    public bool afterImage = false;
    public float afterImageSpeed = 0.1f;
    public float afterImageDuration = 0.1f;
    public bool flipXAfterImage = false;
    public bool flipYAfterImage = false;
    public Color afterImageColour = Color.white;

    [Header("Character Animation Lists")]
    public List<Sprite> LeftAnimations = new(); public List<Vector2> LeftOffsets = new();
    public List<Sprite> DownAnimations = new(); public List<Vector2> DownOffsets = new();
    public List<Sprite> UpAnimations = new(); public List<Vector2> UpOffsets = new();
    public List<Sprite> RightAnimations = new(); public List<Vector2> RightOffsets = new();
    public List<Sprite> IdleAnimation = new(); public List<Vector2> IdleOffsets = new();

    private void OnEnable()
    {
        TempoManager.OnStep += PlayStillAnimation;
        PauseMenu.Pause += OnPause;
        NoteHitbox.NoteHit += OnNoteHit;
    }

    private void OnDisable()
    {
        TempoManager.OnStep -= PlayStillAnimation;
        PauseMenu.Pause -= OnPause;
        NoteHitbox.NoteHit -= OnNoteHit;
    }
    private void OnDestroy()
    {
        TempoManager.OnStep -= PlayStillAnimation;
        PauseMenu.Pause -= OnPause;
        NoteHitbox.NoteHit -= OnNoteHit;
    }

    private void OnPause(bool paused)
    {
        if (paused) { _canAnimate = false; StopAllCoroutines(); }
        else _canAnimate = true;
    }

    private void Update()
    {
        if (GameManager.Instance.canFreeAnimate) InputStates();
    }

    private void InputStates()
    {
        if (!_canAnimate) return;
        if (!GameManager.Instance.canSongStart) return;
        if (!GameManager.Instance.canFreeAnimate) return;

        if (Input.GetKeyDown(GameManager.Instance.left))
        {
            StopAllCoroutines();
            StartCoroutine(SingAnimation("Left"));
        }
        else if (Input.GetKeyDown(GameManager.Instance.down))
        {
            StopAllCoroutines();
            StartCoroutine(SingAnimation("Down"));
        }
        else if (Input.GetKeyDown(GameManager.Instance.up))
        {
            StopAllCoroutines();
            StartCoroutine(SingAnimation("Up"));
        }
        else if (Input.GetKeyDown(GameManager.Instance.right))
        {
            StopAllCoroutines();
            StartCoroutine(SingAnimation("Right"));
        }
    }

    private void OnNoteHit(string scoreType, float msDelay, float noteDistance, string direction)
    {
        if (GameManager.Instance.canFreeAnimate) return;

        switch (scoreType)
        {
            case "Shit": // will handle miss sprites another time
                break;
            case "Miss":
                break;
            default:
                if (direction == GameManager.Instance.left.ToString())
                {
                    StopAllCoroutines();
                    StartCoroutine(SingAnimation("Left"));
                    StartCoroutine(AfterImageEffect(direction));
                }
                else if (direction == GameManager.Instance.down.ToString())
                {
                    StopAllCoroutines();
                    StartCoroutine(SingAnimation("Down"));
                    StartCoroutine(AfterImageEffect(direction));
                }
                else if (direction == GameManager.Instance.up.ToString())
                {
                    StopAllCoroutines();
                    StartCoroutine(SingAnimation("Up"));
                    StartCoroutine(AfterImageEffect(direction));
                }
                else if (direction == GameManager.Instance.right.ToString())
                {
                    StopAllCoroutines();
                    StartCoroutine(SingAnimation("Right"));
                    StartCoroutine(AfterImageEffect(direction));
                }
                break;
        }
    }

    private void PlayStillAnimation(int step)
    {
        switch (step)
        {
            case 1:
                if (!_isSinging)
                {
                    PlayAnimation(Renderer, IdleAnimation, IdleOffsets, AnimationSpeed);
                }
                break;
            case 3:
                if (!_isSinging)
                {
                    if (TempoManager.instance.beatsPerMinute <= 200) PlayAnimation(Renderer, IdleAnimation, IdleOffsets, AnimationSpeed);
                }
                break;
        }
    }

    private IEnumerator SingAnimation(string direction)
    {
        _isSinging = true;

        var animations = direction switch
        {
            "Left" => LeftAnimations,
            "Right" => RightAnimations,
            "Up" => UpAnimations,
            "Down" => DownAnimations,
            _ => IdleAnimation
        };
        if (animations.Count == 0) { _isSinging = false; yield break; }

        var offsets = direction switch
        {
            "Left" => LeftOffsets,
            "Right" => RightOffsets,
            "Up" => UpOffsets,
            "Down" => DownOffsets,
            _ => IdleOffsets
        };

        PlayAnimation(Renderer, animations, offsets, AnimationSpeed);

        yield return new WaitForSecondsRealtime(SingAnimationHold);

        _isSinging = false;
        yield break;
    }
    
    private IEnumerator AfterImageEffect(string dir)
    {
        if (!afterImage) yield break;
        GameObject afterImageObj = new("AfterImage");
        SpriteRenderer sr = afterImageObj.AddComponent<SpriteRenderer>();
        var ai = afterImageObj.AddComponent<AfterImage>();
        ai.spriteRenderer = sr;
        ai.direction = dir.ToLower();
        ai.time = afterImageDuration;
        ai.timer = afterImageDuration;
        ai.speed = afterImageSpeed;
        sr.sprite = Renderer.sprite;
        sr.color = afterImageColour;
        sr.sortingOrder = Renderer.sortingOrder;
        afterImageObj.transform.position = new Vector3(Renderer.transform.position.x, Renderer.transform.position.y, Renderer.transform.position.z);
        afterImageObj.transform.localScale = Renderer.transform.lossyScale;
        yield return null;
    }
}
