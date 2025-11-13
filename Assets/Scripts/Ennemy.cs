using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Ennemy : MonoBehaviour
{

    public Transform target; 
    public float within_range;
    public float speed;


    public void Update()
    {
        //distance between the target and (this object)
        float dist = Vector3.Distance(target.position, transform.position);
        //within the range?
        if (dist <= within_range)
        {
            //move to target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
        }
        //else, if it is not in rage, it will not follow
    }
}



