using UnityEngine;

public class RepeatedZoom : MonoBehaviour
{
    public Material _repeatedZoom;
    public float _zoomedSmoothTime;
    public float _zoomedValue;
    public float _zoom;

    void Update()
    {
        _zoom = Mathf.Lerp(_zoom, _zoomedValue, Time.deltaTime * _zoomedSmoothTime);
        _repeatedZoom.SetFloat("_Zoom", _zoom);
    }
}
