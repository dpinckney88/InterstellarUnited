using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SceneCompleteArgs : EventArgs
{
    public string ScenePath;
}

public class SceneStartArgs : EventArgs
{
    public string ScenePath;
}


public class NarrativeProcessor : MonoBehaviour
{
    private TextAsset scene;
    private List<Dialogue> processedDialogue = new List<Dialogue>();
    public GameObject portrait;
    // Start is called before the first frame update
    private enum State { inactive, active };
    private State currentState = State.inactive;
    private int currentDialogeLine = 0;
    private Coroutine textProcessing;
    private bool sceneEnd = false;
    private bool autoComplete = false;
    private bool isProcessingText = false;
    private bool paginating = false;
    private int pageNumber = 0;
    public static NarrativeProcessor instence;
    public TMP_Text dialogueBoxTextField;
    public TMP_Text dialogueBoxCharacterNameField;
    public static event EventHandler<SceneCompleteArgs> sceneComplete;
    public static event EventHandler<SceneStartArgs> sceneStart;
    public String pathToScene;
    private class Dialogue
    {
        public string characterName;
        public string dialogue;
        public string portraitAssetPath;
    }

    void Awake()
    {
        //Scene.loadScene += loadScene;
        //NPC.npcDialogue += loadScene;
        instence = this;
        gameObject.SetActive(false);
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (currentState == State.active)
        {
            //If we aren't currenlty paginating, ensure that we reset the page number to 0 to prevent issues with future paginated dialogue
            if (!paginating)
            {
                pageNumber = 0;
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                if (isProcessingText)
                {
                    autoComplete = true;
                }
                else if (paginating)
                {
                    pageNumber++;
                }
                else
                {
                    if (currentDialogeLine + 1 < processedDialogue.Count)
                    {
                        currentDialogeLine++;
                        DisplayDialogue(currentDialogeLine);
                    }
                    else
                    {
                        processedDialogue.Clear();
                        sceneComplete?.Invoke(this, new SceneCompleteArgs() { ScenePath = pathToScene });
                        currentState = State.inactive;
                        gameObject.SetActive(false);
                    }
                }
            }
        }
        else { }
    }

    //Returns a true if a scene is being run, false otherwise.
    public bool isActive()
    {
        if (currentState == State.active)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //  private void loadScene(object sender, LoadSceneArgs e)
    public void loadScene(string scenePath)
    {
        CanvasManager.instance.CloseAllGUI();
        sceneStart?.Invoke(this, new SceneStartArgs() { ScenePath = scenePath });
        pathToScene = scenePath;
        StartConversation(pathToScene);
    }

    private void ProcessCSV()
    {
        String[] sceneLine = scene.ToString().Split("\n");
        int lineNum = 0;
        foreach (string line in sceneLine)
        {
            String[] lineByCell = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            if (lineNum == 0 || lineByCell.Length < 2)
            {
                lineNum = 1;
                continue;
            }

            Dialogue temp = new Dialogue();
            temp.characterName = lineByCell[0].Trim();
            temp.dialogue = lineByCell[1].Trim();
            temp.portraitAssetPath = lineByCell[2].Trim();
            processedDialogue.Add(temp);
        }
    }

    public void StartConversation(String path)
    {
        //Load the scene
        scene = Resources.Load<TextAsset>(path);
        //Process the scene
        ProcessCSV();
        //Initiate the Dialogue
        InitiateDialogue();

    }

    private void InitiateDialogue()
    {
        //Set the state to active
        currentState = State.active;
        //gameObject.SetActive(true);
        CanvasManager.instance.ShowGUI("Visual Novel");
        //Automatically start and show the first line of text
        DisplayDialogue();
    }

    private void DisplayDialogue(int lineNum = 0)
    {
        if (textProcessing != null)
        {
            StopCoroutine(textProcessing);
        }
        currentDialogeLine = lineNum;
        String character = processedDialogue[currentDialogeLine].characterName;
        portrait.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Characters/" + processedDialogue[currentDialogeLine].portraitAssetPath);
        dialogueBoxCharacterNameField.text = character;
        textProcessing = StartCoroutine(TyperWritterTextCO());

    }

    IEnumerator TyperWritterTextCO()
    {
        paginating = true;
        List<String> textPages = PaginateText(processedDialogue[currentDialogeLine].dialogue);
        int workingPageNumber = 0;
        foreach (String dialogue in textPages)
        {
            //String dialogue = textPages[pageNumber];
            String displayedText = "";
            Char[] dialogueLetters = dialogue.ToCharArray();

            int letterCounter = 0;


            while (displayedText != dialogue)
            {
                if (dialogueLetters.Length == 0)
                {
                    yield break;
                }

                if (autoComplete)
                {
                    displayedText = dialogue;
                    dialogueBoxTextField.text = displayedText;
                    break;
                }

                isProcessingText = true;
                displayedText += dialogueLetters[letterCounter];
                dialogueBoxTextField.text = displayedText;
                letterCounter++;
                yield return new WaitForSeconds(.08f);
            }

            autoComplete = false;
            isProcessingText = false;

            if (pageNumber < textPages.Count - 1)
            {
                //If the page number hasn't advanced, wait until the player advances.
                while (workingPageNumber == pageNumber)
                {
                    yield return null;
                }
                //Track the current working page
                workingPageNumber = pageNumber;
            }
        }
        paginating = false;
        yield break;
    }

    private List<String> PaginateText(String dialogue, int length = 200)
    {
        List<String> paginatedText = new List<String> { };
        String workingText = dialogue;
        //If the dialouge is less than or equal to the specified page length, then just send it back in a list
        if (dialogue.Length <= length)
        {
            return new List<String> { dialogue };
        }

        int pageLength = length;

        while (workingText.Length != 0)
        {
            //Find how long the page can be. We do want to check what the pageLength because we want to check if the next char is a space char indicating the end of a word.
            int pageLengthCheck = (pageLength + 1 > workingText.Length) ? workingText.Length : pageLength + 1;

            //Use the string if:
            //1) The length of the page is equal to how many characters are left or
            //2) The next char is a space, indicating that the substring has found the end of a word or
            //3) If somehow the string is a single word that is the length of the page, use it.
            if (pageLengthCheck == workingText.Length || workingText[pageLengthCheck] == ' ' || pageLength == 0)
            {
                paginatedText.Add(workingText.Substring(0, pageLengthCheck).Trim());
                workingText = workingText.Substring(pageLengthCheck).Trim();
                pageLength = length;
            }
            else
            {
                pageLength--;
            }
        }

        return paginatedText;
    }
}
