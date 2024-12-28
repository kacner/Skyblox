using System.Diagnostics;

public class CollectionGoal : Goal
{
    public string ItemID { get; set; }
    
    public CollectionGoal(Quest quest, string itemID, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.Quest = quest;
        this.ItemID = itemID;
        this.description = description;
        this.Completed = completed;
        this.currentAmount = currentAmount;
        this.requiredAmount = requiredAmount;
    }

    public override void Init()
    {
        base.Init();
        ItemEvents.OnItemPickedUp += itemPickedUp;
    }

    void itemPickedUp(string item) //has killed enemy
    {
        if (item == this.ItemID)
        {
            this.currentAmount++;
            evaluate();
        }
    }
}
public static class ItemEvents
{
    public static event System.Action<string> OnItemPickedUp;

    public static void ItemPickedUp(string item)
    {
        OnItemPickedUp?.Invoke(item);
    }
}
