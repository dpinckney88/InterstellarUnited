using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneArgs : EventArgs
{
    public string scenePath;
}
public abstract class Scene
{
    protected StoryEvents storyEvents;
    public static event EventHandler<LoadSceneArgs> loadScene;
    // Start is called before the first frame update
    protected virtual void OnEnter()
    {

    }

    public void OnStateEnter(StoryEvents se)
    {
        storyEvents = se;
        OnEnter();
    }

    protected virtual void OnUpdate()
    {

    }

    public void OnStateUpdate()
    {
        OnUpdate();
    }

    public void OnStateStart()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {

    }

    protected void LoadScene(String scene)
    {
        NarrativeProcessor.instence.loadScene(scene);
        //loadScene?.Invoke(this, new LoadSceneArgs() { scenePath = scene });
    }
}
