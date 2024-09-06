using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Slot_UI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Button DropButton;
    public Image RarityBackLight;
    public RarityLevel slotItemsRarity; // could cause glitches

    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null)
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            
            if (slot.count > 1)
            {
                quantityText.text = slot.count.ToString();
            }
            else
            {
                quantityText.text = "";
            }

            DropButton.gameObject.SetActive(true);
            
            slotItemsRarity = slot.itemRarity;
            checkRarityLevel();
        }
    }

    public void setEmpty()
    {
        RarityBackLight.color = new Color(0, 0, 0, 0);
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
        DropButton.gameObject.SetActive(false);
    }

    public void checkRarityLevel()
    {
        if (slotItemsRarity == RarityLevel.Ledgendairy)
        {
            RarityBackLight.color = new Color(255, 190, 0, 1f);
        }
        else if (slotItemsRarity == RarityLevel.Epic)
        {
            RarityBackLight.color = new Color(252, 0, 255, 1f);
        }
        else if (slotItemsRarity == RarityLevel.Rare)
        {
            RarityBackLight.color = new Color(0, 137, 255, 1f);
        }
        else if (slotItemsRarity == RarityLevel.Uncommon)
        {
            RarityBackLight.color = new Color(0, 1, 0, 1f);
        }
        else
        {
            RarityBackLight.color = new Color(70, 70, 70, 1f);
        }
    }
}
