using UnityEngine;

public class RotateObject : MonoBehaviour
{
    
    public int rotationSpeed = 10;

    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0, Space.World);
    }

}
