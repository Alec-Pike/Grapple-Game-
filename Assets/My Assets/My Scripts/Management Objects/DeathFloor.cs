using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    [SerializeField] private float threshold = 1f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private MyTimer timer;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private Component[] thingsToDeactivate;
    private float myY;
    private bool isDead = false;

    void Start()
    {
        myY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead && playerTransform.position.y < myY + threshold)
        {
            // stop player movement
            playerRigidbody.linearVelocity = Vector3.zero;
            // disable all specified components
            EnablerUtil.SetMultipleComponentsEnabled(thingsToDeactivate, false);

            // play death animation
            playerAnimator.SetTrigger("death");

            gameOverScreen.SetActive(true);
            retryButton.SetActive(true);

            timer.StopTimer();

            isDead = true;
        }
    }
}
