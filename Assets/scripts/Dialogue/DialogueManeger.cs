using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManeger : MonoBehaviour
{
    //aqui usamos do Sigleton para ter acesso a nossos metodos 
    private static DialogueManeger instance;
    //aqui pegamos a parte UI o painel e o texto
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    //aqui pegamos as escolhas tanto o gameobject como o texto
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    //aqui armazenaremos nossa historia
    private Story currentStory;
    //aqui e apenas uma variavel para verificarmos se o dialogo esta rodando
    public bool dialoguePlaying { get;private set;}
    //apenas garante que apenas uma instâcia do DialogueManeger exita
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
    }
    //Metodo de acesso, serve para ter acesso aos nossos metodos publicos
    public static DialogueManeger GetInstace()
    {
        return instance;
    }
    //quando o start for acionado ele disativa o painel e informa que o dialogo n esta rodando
    private void Start()
    {
        dialoguePlaying = false;
        dialoguePanel.SetActive(false);
        
        //aqui estamos declarando que o tamanho do nosso array de texto e o
        //tamanho de escolhas que temos no nosso gameobject choices
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

    }
    private void Update()
    {
        if (!dialoguePlaying)
        {
            return;
        }
        if (currentStory.currentChoices.Count == 0 && Input.GetKeyDown(KeyCode.I))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialoguePlaying = true;
        dialoguePanel.SetActive(true);
        
    }
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            //display choices
            DisplayChoices();
            
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More Choices were given than the UI can suport. Number of choices give: "+ currentChoices.Count);
        }
        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for(int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
        StartCoroutine(SelectFirstChoice());
    }
    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }
    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        
        ContinueStory();
    }
}
