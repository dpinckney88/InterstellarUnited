using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FirstDayOnTheJob : IQuest
{

    [field: SerializeField] public string questName { get; private set; } = "First Day On The Job";

    private List<QuestObjective> questObjectives = new List<QuestObjective>();

    public event EventHandler<QuestcompleteArgs> questObjectiveComplete;
    public event EventHandler<QuestStartedArgs> questStarted;

    public List<QuestObjective> currentObjective = new List<QuestObjective>();

    public List<QuestObjective> GetNextObjective()
    {

        if (questObjectives.Count == 0)
        {
            CompletedQuest();
        }

        QuestObjective temp = questObjectives[0];
        currentObjective.Add(temp);
        questObjectives.RemoveAt(0);
        return new List<QuestObjective>() { temp };
    }

    // Start is called before the first frame update
    public void Setup()
    {
        questObjectives.Add(new Objective1());
        questObjectives.Add(new Objective2());
        questObjectives.Add(new Objective3());
        questObjectives.Add(new Objective4());
        questStarted?.Invoke(this, new QuestStartedArgs() { quest = this });
    }

    public void CompletedObjective(QuestObjective objective)
    {
        currentObjective.Clear();
        objective.OnStateExit();
        questObjectiveComplete?.Invoke(this, new QuestcompleteArgs() { questObjective = objective, quest = this });
    }

    public void CompletedQuest()
    {
        //Dispatch a message
    }

    public List<QuestObjective> getCurrentObjective()
    {
        return currentObjective;
    }
}
