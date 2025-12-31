using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingNoteWarning : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sideWarningRenderer;

    [Header("States")]
    [SerializeField] private bool _showSideWarning;
    [SerializeField] private List<GameObject> _notesInSection = new();

    [Header("Settings")]
    [SerializeField] private float _smoothing = 10f;

    private void LateUpdate()
    {
        if (!GameManager.Instance.canSongStart) return;
        if (!GameManager.Instance.start) return;

        if (_notesInSection.Count >= 1) _showSideWarning = true;
        else _showSideWarning = false;

        Color color = _sideWarningRenderer.color;

        if (!GameManager.Instance.shouldDisplayIncomingNoteWarning)
        {
            color.a = 0;
            _sideWarningRenderer.color = color;
            return;
        }

        if (_showSideWarning)
        {
            color.a = Mathf.Lerp(color.a, 1, _smoothing * Time.deltaTime);
            _sideWarningRenderer.color = color;
        }
        else
        {
            color.a = Mathf.Lerp(color.a, 0, _smoothing * Time.deltaTime);
            _sideWarningRenderer.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note") | collision.gameObject.CompareTag("EnemyNote") | collision.gameObject.CompareTag("Note Hold Parent"))
        {
            _notesInSection.Add(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note") | collision.gameObject.CompareTag("EnemyNote") | collision.gameObject.CompareTag("Note Hold Parent"))
        {
            if (collision.gameObject.GetComponent<MsNote>().wasJudged)
            {
                _notesInSection.Remove(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note") | collision.gameObject.CompareTag("EnemyNote") | collision.gameObject.CompareTag("Note Hold Parent"))
        {
            _notesInSection.Remove(collision.gameObject);
        }
    }
}
