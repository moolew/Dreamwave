using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************************************
**                                         
**  Dreamwave AI Communicator
**
**  Name    :   DreamwaveAICommunicator.cs
**  Author  :   Lewis-Lee
** 
** 
**  Desc
**      A custom character animator for 2d chracters. Allows for easy
**      modding in-game without having to use the Unity Engine whatsoever.
**      Also better for this type of project than Unity's Animator Component.
**      This is for AI as the second player when playing solo.
**
*******************************************************************************/

public class DreamwaveAICommunicator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private WhichSide _whichSide;
    [SerializeField] private float noteAnimSpeed = 0.05f;

    [Header("Scripts")]
    [SerializeField] private DreamwaveAICharacter _character;
    [SerializeField] private NoteController _noteController;

    [Header("GameObjects")]
    [SerializeField] private GameObject _noteParticle;

    [Header("Components")]
    [SerializeField] public SpriteRenderer _spriteRenderer;
    [SerializeField] public List<Sprite> _noteSpritesHeld;
    [SerializeField] public List<Sprite> _noteSpritesReleased;

    private void Awake()
    {
        _spriteRenderer = transform.parent.parent.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _noteSpritesHeld.Add(_noteController.noteSpritesDown[_noteController.noteSpritesDown.Count - 1]);
        _noteSpritesReleased.Add(_noteController.noteSpritesRelease[_noteController.noteSpritesRelease.Count - 1]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyNote"))
        {
            switch (_whichSide)
            {
                case WhichSide.Left:
                    _character.StopAllCoroutines();
                    _character.StartCoroutine(_character.SingAnimation("Left"));
                    if (GameManager.Instance.shouldDrawNoteSplashes)
                    {
                        var pl = Instantiate(_noteParticle, _noteParticle.transform.position, Quaternion.identity);
                        pl.SetActive(true);
                    }
                    break;
                case WhichSide.Down:
                    _character.StopAllCoroutines();
                    _character.StartCoroutine(_character.SingAnimation("Down"));
                    if (GameManager.Instance.shouldDrawNoteSplashes)
                    {
                        var pd = Instantiate(_noteParticle, _noteParticle.transform.position, Quaternion.identity);
                        pd.SetActive(true);
                    }
                    break;
                case WhichSide.Up:
                    _character.StopAllCoroutines();
                    _character.StartCoroutine(_character.SingAnimation("Up"));
                    if (GameManager.Instance.shouldDrawNoteSplashes)
                    {
                        var pu = Instantiate(_noteParticle, _noteParticle.transform.position, Quaternion.identity);
                        pu.SetActive(true);
                    }
                    break;
                case WhichSide.Right:
                    _character.StopAllCoroutines();
                    _character.StartCoroutine(_character.SingAnimation("Right"));
                    if (GameManager.Instance.shouldDrawNoteSplashes)
                    {
                        var pr = Instantiate(_noteParticle, _noteParticle.transform.position, Quaternion.identity);
                        pr.SetActive(true);
                    }
                    break;
            }

            StopAllCoroutines();
            StartCoroutine(SimulateKey());
            collision.gameObject.SetActive(false);
        }
    }

    private IEnumerator SimulateKey()
    {
        for (int i = 0; i < _noteSpritesHeld.Count; i++)
        {
            _spriteRenderer.sprite = _noteSpritesHeld[i];

            yield return new WaitForSecondsRealtime(noteAnimSpeed);
        }

        yield return new WaitForSecondsRealtime(0.05f);

        for (int i = _noteSpritesReleased.Count - 1; i >= 0; i--)
        {
            _spriteRenderer.sprite = _noteSpritesReleased[i];
            yield return new WaitForSecondsRealtime(noteAnimSpeed);
        }

        yield break;
    }
}
