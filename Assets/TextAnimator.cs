using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] string[] texts;
    int currentIndex;

    public float charTime;
    public UnityEvent onEnd;
    Coroutine textCR;


    TextMeshProUGUI textMesh;


    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (textCR == null && currentIndex < texts.Length)
        {
            textCR = StartCoroutine(AnimateText(texts[currentIndex]));
            currentIndex++;
        }

        if (textMesh.enabled && currentIndex >= texts.Length)
        {
            textMesh.enabled = false;
            onEnd.Invoke();
        }
    }


    IEnumerator AnimateText(string line)
    {
        textMesh.text = "";

        float t = 0f;
        float currentTime = charTime;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '.') currentTime = charTime * 2.5f;
            else if (line[i] == ' ') currentTime = charTime * 1.5f;
            else if (line[i] == ',') currentTime = charTime * 2f;
            else currentTime = charTime;

            while (t < currentTime)
            {
                t += Time.deltaTime;
                yield return null;
            }

            textMesh.text += line[i];
            t = 0;

        }

        textMesh.text += "<br>[PRESS ANY KEY TO CONTINUE]";

        yield return new WaitUntil(() => Input.anyKeyDown);

        textCR = null;
    }
}
