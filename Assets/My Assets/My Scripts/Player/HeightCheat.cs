using UnityEngine;

public class HeightCheat : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            //transform.position = new Vector3(transform.position.x, 350f, transform.position.z);
            rb.linearVelocity = new Vector3(0f, 35f, 0f);
        }
    }
}