using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{

    public Image heart;
    public Sprite allHeartsSprite;
    public Sprite noHeartsSprite;


    private List<Image> hearts = new List<Image>();

    public void SetMaxHearts(int maxHearts)
    {
        foreach (Image heart in hearts) 
        { 
            Destroy(heart.gameObject);    
        }

        hearts.Clear();

        for(int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heart, transform);
            newHeart.sprite = allHeartsSprite;
            hearts.Add(newHeart);
        }
    }

    public void UpdateHearts(int currentHearts)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHearts) 
            {
                hearts[i].sprite = allHeartsSprite;
            }
            else
            {
                hearts[i].sprite = noHeartsSprite;
            }
        }
    }
}
