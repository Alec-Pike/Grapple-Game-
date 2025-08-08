using UnityEngine;
using System.Collections; // needed for IEnumerator

public class GoalScript : MonoBehaviour
{
    [SerializeField] private Component[] thingsToDeactivate;
    [SerializeField] private GameObject retryButton, winScreen;
    [SerializeField] private Transform cameraPosition, playerPosition;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float movementDuration;
    private Transform mCam;
    [SerializeField] private Transform player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private MyTimer timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Collision Detected!");
        if (col.gameObject.tag != "Player") return;
        //Debug.Log("It was the player!");

        // stop timer
        timer.StopTimer();

        // disable all specified components
        EnablerUtil.SetMultipleComponentsEnabled(thingsToDeactivate, false);
        // lerp camera and player into place
        StartCoroutine(LerpIntoPosition());
    }

    /*
    * based on code by John French
    * Link: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
    */
    private IEnumerator LerpIntoPosition()
    {
        Vector3 camStartPos = mCam.position;
        Vector3 playerStartPos = player.position;
        Vector3 camStartDir = mCam.forward;
        Vector3 playerStartDir = player.forward;
        float timeFactor;
        for (float time = 0f; time < movementDuration; time += Time.deltaTime)
        {
            timeFactor = time / movementDuration;
            mCam.position = Vector3.Lerp(camStartPos, cameraPosition.position, timeFactor);
            player.position = Vector3.Lerp(playerStartPos, playerPosition.position, timeFactor);
            mCam.forward = Vector3.Lerp(camStartDir, cameraPosition.forward, timeFactor);
            player.forward = Vector3.Lerp(playerStartDir, playerPosition.forward, timeFactor);
            yield return null;
        }
        mCam.position = cameraPosition.position;
        player.position = playerPosition.position;
        mCam.forward = cameraPosition.forward;
        player.forward = playerPosition.forward;

        yield return new WaitForSeconds(0.5f);

        winScreen.SetActive(true);
        retryButton.SetActive(true);

        // player victory dance animation
        playerAnimator.SetTrigger("dance");
    }
}
