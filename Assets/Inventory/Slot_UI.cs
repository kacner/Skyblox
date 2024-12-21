using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;


public class Slot_UI : MonoBehaviour
{
    public int slotID;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Button DropButton;
    public Image RarityBackLight;
    public string slotItemsRarity;
    public Inventory inventory;
    public bool WantsArmorItem = false;
    [SerializeField] public ArmorType WantsArmorType;
    public ArmorType SlothasArmorType;
    [SerializeField] private GameObject highLight;

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
      
    }

    public void SetHighLight(bool isOn)
    {
        highLight.SetActive(isOn);
    }

    public void checkRarityLevel()
    {
        if (RarityBackLight != null)
        {
            if (slotItemsRarity == "Ledgendairy")
            {
                RarityBackLight.color = new Color(255, 190, 0, 1f);
            }
            else if (slotItemsRarity == "Epic")
            {
                RarityBackLight.color = new Color(252, 0, 255, 1f);
            }
            else if (slotItemsRarity == "Rare")
            {
                RarityBackLight.color = new Color(0, 137, 255, 1f);
            }
            else if (slotItemsRarity == "Uncommon")
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
