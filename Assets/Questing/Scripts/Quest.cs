using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;
using Unity.VisualScripting;

public class Quest : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string QuestName { get; set; }
    public string Description { get; set; }
    public int ExperienceReward { get; set; }
    public Item ItemReward { get; set; }
    public bool Completed { get; set; }

    private Player player;
    private void Start()
    {
        player = GameManager.instance.player;
    }
    public void CheckGoals()
    {
        Completed = Goals.All(g => g.Completed);

        if (Completed)
            GiveReward();
    }
    public void GiveReward()
    {
        addItem();
    }

    void addItem()
    {
        if (ItemReward != null)
        {
            ItemData itemData = ItemReward.data;
            if (itemData is WeapondData)
            {
                player.inventory.Add("Toolbar", ItemReward);
                GameManager.instance.ui_Manager.RefreshInventoryUI("Toolbar");
            }
            else
                player.inventory.Add("Backpack", ItemReward);
        }
    }
}
