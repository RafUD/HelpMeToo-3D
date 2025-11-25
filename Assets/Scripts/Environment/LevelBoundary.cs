using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    public static float leftSide = -77.8f;
    public static float rightSide = -73.0f;
    public float internalLeftSide;
    public float internalRightSide;

    // Update is called once per frame
    void Update()
    {
        internalLeftSide = leftSide;
        internalRightSide = rightSide;
        
    }
}
