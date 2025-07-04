using UnityEngine;

public class RepeatedZoom : MonoBehaviour
{
    public Material _repeatedZoom;
    public float _zoomedSmoothTime;
    public float _zoomedValue;
    public float _zoom;
    
    public float _x;
    public float _smoothedX;
    public float _xTime;

    public float _y;
    public float _smoothedY;
    public float _yTime;

    /// <summary>
    /// This is for rotation, not positioning.
    /// </summary>
    public float _z; // this is for rotation
    public float _smoothedZ; // this is for rotation
    public float _zTime; // this is for rotation

    void Update()
    {
        _zoom = Mathf.Lerp(_zoom, _zoomedValue, Time.deltaTime * _zoomedSmoothTime);
        _repeatedZoom.SetFloat("_Zoom", _zoom);

        _x = Mathf.Lerp(_x, _smoothedX, Time.deltaTime * _xTime);
        _y = Mathf.Lerp(_y, _smoothedY, Time.deltaTime * _yTime);
        _z = Mathf.Lerp(_z, _smoothedZ, Time.deltaTime * _zTime);

        _repeatedZoom.SetFloat("_OffsetX", _x);
        _repeatedZoom.SetFloat("_OffsetY", _y);
        _repeatedZoom.SetFloat("_Rotation", _z);
    }
}
