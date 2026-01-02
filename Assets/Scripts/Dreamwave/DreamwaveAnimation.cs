using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreamwaveAnimation : MonoBehaviour
{
    protected bool _complete = false;
    private Coroutine _currentAnim;

    protected void PlayAnimation(Component target, List<Sprite> sprites, List<Vector2> offsets, float timeToFlick)
    {
        if (_currentAnim != null)
            StopCoroutine(_currentAnim);

        _currentAnim = StartCoroutine(RunAnimation(target, sprites, offsets, timeToFlick));
    }

    private IEnumerator RunAnimation(Component target, List<Sprite> sprites, List<Vector2> offsets, float timeToFlick)
    {
        if (sprites == null || sprites.Count == 0 || (offsets != null && offsets.Count != sprites.Count))
        {
            Debug.LogError("Sprite list is empty, or offsets do not match the number of sprites.");
            yield break;
        }

        SpriteRenderer sr = target as SpriteRenderer;
        Image img = target as Image;
        RectTransform rect = img ? img.rectTransform : null;

        if (!sr && !img)
        {
            Debug.LogError("DreamwaveAnimation target must be SpriteRenderer or Image.");
            yield break;
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            if (sr)
            {
                sr.sprite = sprites[i];
                if (offsets != null) sr.transform.localPosition = offsets[i];
            }
            else
            {
                img.sprite = sprites[i];
                if (offsets != null) rect.anchoredPosition = offsets[i];
            }

            yield return new WaitForSecondsRealtime(timeToFlick);

            if (i == sprites.Count - 1)
            {
                _complete = true;
                yield break;
            }
        }
    }
}
