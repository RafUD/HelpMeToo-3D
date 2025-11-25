using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    public static float leftSide = -80f;
    public static float rightSide = -72.6f;
    public float internalLeftSide;
    public float internalRightSide;

    // Update is called once per frame
    void Update()
    {
        internalLeftSide = leftSide;
        internalRightSide = rightSide;
        
    }
}
