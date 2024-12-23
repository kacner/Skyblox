using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;
using Unity.Burst.Intrinsics;

public class Slot_UI : MonoBehaviour
{
    [Header("Genral")]
    public int slotID;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Button DropButton;

    [Space(10)]

    [Header("RaritySettings")]
    public Image RarityBackLight;
    public Rarity slotItemsRarity;

    [Space(10)]

    [Header("ArmorSettings")]
    public bool isArmorSlot = false;
    public ArmorType WantsArmorType;
    public ArmorType SlothasArmorType;
    public Image ArmorIcon;

    [Space(10)]

    [Header("Misc")]
    public Inventory inventory;
    [SerializeField] private GameObject highLight;
    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null)
        {
            if (DropButton)
                DropButton.gameObject.SetActive(true);

            if (isArmorSlot)
                ArmorIcon.enabled = false;

            if (RarityBackLight != null)
            RarityBackLight.enabled = true;

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
            
            slotItemsRarity = slot.itemRarity;
            SlothasArmorType = slot.HasArmorType;
            checkRarityLevel();
        }
        else
        {
            print("fail1");
        }
    }

    public void setEmpty()
    {
        if (DropButton)
            DropButton.gameObject.SetActive(false);

        if (isArmorSlot)
            ArmorIcon.enabled = true;

        if (RarityBackLight != null)
            RarityBackLight.color = new Color(0, 0, 0, 0);

        if (itemIcon != null)
            itemIcon.sprite = null;

        if (itemIcon != null)
            itemIcon.color = new Color(1, 1, 1, 0);

        if (quantityText != null)
            quantityText.text = "";

        if (DropButton != null)
            DropButton.gameObject.SetActive(false);

        slotItemsRarity = Rarity.None;
        SlothasArmorType = ArmorType.None;
      
    }

    public void SetHighLight(bool isOn)
    {
        highLight.SetActive(isOn);
    }

    public void checkRarityLevel()
    {
        if (RarityBackLight != null)
        {
            if (slotItemsRarity == Rarity.Ledgendairy)
            {
                RarityBackLight.color = new Color(255, 190, 0, 1f);
            }
            else if (slotItemsRarity == Rarity.Epic)
            {
                RarityBackLight.color = new Color(252, 0, 255, 1f);
            }
            else if (slotItemsRarity == Rarity.Rare)
            {
                RarityBackLight.color = new Color(0, 137, 255, 1f);
            }
            else if (slotItemsRarity == Rarity.Uncommon)
            {
                RarityBackLight.color = new Color(0, 1, 0, 1f);
            }
            else
            {
                RarityBackLight.color = new Color(70, 70, 70, 1f);
            }
        }
    }
}
