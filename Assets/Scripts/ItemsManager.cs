using TMPro;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static int coinsCollected = 0;
    public TextMeshProUGUI coinCountDisplay;

    void Update()
    {
        coinCountDisplay.text = "" + coinsCollected;
    }
}
