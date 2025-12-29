using UnityEngine;

public class StrumManager : MonoBehaviour
{
    public static StrumManager SM_Instance;

    [SerializeField] private AudioSource _audioSource;

    public float SongTimeMs;
    public float ScrollSpeed;
    public float strumLineY;
    [SerializeField] private float unitsPerSecond = 5f;

    private void Awake()
    {
        SM_Instance = this;
    }

    private void Start()
    {
        ScrollSpeed = unitsPerSecond / 1000f;
    }

    private void Update()
    {
        SongTimeMs = _audioSource.time * 1000f;
    }
}
