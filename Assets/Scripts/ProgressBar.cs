using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    int maximum = 300;
    public int current;
    public Image mask;

    private void Start()
    {
        current = int.Parse(PlayerStatsManager.XP);
        float fillAmount = (float)current / (float)maximum;
        mask.fillAmount = fillAmount;
    }
}
