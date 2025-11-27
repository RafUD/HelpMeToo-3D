using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    public static float leftSide = -81f;
    public static float rightSide = -71f;
    public float internalLeftSide;
    public float internalRightSide;

    // Update is called once per frame
    void Update()
    {
        internalLeftSide = leftSide;
        internalRightSide = rightSide;
        
    }
}
