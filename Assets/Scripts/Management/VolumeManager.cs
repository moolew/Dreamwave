using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private List<Image> volumeSprites = new();
    [SerializeField] private float visibleTime = 2f;
    private Transform _slider;
    private AudioSource _source;
    [SerializeField] private AudioClip _clip;

    private float timer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _slider = transform.GetChild(0);

        _source = new GameObject().AddComponent<AudioSource>();
        _source.gameObject.transform.SetParent(transform);
        _source.transform.SetAsLastSibling();
        _source.clip = _clip;
    }

    private bool _changed = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            AudioListener.volume += 0.1f;
            timer = visibleTime;
            _changed = true;
            _source.Play();
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            AudioListener.volume -= 0.1f;
            timer = visibleTime;
            _changed = true;
            _source.Play();
        }

        AudioListener.volume = Mathf.Clamp01(AudioListener.volume);

        if (_changed)
            _slider.transform.localPosition = Vector3.Lerp(_slider.transform.localPosition, new(0, 520, 0), Time.deltaTime * 15f);
        else
            _slider.transform.localPosition = Vector3.Lerp(_slider.transform.localPosition, new(0, 700, 0), Time.deltaTime * 15f);

        if (timer > 0.01f)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0.01f)
        {
            _changed = false;
        }
    }

    private void VolumeVisualiser(float volume)
    {
        int bars = Mathf.RoundToInt(volume * volumeSprites.Count);

        for (int i = 0; i < volumeSprites.Count; i++)
        {
            volumeSprites[i].enabled = i < bars;
        }
    }
}
