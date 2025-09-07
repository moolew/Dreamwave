using System.Collections.Generic;
using UnityEngine;

public class ChartEditor : MonoBehaviour
{
    public static ChartEditor CE_Instance;

    [SerializeField] private List<GameObject> _placedNotes;
    [SerializeField] private GameObject _note;

    public bool _canPlaceNotes = false;

    private void Awake()
    {
        CE_Instance = this;
    }

    public GameObject CurrentSelectedNote;
    private GameObject _currentPlacedNote;
    public void PlaceNote(Vector3 position, int side)
    {
        // just stop editor from placing multiple notes on the same position
        if (_currentPlacedNote.transform.position != position) return;

        switch (side)
        {
            case 0:
                _currentPlacedNote = Instantiate(_note, position, Quaternion.Euler(0, 0, 0));
                break;
            case 1:
                _currentPlacedNote = Instantiate(_note, position, Quaternion.Euler(0, 0, 90));
                break;
            case 2:
                _currentPlacedNote = Instantiate(_note, position, Quaternion.Euler(0, 0, 180));
                break;
            case 3:
                _currentPlacedNote = Instantiate(_note, position, Quaternion.Euler(0, 0, -90));
                break;
        }

        _placedNotes.Add(_currentPlacedNote);
        _currentPlacedNote.transform.parent = transform;
    }

    public void EnterGrid()
    {
        _canPlaceNotes = true;
    }

    public void ExitGrid()
    {
        _canPlaceNotes = false;
    }
}
