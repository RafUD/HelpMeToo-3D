using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    private float startPos, lenght;
    public GameObject cam;
    public float parallaxEffect;  //Speed of background in relation to camera
    void Start()
    {

        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Calculate distance background move based on cam movement
        float distance = cam.transform.position.x * parallaxEffect; // 1 = moves with camera
                                                                    // 0 = does not move
                                                                    // 0.5 = half
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        
        //if background reach end of its length, adjust position 
        if (movement > startPos + lenght)
        {
            startPos += lenght;
        }
        else if (movement < startPos - lenght)
        {
            startPos -= lenght;
        }
    }
}
