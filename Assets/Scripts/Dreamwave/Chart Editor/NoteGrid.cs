using UnityEngine;
using static ChartEditor;
using static PauseMenu;

public class NoteGrid : MonoBehaviour
{
    public int side;

    private void Update()
    {
        if (!instance._isPaused) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RequestPlaceNote();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeleteSelectedNote();
        }
    }

    public void RequestPlaceNote()
    {
        CE_Instance.PlaceNote(Input.mousePosition, side);
    }

    public void DeleteSelectedNote()
    {
        if (CE_Instance.CurrentSelectedNote != null)
        {
            Destroy(CE_Instance.CurrentSelectedNote);
            CE_Instance.CurrentSelectedNote = null;
        }
    }
}
