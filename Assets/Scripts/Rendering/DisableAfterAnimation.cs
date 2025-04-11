using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAnimation : MonoBehaviour
{
    [SerializeField] private Animation anim;

    void Update()
    {
        //if (!anim.isPlaying) gameObject.SetActive(false);
        if (!anim.isPlaying) Destroy(gameObject);
    }
}
