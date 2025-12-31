using UnityEngine;

public class MsNote : MonoBehaviour
{
    public float noteTimeMs;
    [HideInInspector] public Transform cachedTransform;
    public bool wasJudged = false;

    void Awake()
    {
        cachedTransform = transform;
    }
}
