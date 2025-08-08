using UnityEngine;
using System.Collections; // needed for IEnumerator
using TMPro;

public class MyCountDown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Component[] thingsToActivate;
    [SerializeField] private MyTimer timer;
    void OnEnable()
    {
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        textMesh.text = "Ready?";
        yield return new WaitForSeconds(3);
        textMesh.text = "3";
        yield return new WaitForSeconds(1);
        textMesh.text = "2";
        yield return new WaitForSeconds(1);
        textMesh.text = "1";
        yield return new WaitForSeconds(1);
        textMesh.text = "GO!!";
        EnablerUtil.SetMultipleComponentsEnabled(thingsToActivate, true);
        timer.StartTimer();
        yield return new WaitForSeconds(1);
        EnablerUtil.SetComponentEnabled(textMesh, false);
    }
}