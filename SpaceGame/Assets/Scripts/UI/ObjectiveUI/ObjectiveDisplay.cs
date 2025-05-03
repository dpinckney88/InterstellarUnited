using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDisplay : MonoBehaviour
{
    [SerializeField] private GameObject objectiveBlockPrefab;
    [SerializeField] private GameObject objectiveStack;
    private Dictionary<IQuest, GameObject> displayedQuests = new Dictionary<IQuest, GameObject>();

    public static ObjectiveDisplay instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayQuest(IQuest quest)
    {
        GameObject objectiveBlock;
        if (!displayedQuests.TryGetValue(quest, out objectiveBlock))
        {
            objectiveBlock = GameObject.Instantiate(objectiveBlockPrefab);
            objectiveBlock.transform.SetParent(objectiveStack.transform);
            displayedQuests.Add(quest, objectiveBlock);
        }

        objectiveBlock.GetComponent<ObjectiveBlock>().objectiveName.text = quest.questName;
        objectiveBlock.GetComponent<ObjectiveBlock>().currentObjective.text = quest.getCurrentObjective()[0].ObjectiveDescription();

    }
}
