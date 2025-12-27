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
    PostProcessEffect
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

    private void ChangeSongSpeed() => Instance.scrollManager.scrollSpeedMultiplier = scrollSpeedModificationAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ScrollEventTrigger"))
        {
            switch (typeOfScrollEvent)
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
            }
        }
    }
}
