using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreamwaveRating : DreamwaveAnimation
{
    public Image rrenderer;
    public List<Sprite> entranceFrames = new();
    public List<Sprite> frames = new();
    public List<Sprite> exitFrames = new();
    public List<Vector2> offsets = new();
    public float timeToFlick = 10f;
    public float lastingTime = 0.5f;
    public bool repeatAnim = true;

    private void Awake()
    {
        rrenderer = GetComponent<Image>();
        timer = lastingTime;
    }

    private void OnEnable()
    {
        timer = lastingTime;
        color = rrenderer.color;
        color.a = 1f;
        rrenderer.color = color;
    }

    private void Start() => StartCoroutine(RepeatAnimation());

    private IEnumerator RepeatAnimation()
    {
        yield return new WaitUntil(() => frames.Count > 0 && offsets.Count > 0);

        while (repeatAnim)
        {
            _complete = false;
            rrenderer.enabled = true;

            PlayAnimation(rrenderer, frames, offsets, timeToFlick);

            yield return new WaitUntil(() => _complete);

            timer = lastingTime;

            yield return new WaitUntil(() => timer <= 0f);
        }
    }

    private float timer;
    private Color color;
    private void Update()
    {
        if (!rrenderer.enabled) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 15f);
            rrenderer.color = color;

            if (color.a <= 0.01f)
            {
                rrenderer.enabled = false;
            }
        }
    }

}
