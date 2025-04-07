using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamwaveVisualiser : MonoBehaviour
{
    public SpriteRenderer _renderer;
    public Sprite[] visualizerSprites;
    public int numSamples = 512;
    public float sensitivity = 100.0f; // Sensitivity factor to spread the loudness values

    [SerializeField] private AudioSource audioSource;
    private float[] samples;
    public float loudness;

    void Start()
    {
        audioSource = TempoManager.instance.audioSource;
        samples = new float[numSamples];
    }

    private void Update()
    {
        AnalyzeSound();
        UpdateVisualizer();
    }

    private void AnalyzeSound()
    {
        audioSource.GetOutputData(samples, 0); // Fill array with samples

        // Calculate the average loudness
        float sum = 0;
        for (int i = 0; i < numSamples; i++)
        {
            sum += samples[i] * samples[i]; // Square the sample values to get the power
        }
        loudness = Mathf.Sqrt(sum / numSamples); // Get the RMS value of the samples

        // Normalize the loudness value
        loudness = Mathf.Clamp01(loudness * sensitivity);
    }

    private void UpdateVisualizer()
    {
        // Scale the loudness to the range of the sprite array
        int spriteIndex = Mathf.Clamp((int)(loudness * (visualizerSprites.Length - 1)), 0, visualizerSprites.Length - 1);
        _renderer.sprite = visualizerSprites[spriteIndex];
    }
}
