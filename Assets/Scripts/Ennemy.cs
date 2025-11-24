using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float within_range = 10f;
    public float speed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float dist = Vector3.Distance(target.position, transform.position);

        if (dist <= within_range)
        {
            // Keep enemy on current ground height
            Vector3 targetPos = new Vector3(
                target.position.x,
                transform.position.y,
                target.position.z
            );

            Vector3 newPos = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(newPos);
        }
    }
}
