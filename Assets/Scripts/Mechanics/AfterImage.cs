using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public string direction;
    public float speed;
    public float time;
    public float timer;
    public SpriteRenderer spriteRenderer;

    private void Update()
    {
        timer -= Time.deltaTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, timer / time);

        if (timer <= 0.01f)
        {
            Destroy(gameObject);
        }

        if (direction == "left")
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        else if (direction == "right")
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        else if (direction == "up")
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
        }
        else if (direction == "down")
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}