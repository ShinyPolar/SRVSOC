using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;

    public static string DisplayName, PlayFabID;

    public static string Level, XP;

    public static int Gold;

    public static bool bRaq, bExtraLife;
    public static string ExtraLifeID;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
