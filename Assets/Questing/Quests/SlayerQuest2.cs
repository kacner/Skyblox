using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlayerQuest2 : Quest
{
    private void Awake()
    {
        QuestName = "Slayer 2 Quest";
        Description = "Collect slime!";
        ItemReward = GameManager.instance.itemManager.GetItemByName("Rookie_Sword");

        Goals.Add(new CollectionGoal(this, "Slime_Chunk", "Collect 2 SlimeChunks", false, 0, 2));

        Goals.ForEach(g => g.Init());
    }
}