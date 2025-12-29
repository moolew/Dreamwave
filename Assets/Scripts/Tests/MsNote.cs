using UnityEngine;
using static StrumManager;

public class MsNote : MonoBehaviour
{
    [SerializeField] private float _noteMsTime;
    private float _noteY;

    private void Start()
    {
        NoteMsPosition();
    }

    void Update()
    {
        NoteMsPosition();
    }

    private void NoteMsPosition()
    {
        float songTime = SM_Instance.SongTimeMs;

        _noteY = SM_Instance.strumLineY - (_noteMsTime - songTime) * SM_Instance.ScrollSpeed;

        transform.localPosition = new Vector3(transform.localPosition.x, _noteY, transform.localPosition.z);
    }
}
