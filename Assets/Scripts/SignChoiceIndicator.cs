using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignChoiceIndicator : MonoBehaviour
{
    [SerializeField] Image spriteRenderer;
    [SerializeField] Sprite correct;
    [SerializeField] Sprite wrong;
    float alpha;



    void Start()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0);

    }

    void Update()
    {

    }

    public void Pop(bool isCorrect)
    {
        spriteRenderer.sprite = isCorrect ? correct : wrong;
        StartCoroutine(FadeOut(0.4f));
    }
    IEnumerator FadeOut(float time)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            //alpha = Mathf.Lerp(1, 0, Mathf.Abs(elapsed / time - 0.5f) * 2);
            alpha = Mathf.Lerp(1, 0, Mathf.Sqrt(elapsed / time));

            spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return null;
        }
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
}
