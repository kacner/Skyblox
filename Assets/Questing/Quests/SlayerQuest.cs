public class SlayerQuest : Quest
{
    private void Awake()
    {
        QuestName = "Slayer Quest";
        Description = "Slay Slimes!";
        ItemReward = GameManager.instance.itemManager.GetItemByName("Rookie_Bow");

        Goals.Add(new KillGoal(this, 0, "Murder 3 Slimes", false, 0, 3));
        Goals.ForEach(g => g.Init());
    }
}