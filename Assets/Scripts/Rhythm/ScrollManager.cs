using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public float scrollSpeedMultiplier = 1.0f;
    public float smoothingFactor = 5.0f; // Adjust this value for more or less smoothing.
    public TempoManager tempoManager;
    public bool CanScroll = true;

    private void OnEnable()
    {
        PauseMenu.Pause += OnPause;
    }

    private void OnDisable()
    {
        PauseMenu.Pause -= OnPause;
    }

    private void OnDestroy()
    {
        PauseMenu.Pause -= OnPause;
    }

    private void Start()
    {
        scrollSpeedMultiplier /= tempoManager.audioSource.pitch;
    }

    private void Update()
    {
        if (tempoManager != null && GameManager.Instance.start && CanScroll && GameManager.Instance.canSongStart)
        {
            float audioTime = tempoManager.audioSource.time;

            float targetY = (audioTime * tempoManager.beatsPerMinute / 60f) * scrollSpeedMultiplier;

            float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothingFactor);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            //Debug.Log(transform.position.y);
        }
    }

    private void OnPause(bool pausedState)
    {
        CanScroll = !pausedState;
    }
}
