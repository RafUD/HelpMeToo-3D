using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float within_range;

    void Update()
    {
        float dist = Vector3.Distance(target.position, transform.position);

        if (dist <= within_range)
        {
            Vector3 targetPos = new Vector3(
                target.position.x,
                transform.position.y,
                target.position.z
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
        }
    }
}
