using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private FadeController fadeController;
        [SerializeField]
        private float fadeTime = 0.9f;

        private bool switchingScene = false;

        public void StartScene(string name)
        {
            if (switchingScene)
                return;

            switchingScene = true;

            StartCoroutine(StartSceneCo());
            IEnumerator StartSceneCo()
            {
                fadeController.FadeOut(fadeTime);
                while (fadeController.IsFading)
                {
                    yield return null;
                }

                SceneManager.LoadScene(name);
            }
        }
    }
}
