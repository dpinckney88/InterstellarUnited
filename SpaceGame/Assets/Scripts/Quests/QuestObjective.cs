using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class QuestObjective
{
    protected StoryEvents storyEvents;
    protected IQuest parentQuest;
    protected virtual void OnEnter()
    {

    }

    public void OnStateEnter(StoryEvents se, IQuest quest)
    {
        storyEvents = se;
        parentQuest = quest;
        TrackingPoint();
        OnEnter();
    }

    protected virtual void OnUpdate()
    {

    }

    public void OnStateUpdate()
    {
        OnUpdate();
    }

    protected virtual void OnExit()
    {

    }

    public void OnStateExit()
    {
        OnExit();
    }

    protected void LoadScene(String scene)
    {
        NarrativeProcessor.instence.loadScene(scene);
        //loadScene?.Invoke(this, new LoadSceneArgs() { scenePath = scene });
    }



    protected void SetupScene(String scene)
    {
        storyEvents.ShowVisualNovel(true);
        storyEvents.ShowShipUI(false);
        LoadScene(scene);
    }

    public virtual string ObjectiveDescription()
    {
        return "";
    }

    public virtual Transform TrackingPoint()
    {
        return null;
    }
}
