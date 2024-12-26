using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlayerQuest : Quest
{
    private void Start()
    {
        QuestName = "Slayer Quest";
        Description = "Slay all the slimes!";
        ItemReward = GameManager.instance.itemManager.GetItemByName("Arrow");

        Goals.Add(new KillGoal(this, 0, "Kill 3 Slimes", false, 0, 3));

        Goals.ForEach(g => g.Init());
    }
}
