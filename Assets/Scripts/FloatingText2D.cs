using UnityEngine;
using TMPro;

public class FloatingText2D : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    private Transform unit;
    private TMP_Text nameText;

    void Start()
    {
        unit = transform.parent;

        // Move to world-space canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        transform.SetParent(canvas.transform);

        // Get TMP component
        nameText = GetComponent<TMP_Text>();

        // Load player name
        string playerName = PlayerPrefs.GetString("PlayerID", "Player");
        nameText.text = playerName;
    }

    void LateUpdate()
    {
        // Follow player in 2D
        transform.position = unit.position + offset;
    }
}
