using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isShooted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (isShooted)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }
    public void Shoot(float currentPower)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(transform.forward * currentPower * 4.448f);
        isShooted = true;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        isShooted = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        transform.SetParent(collision.transform);

    }
}
