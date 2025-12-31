using UnityEngine;

public class MsNote : MonoBehaviour
{
    public float noteTimeMs;
    [HideInInspector] public Transform cachedTransform;

    void Awake()
    {
        cachedTransform = transform;
    }
}
