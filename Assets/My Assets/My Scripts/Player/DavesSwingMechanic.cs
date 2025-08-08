using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DavesMovement))]

/*
    Spider-man-esque swinging mechanic based on a tutorial by Dave / GameDevelopment on YouTube
    Link: https://youtu.be/HPjuTK91MA8?si=lRH_SLHyOXKHXaQd
*/

public class DavesSwingMechanic : MonoBehaviour
{
    [Header("References")]
    private LineRenderer lr;
    private DavesMovement dm;
    private Transform player;
    [SerializeField] private Transform gunTip, cam;
    [SerializeField] private LayerMask whatIsGrappleable;

    [Header("Swinging")]
    [SerializeField] private float maxSwingDist = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    [SerializeField] private float maxDistMultiplier = 0.8f;
    [SerializeField] private float minDistMultiplier = 0.25f;
    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7f;
    [SerializeField] private float massScale = 4.5f;
    [SerializeField] private float ropeSpeed = 8f;
    private Vector3 currentGrapplePosition;

    [Header("OdmGear")]
    [SerializeField] private Transform orientation;
    private Rigidbody rb;
    [SerializeField] private float horizontalThrustForce, forwardThrustForce, cameraForce, extendCableSpeed;

    [Header("Disconnect")]
    [Tooltip("Rope will disconnect if the player is above the connection point and the angle between the rope and Vector3.up is less than this. Set to 0 to turn off this feature.")]
    [SerializeField] private float cutoffAngle = 45f;
    [Tooltip("Rope will disconnect and player will jump if player is this close to the connection point")]
    [SerializeField] private float jumpThreshold;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool drawThresholdGizmo = false;

    [Header("Prediction")]
    [SerializeField] private float predictionConeCastAngle = 30f;
    private float predictionConeCastRadius;
    [SerializeField] private Transform predictionPoint;
    [SerializeField] private bool drawPredictionGizmo = false;
    private RaycastHit predictionHit;

    //[Header("Input")]
    //[SerializeField] private KeyCode swingKey = KeyCode.Mouse0;
    private float prevGrappleVal = 0f;
    //[SerializeField] private KeyCode cableShortenKey = KeyCode.Space;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        dm = GetComponent<DavesMovement>();
        player = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        predictionConeCastRadius = maxSwingDist * Mathf.Tan(predictionConeCastAngle * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForSwingPoints();

        if (Input.GetButtonDown("Grapple") || (Input.GetAxisRaw("Grapple") > 0.3f && prevGrappleVal < 0.3f)) StartSwing();
        if (Input.GetButtonUp("Grapple") || (Input.GetAxisRaw("Grapple") < 0.3f && prevGrappleVal > 0.3f)) StopSwing();

        if (joint != null) OdmGearMovement();

        ProcessDisconnect();

        // update prevGrappleVal for next time
        prevGrappleVal = Input.GetAxisRaw("Grapple");
    }

    void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        // return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        dm.SetSwinging(true);

        //Debug.Log("Starting swing");

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distFromPoint = Vector3.Distance(player.position, swingPoint);

