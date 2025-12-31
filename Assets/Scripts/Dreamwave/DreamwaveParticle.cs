using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamwaveParticle : DreamwaveAnimation
{
    public SpriteRenderer Renderer;
    public List<Sprite> ParticleAnimation;
    public float AnimationTime = 0.04f;
    public Vector2 offset;
    public Vector2 scale;
    public Transform parent;

    private void OnEnable()
    {
        transform.SetParent(parent);
        transform.localScale = scale;
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.sortingOrder = 20;

        if (ParticleAnimation == null || ParticleAnimation.Count == 0)
        {
            Debug.LogError("ParticleAnimation list is empty. Cannot start animation.");
            return;
        }

        List<Vector2> offsets = new List<Vector2>();
        for (int i = 0; i < ParticleAnimation.Count; i++)
        {
            offsets.Add(new Vector3(offset.x, 0));
        }

        PlayAnimation(Renderer, ParticleAnimation, offsets, AnimationTime);
    }

}
