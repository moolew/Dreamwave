using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static PostProcessingManager;

public enum TypeOfScrollEvent 
{
    FocusCentre,
    FocusPlayerRight,
    FocusPlayerLeft,
    CameraFov,
    ChangeSongSpeed,
    SectionCompleteAnimation,
    Cutscene,
    Animation,
    InstantRestart,
    RepeatedTile,
    RotateTile,
    MoveTiles,
    PostProcessEffect,
    AfterImageEffect
}

public class ScrollEvents : MonoBehaviour
{
    public TypeOfScrollEvent typeOfScrollEvent;

    public float ZoomAmount;
    public float ZoomSpeed;
    public bool BpmBump;

    public float RepeatRate;
    public float RepeatTime;

    public string Axis;
    public float RotateAmount;
    public float RotateTime;

    public float MoveAmount;
    public float MoveTime;

    public string PostProcessEffectName;
    public float PostProcessEffectValue;
    public float PostProcessEffectSpeed;

    public string whichPlayerToAfterImage;
    public bool displayAfterImage;
    public float afterImageSpeed;
    public float afterImageColourR;
    public float afterImageColourG;
    public float afterImageColourB;
    public float afterImageColourA;
    public float afterImageDuration;
    public int afterImageZIndex;
    public bool flipXAfterImage;
    public bool flipYAfterImage;

    [SerializeField] private float scrollSpeedModificationAmount;

    [Header("Animation")]
    [Tooltip("If this event doesn't make use of animations, don't drag one into this variable.")][SerializeField] private Animation eventAnim;

    [Header("Cutscene")]
    [SerializeField] private string _cutscenePath;

    private void FocusCentre() => Instance.focus = Focus.Centre;
    private void FocusLeftPlayer() => Instance.focus = Focus.LeftPlayer;
    private void FocusRightPlayer() => Instance.focus = Focus.RightPlayer;

    private void CameraFov()
    {
        Instance.CameraFov = ZoomAmount;
        Instance.CameraFovSpeed = ZoomSpeed;
        Instance.BpmBump = BpmBump;
    }

    private void CameraZoomRepeat(float repeatValue, float smoothing)
    {
        Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._zoomedValue = repeatValue;
        Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._zoomedSmoothTime = smoothing;
    }

    private void CameraRotateTile(float rotateAmount, float smoothing)
    {
        Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._smoothedZ = rotateAmount;
        Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._zTime = smoothing;
    }

    private void CameraMoveTile(string axis, float moveAmount, float smoothing)
    {
        if (axis == "X")
        {
            Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._smoothedX = moveAmount;
            Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._xTime = smoothing;
        }
        else if (axis == "Y")
        {
            Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._smoothedY = moveAmount;
            Camera.main.transform.parent.parent.parent.GetComponent<RepeatedZoom>()._yTime = smoothing;
        }
    }

    private void PostProcessEffect(string effect, float value, float speed)
    {
        PP_Instance.SetEffect(effect, value, speed);
    }

    private void AfterImageEffect(string player, bool display, bool flipX, bool flipY, int zIndex,float duration, float speed, float R, float G, float B, float A)
    {
        player = (player ?? "").Trim().ToLower();

        // support both formats so mods don't break
        bool isLeft = (player == "left" || player == "leftplayer");
        bool isRight = (player == "right" || player == "rightplayer");

        float nr = (R > 1f) ? (R / 255f) : R;
        float ng = (G > 1f) ? (G / 255f) : G;
        float nb = (B > 1f) ? (B / 255f) : B;
        float na = Mathf.Clamp01(A);

        var col = new Color(Mathf.Clamp01(nr), Mathf.Clamp01(ng), Mathf.Clamp01(nb), na);

        if (isLeft)
        {
            var s = Instance._aiScript.GetComponent<DreamwaveAICharacter>();
            s.afterImage = display;
            s.afterImageSpeed = duration;
            s.afterImageColour = col;
            s.afterImageSpeed = speed;
            s.afterImageDuration = duration;
            s.afterImageZIndex = zIndex;
            if (flipX) s.flipXAfterImage = true;
            if (flipY) s.flipYAfterImage = true;
        }
        else if (isRight)
        {
            var s = Instance._playerScript.GetComponent<DreamwaveCharacter>();
            s.afterImage = display;
            s.afterImageSpeed = duration;
            s.afterImageColour = col;
            s.afterImageSpeed = speed;
            s.afterImageDuration = duration;
            s.afterImageZIndex = zIndex;
            if (flipX) s.flipXAfterImage = true;
            if (flipY) s.flipYAfterImage = true;
        }
    }

    private void ChangeSongSpeed() => Instance.scrollManager.scrollSpeedMultiplier = scrollSpeedModificationAmount;

    private bool _fired = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ScrollEventTrigger"))
        {
            if (!_fired)
            {
                FireEvent(typeOfScrollEvent);
                _fired = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ScrollEventTrigger"))
        {
            if (!_fired)
            {
                FireEvent(typeOfScrollEvent);
                _fired = true;
            }
        }
    }

    private void FireEvent(TypeOfScrollEvent ev)
    {
        switch (ev)
        {
            case TypeOfScrollEvent.FocusCentre: FocusCentre(); break;
            case TypeOfScrollEvent.FocusPlayerRight: FocusRightPlayer(); break;
            case TypeOfScrollEvent.FocusPlayerLeft: FocusLeftPlayer(); break;
            case TypeOfScrollEvent.CameraFov: CameraFov(); break;
            case TypeOfScrollEvent.ChangeSongSpeed: ChangeSongSpeed(); break;
            case TypeOfScrollEvent.Animation: eventAnim.Play(); break;
            case TypeOfScrollEvent.InstantRestart: SceneManager.LoadScene(SceneManager.GetActiveScene().name); break;
            case TypeOfScrollEvent.Cutscene: Instance.DreamwaveVideoStreamer.InitLoad(_cutscenePath); break;
            case TypeOfScrollEvent.RepeatedTile: CameraZoomRepeat(RepeatRate, RepeatTime); break;
            case TypeOfScrollEvent.RotateTile: CameraRotateTile(RotateAmount, RotateTime); break;
            case TypeOfScrollEvent.MoveTiles: CameraMoveTile(Axis, MoveAmount, MoveTime); break;
            case TypeOfScrollEvent.PostProcessEffect: PostProcessEffect(PostProcessEffectName, PostProcessEffectValue, PostProcessEffectSpeed); break;
            case TypeOfScrollEvent.AfterImageEffect: AfterImageEffect(whichPlayerToAfterImage, displayAfterImage, flipXAfterImage, flipYAfterImage, afterImageZIndex, afterImageDuration, afterImageSpeed, afterImageColourR, afterImageColourG, afterImageColourB, afterImageColourA); break;
        }
    }
}
