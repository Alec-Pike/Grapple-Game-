using UnityEngine;
using System.Collections; // needed for IEnumerator

public class MyLoadingScreen : MonoBehaviour
{
    [SerializeField] private float loadDuration, fadeDuration;
    [SerializeField] private GameObject countDown;
    private CanvasRenderer cr;

    void Start()
    {
        cr = GetComponent<CanvasRenderer>();
        StartCoroutine(DoSetup());
    }

    private IEnumerator DoSetup()
    {
        yield return new WaitForSeconds(loadDuration);

        countDown.SetActive(true);

        float startingAlpha = cr.GetAlpha();
        //float timeFactor;
        for (float time = 0f; time < fadeDuration; time += Time.deltaTime)
        {
            cr.SetAlpha(Mathf.Lerp(startingAlpha, 0, time / fadeDuration));
            yield return null;
        }
        cr.SetAlpha(0);

        //gameObject.active = false;
    }
}