        // the distance grapple will try to keep from grapple point
        joint.maxDistance = distFromPoint * maxDistMultiplier;
        joint.minDistance = distFromPoint * minDistMultiplier;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }

    private void StopSwing()
    {
        //Debug.Log("Stopping swing");

        dm.SetSwinging(false);

        lr.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        // if not grappling, don't draw rope
        if (!joint) return;

        //Debug.Log("Drawing rope");

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * ropeSpeed);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void OdmGearMovement()
    {
        // SHINZOU WO SASAGEYO!!!!

        // right and left
        float horizInput = Input.GetAxisRaw("Horizontal");
        rb.AddForce(orientation.right * horizontalThrustForce * horizInput * Time.deltaTime);

        // forward
        float vertInput = Input.GetAxisRaw("Vertical");
        if (vertInput > 0f) rb.AddForce(orientation.forward * forwardThrustForce * vertInput * Time.deltaTime);

        // force in direction of camera
        Vector3 lateralCamDir = new Vector3(cam.forward.x, 0f, cam.forward.z);
        rb.AddForce(lateralCamDir * cameraForce * Time.deltaTime);

        // shorten cable
        if (Input.GetButton("Jump"))
        {
            Vector3 dirToPoint = swingPoint - player.position;
            rb.AddForce(dirToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distFromPoint * maxDistMultiplier;
            joint.minDistance = distFromPoint * minDistMultiplier;
        }
        // extend cable
        if (vertInput < 0f)
        {
            float extendedDistFromPoint = Vector3.Distance(player.position, swingPoint) + extendCableSpeed * -1f * vertInput;

            joint.maxDistance = extendedDistFromPoint * maxDistMultiplier;
            joint.minDistance = extendedDistFromPoint * minDistMultiplier;
        }
    }

    private void CheckForSwingPoints()
    {
        //if (joint != null) return; // optional

        //Debug.Log("Checking for swing points");

        //Physics.SphereCast(cam.position + predictionStartOffset, predictionSphereCastRadius, cam.forward, out predictionHit, maxSwingDist, whatIsGrappleable);
        RaycastHit[] coneCastHits = ConeCastExtension.ConeCastAll(cam.position, cam.forward, maxSwingDist, predictionConeCastAngle, whatIsGrappleable);

        // Line of sight check
        // turn into list
        List<RaycastHit> coneCastHitsList = new List<RaycastHit>(coneCastHits);
        // List indeciesToRemove = new List<int>();
        // for (int i = 0; i < coneCastHitsList.Length; i++)
        // {
        //     // get the direction from camera to hit
        //     Vector3 sightCheckDir = coneCastHits[i].point - cam.position;
        //     // if there's something non-grappleable between the camera and the hit...
        //     if (Physics.Raycast(cam.position, sightCheckDir, maxSwingDist, ~whatIsGrappleable))
        //     {
        //         // clear that hit entry
        //         coneCastHits[i] = new RaycastHit();
        //     }
        // }
        coneCastHitsList.RemoveAll(CheckLineOfSight);
        RaycastHit[] coneCastHitsFiltered = coneCastHitsList.ToArray();

        // get the closest hit to center of the cone
        if (ConeCastUtility.FindClosestToConeCenter(coneCastHitsFiltered, cam.position, cam.forward, whatIsGrappleable, out predictionHit))
        {
            // hit found!
            Vector3 hitPoint = predictionHit.point;
            //Debug.Log("Swing point found!");
            Debug.DrawLine(cam.position, hitPoint); // draw line to hitPoint
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = hitPoint;
        }
        else
        {
            // hit not found...
            //Debug.Log("No swing points found.");
            Debug.DrawLine(cam.position, cam.position + cam.forward * maxSwingDist); // draw line out to maxSwingDist
            predictionPoint.gameObject.SetActive(false);
        }
    }

    private void ProcessDisconnect()
    {
        if (!joint) return; // return if we're not swinging

        #region Distance check
        //calculate distance to swingPoint
        float distFromPoint = Vector3.Distance(player.position, swingPoint);
        // is it below the threshold?
        if (distFromPoint <= jumpThreshold)
        {
            StopSwing();

            // reset y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            // do the jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        #endregion

        #region Angle check
        // don't disconnect if player is below swingPoint
        if (player.position.y <= swingPoint.y) return;
        // get vector between player and swingPoint
        Vector3 lineBetweenPoints = player.position - swingPoint;
        // calculate the angle against the vertical
        float angle = Vector3.Angle(Vector3.up, lineBetweenPoints);
        //Debug.Log("angle = " + angle);
        // stop swinging if we're too close to the vertical
        if (angle < cutoffAngle && rb.linearVelocity.y > 10f) StopSwing();
        // reset y velocity
        //rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // do the jump
        //rb.AddForce(Vector3.up * jumpForce * 2, ForceMode.Impulse);
        #endregion
    }

    void OnDrawGizmos()
    {
        if (drawPredictionGizmo)
        {
            Gizmos.color = Color.red;
            // Gizmos.DrawLine(cam.position, cam.position + cam.forward * maxSwingDist);
            // Gizmos.DrawWireSphere(cam.position + cam.forward * maxSwingDist, predictionConeCastRadius);

            // Have to calculate radius bc we can't reference the definition in Start()
            predictionConeCastRadius = maxSwingDist * Mathf.Tan(predictionConeCastAngle * Mathf.Deg2Rad);

            // Draw the cone's base as a wireframe sphere
            Gizmos.DrawWireSphere(cam.position + cam.forward * maxSwingDist, predictionConeCastRadius);

            // Draw sample rays to show the cone's boundaries
            int sampleRays = 4;
            for (int i = 0; i < sampleRays; i++)
            {
                // Distribute rays evenly around the cone's edge
                float angle = i * 360f / sampleRays;
                Vector3 offset = Quaternion.AngleAxis(angle, cam.forward) * cam.right * Mathf.Tan(predictionConeCastAngle * Mathf.Deg2Rad);
                Vector3 rayDirection = (cam.forward + offset).normalized;
                Gizmos.DrawRay(cam.position, rayDirection * maxSwingDist);
            }
        }

        if (drawThresholdGizmo)
        {
            Gizmos.DrawWireSphere(transform.position, jumpThreshold);
        }
    }

    private bool CheckLineOfSight(RaycastHit rch)
    {
        // get the direction from camera to hit
        Vector3 sightCheckDir = rch.point - cam.position;
        // return true if there's something non-grappleable between the camera and the hit
        return Physics.Raycast(cam.position, sightCheckDir, maxSwingDist, ~whatIsGrappleable);
    }
}
