using UnityEngine;
using static ChartEditor;

public class EditorNote : MonoBehaviour
{
    public void Enter()
    {
        CE_Instance.CurrentSelectedNote = gameObject;
    }

    public void Exit()
    {
        CE_Instance.CurrentSelectedNote = null;
    }
}
