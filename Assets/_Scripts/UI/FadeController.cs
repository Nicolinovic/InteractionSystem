using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class FadeController : MonoBehaviour
    {
        private bool isFading;
        public bool IsFading => isFading;

        [SerializeField]
        private bool fadeInOnAwake;

        [SerializeField]
        private float fadeTime;

        private Image image;
        private Coroutine coroutine;

        private void Awake()
        {
            image = GetComponent<Image>();
            if (fadeInOnAwake)
                FadeIn(fadeTime);
        }

        public void FadeIn(float time)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeCo(1, 0, time));
        }

        public void FadeOut(float time)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeCo(0, 1, time));
        }

        private IEnumerator FadeCo(float startAlpha, float endAlpha, float time)
        {
            isFading = true;
            var elapsedTime = 0f;

            while (elapsedTime < time)
            {
                var alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / time);
                SetColorAlpha(alpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            SetColorAlpha(endAlpha);
            coroutine = null;

            isFading = false;
        }

        private void SetColorAlpha(float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}

