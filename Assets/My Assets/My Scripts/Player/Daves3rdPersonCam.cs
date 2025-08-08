using UnityEngine;

/*
    3rd person camera controller based on a tutorial by Dave / GameDevelopment on YouTube test
    Link: https://youtu.be/UCwwn2q4Vys?si=XdENVXNYQrw7W6az
*/

public class Daves3rdPersonCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation, player, playerModel, zoomLookAt;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject normalCam, zoomedInCam, crosshairs;

    //[Header("Keybinds")]
    //[SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    //[SerializeField] private KeyCode grappleKey = KeyCode.Mouse0;
    private float prevZoomVal = 0f, prevGrappleVal = 0f;

    public CameraStyle currentStyle = CameraStyle.Normal;
    public enum CameraStyle
    {
        Normal,
        ZoomedIn
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Zoom In") || (Input.GetAxisRaw("Zoom In") > 0.3f && prevZoomVal < 0.3f)) SwitchCameraStyle(CameraStyle.ZoomedIn);
        if (Input.GetButtonUp("Zoom In") || (Input.GetAxisRaw("Zoom In") < 0.3f && prevZoomVal > 0.3f)) SwitchCameraStyle(CameraStyle.Normal);

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // rotate player model
        switch (currentStyle)
        {
            case CameraStyle.Normal:
                float horizInput = Input.GetAxis("Horizontal");
                float vertInput = Input.GetAxis("Vertical");
                Vector3 inputDir = orientation.forward * vertInput + orientation.right * horizInput;

                if (Input.GetButtonDown("Grapple") || (Input.GetAxisRaw("Grapple") > 0.3f && prevGrappleVal < 0.3f))
                {
                    playerModel.forward = orientation.forward;
                }
                else if (inputDir != Vector3.zero)
                {
                    playerModel.forward = Vector3.Slerp(playerModel.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
                }
                break;

            case CameraStyle.ZoomedIn:
                Vector3 dirToLookAt = zoomLookAt.position - new Vector3(transform.position.x, zoomLookAt.position.y, transform.position.z);
                orientation.forward = dirToLookAt.normalized;

                playerModel.forward = dirToLookAt.normalized;
                break;
        }

        // update prev vals for next time
        prevZoomVal = Input.GetAxisRaw("Zoom In");
        prevGrappleVal = Input.GetAxisRaw("Grapple");
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        normalCam.SetActive(false);
        zoomedInCam.SetActive(false);
        crosshairs.SetActive(false);

        switch (newStyle)
        {
            case CameraStyle.Normal:
                normalCam.SetActive(true);
                break;
            case CameraStyle.ZoomedIn:
                zoomedInCam.SetActive(true);
                crosshairs.SetActive(true);
                break;
        }

        currentStyle = newStyle;
    }
}
