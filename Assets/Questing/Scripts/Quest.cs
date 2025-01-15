using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        {
            GiveReward();
        }
    }
    public void GiveReward()
    {
        addItem();
        for (int i = 0; i < GameManager.instance.NotebookScript.AllQuests.Count; i++)
        {
            if (GameManager.instance.NotebookScript.AllQuests[i] == this)
            {
                GameManager.instance.NotebookScript.SpawnedTextObjects[i].GetComponent<QuestCheckmark>().Check();
            }
        }
        //GameManager.instance.NotebookScript.AllQuests.Remove(this);
        Destroy(this);
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