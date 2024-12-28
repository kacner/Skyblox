using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        {
            GiveReward();
        }
    }
    public void GiveReward()
    {
        addItem();
    }

    void addItem()
    {
        if (ItemReward != null)
        {
            if (player != null)
            {
                player.inventoryManager.AddBasedOnItem(ItemReward);
                print("Added " + ItemReward.data.itemName);
            }
            else
            {
                player = GameManager.instance.player;
                addItem();
                Debug.Log("atempted to fix player nullreference");
            }
        }
    }
}
