using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToBlack : MonoBehaviour
{

    [SerializeField] Material _effect;

    private void Start()
    {
        FadeIn();

        FindObjectOfType<Necromancer>().OnBossDeath += () =>
        {
            FindObjectOfType<Messages>().MessageShown += () =>
            {
                FadeOut();
            };
        };
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _effect);
    }

    public void FadeIn()
    {
        enabled = true;
        StartCoroutine(FadeInStep());
    }

    public void FadeOut()
    {
        enabled = true;
        StartCoroutine(FadeOutStep());
    }

    IEnumerator FadeInStep()
    {
        float t = 1;

        while (t > 0)
        {
            _effect.SetFloat("_Fade", t);
            yield return null;
            t -= Time.deltaTime;
        }

        enabled = false;
    }

    IEnumerator FadeOutStep()
    {
        float t = 0;

        while (t < 1)
        {
            _effect.SetFloat("_Fade", t);
            yield return null;
            t += Time.deltaTime;
        }

        Application.Quit();
        enabled = false;
    }
}
