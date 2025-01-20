public class Goal
{
    public Quest Quest { get; set; }
    public string description { get; set; }
    public bool Completed { get; set; }
    public int currentAmount { get; set; }
    public int requiredAmount { get; set; }

    public virtual void Init()
    {
        // default init 
    }
    public void evaluate(bool npcCalled = false)
    {
        if (currentAmount >= requiredAmount)
        {
            Complete(npcCalled);
        }
    }

    public void Complete(bool npcCalled)
    {
        Quest.CheckGoals(npcCalled);
        Completed = true;
    }
}
