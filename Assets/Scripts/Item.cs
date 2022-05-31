using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] Button Button_Purchase, Button_Purchased;


    public void Purchased()
    {
        Button_Purchase.gameObject.SetActive(false);
        Button_Purchased.gameObject.SetActive(true);
    }

    public void NotPurchased()
    {
        Button_Purchase.gameObject.SetActive(true);
        Button_Purchased.gameObject.SetActive(false);
    }
}
