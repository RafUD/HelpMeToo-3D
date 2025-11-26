using TMPro;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static int coinsCollected = 0;
    public TextMeshProUGUI coinCountDisplay;

    [Header("Speed Stage Settings")]
    public int coinsForJog = 5;    
    public int coinsForRun = 10;   

    public enum SpeedStage { Walking, Jogging, Running }
    public static SpeedStage CurrentStage { get;  set; }

    void Awake()
    {
        ResetCoins();

        // Reset on scene load
        CurrentStage = SpeedStage.Walking;
    }

    void Update()
    {
        if (coinCountDisplay != null)
            coinCountDisplay.text = coinsCollected.ToString();

        // Determine current stage based on coins collected
        if (coinsCollected >= coinsForRun)
        {
            CurrentStage = SpeedStage.Running;
        }
        else if (coinsCollected >= coinsForJog)
        {
            CurrentStage = SpeedStage.Jogging;
        }
        else
        {
            CurrentStage = SpeedStage.Walking;
        }
    }

    public static void ResetCoins()
    {
        coinsCollected = 0;
        CurrentStage = SpeedStage.Walking;
    }
}