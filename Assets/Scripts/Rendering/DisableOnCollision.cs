using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class DisableOnCollision : MonoBehaviour
{
    public bool _ai = false;
    [SerializeField] private WhichSide _whichSide;
    [SerializeField] public string _colName;

    public bool _keyWasDown = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_colName))
        {
            if (!_ai)
            {
                if (_whichSide == WhichSide.Left && Input.GetKey(GameManager.Instance.left))
                {
                    gameObject.SetActive(false);
                }

                if (_whichSide == WhichSide.Down && Input.GetKey(GameManager.Instance.down))
                {
                    gameObject.SetActive(false);
                }

                if (_whichSide == WhichSide.Up && Input.GetKey(GameManager.Instance.up))
                {
                    gameObject.SetActive(false);
                }

                if (_whichSide == WhichSide.Right && Input.GetKey(GameManager.Instance.right))
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        if (_ai)
        {
            if (collision.gameObject.name == "Hitbox")
            {
                gameObject.SetActive(false);
            }
        }
    }
}
