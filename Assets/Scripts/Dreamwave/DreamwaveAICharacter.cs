/*****************************************************************************
**                                         
**  Dreamverse Character
**
**  Name    :   DreamwaveAICharacter.cs
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

public class DreamwaveAICharacter : DreamwaveCharacter
{
    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();

        if (!IsCustom) return;

        // now loads custom assets from dedicated script
    }

    private void OnEnable()
    {
        TempoManager.OnStep += PlayStillAnimation;
        PauseMenu.Pause += OnPause;
    }

    private void OnDisable()
    {
        TempoManager.OnStep -= PlayStillAnimation;
        PauseMenu.Pause -= OnPause;
    }
    private void OnDestroy()
    {
        TempoManager.OnStep -= PlayStillAnimation;
        PauseMenu.Pause -= OnPause;
    }

    private void OnPause(bool paused)
    {
        if (paused) { _canAnimate = false; StopAllCoroutines(); }
        else _canAnimate = true;
    }

    private void PlayStillAnimation(int step)
    {
        if (!_isSinging)
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
    }

    public IEnumerator SingAnimation(string direction)
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

    public void EmitAfterImage(string dir)
    {
        StartCoroutine(AfterImageEffect(dir));
    }
}
