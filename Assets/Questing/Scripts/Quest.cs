using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class Quest : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string QuestName { get; set; }
    public string Description { get; set; }
    public Item ItemReward { get; set; }
    public bool Completed { get; set; }

    public AdvancedNPCInteract Npc;

    private Player player;
    private void Start()
    {
        player = GameManager.instance.player;
    }
    public void CheckGoals(bool npcCalled)
    {
        Completed = Goals.All(g => g.Completed);

        if (npcCalled && Completed && this.isActiveAndEnabled)
        {
            GiveReward();
        }
        else if (!npcCalled && Completed)
        {
            for (int i = 0; i < GameManager.instance.NotebookScript.AllQuests.Count; i++)
            {
                if (GameManager.instance.NotebookScript.AllQuests[i] == this)
                {
                    GameManager.instance.NotebookScript.CurrentlySpawnedTextObjects[i].GetComponent<QuestCheckmark>().Check();
                }
            }
        }
    }
    public void GiveReward()
    {
        //addItem();
        Dropitem();
        Destroy(this);
    }

    private void Dropitem()
    {
        if (ItemReward != null)
        player.dropItem(ItemReward, Npc.transform.position);
    }

    private void AddItem()
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
                AddItem();
                Debug.Log("atempted to fix player nullreference");
            }
        }
    }
}