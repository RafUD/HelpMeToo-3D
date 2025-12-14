using UnityEngine;
using TMPro;

public class FloatingText3D : MonoBehaviour
{
    Transform mainCam;
    Transform unit;
    Transform worldSpaceCanvas;

    public Vector3 offset;

    private TMP_Text nameText;

    void Start()
    {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        worldSpaceCanvas = GameObject.FindFirstObjectByType<Canvas>().transform;

        transform.SetParent(worldSpaceCanvas);

        // Get TMP component
        nameText = GetComponent<TMP_Text>();

        // Load player name
        string playerName = PlayerPrefs.GetString("PlayerID", "Player");

        // Assign text
        nameText.text = playerName;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(
            transform.position - mainCam.position
        );

        transform.position = unit.position + offset;
    }
}
