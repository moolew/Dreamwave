// PostProcessManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessEffectChannel
{
    public float Target;
    public float Speed;

    public Func<float> Getter;
    public Action<float> Setter;

    public Action EnableOverride;
    public Action EnableParameter;

    public void Activate()
    {
        EnableOverride?.Invoke();
        EnableParameter?.Invoke();
    }

    public void Update()
    {
        float current = Getter();
        float next = Mathf.Lerp(current, Target, Time.deltaTime * Speed);
        Setter(next);
    }
}

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager PP_Instance { get; private set; }
    public Volume volume;

    Bloom bloom;
    ChromaticAberration chromatic;
    Vignette vignette;
    ColorAdjustments colorAdjustments;

    Dictionary<string, PostProcessEffectChannel> effects;

    void Awake()
    {
        PP_Instance = this;
        volume = GetComponent<Volume>();

        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out chromatic);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);

        effects = new Dictionary<string, PostProcessEffectChannel>()
        {
            ["bloom"] = new PostProcessEffectChannel
            {
                Getter = () => bloom.intensity.value,
                Setter = v => bloom.intensity.value = v,
                EnableOverride = () => bloom.active = true,
                EnableParameter = () => bloom.intensity.overrideState = true
            },
            ["chromatic"] = new PostProcessEffectChannel
            {
                Getter = () => chromatic.intensity.value,
                Setter = v => chromatic.intensity.value = v,
                EnableOverride = () => chromatic.active = true,
                EnableParameter = () => chromatic.intensity.overrideState = true
            },
            ["vignette"] = new PostProcessEffectChannel
            {
                Getter = () => vignette.intensity.value,
                Setter = v => vignette.intensity.value = v,
                EnableOverride = () => vignette.active = true,
                EnableParameter = () => vignette.intensity.overrideState = true
            },
            ["saturation"] = new PostProcessEffectChannel
            {
                Getter = () => colorAdjustments.saturation.value,
                Setter = v => colorAdjustments.saturation.value = v,
                EnableOverride = () => colorAdjustments.active = true,
                EnableParameter = () => colorAdjustments.saturation.overrideState = true
            }
        };

    }

    void Update()
    {
        foreach (var fx in effects.Values)
            fx.Update();
    }

    public void SetEffect(string effect, float value, float speed)
    {
        if (!effects.TryGetValue(effect.ToLower(), out var fx))
            return;

        fx.Activate();
        fx.Target = value;
        fx.Speed = speed;
    }

    public void SnapEffect(string effect, float value)
    {
        if (!effects.TryGetValue(effect.ToLower(), out var fx))
            return;

        fx.Target = value;
        fx.Setter(value);
    }
}
