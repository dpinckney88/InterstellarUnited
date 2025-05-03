using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestcompleteArgs : EventArgs
{
    public QuestObjective questObjective;
    public IQuest quest;
    //public IQuest quest;
}


public class QuestStartedArgs : EventArgs
{
    public IQuest quest;
    //public IQuest quest;
}


public interface IQuest
{
    public event EventHandler<QuestcompleteArgs> questObjectiveComplete;
    public event EventHandler<QuestStartedArgs> questStarted;
    public String questName { get; }
    public List<QuestObjective> GetNextObjective();
    public void CompletedObjective(QuestObjective objective);
    public void Setup();
    public List<QuestObjective> getCurrentObjective();
}
