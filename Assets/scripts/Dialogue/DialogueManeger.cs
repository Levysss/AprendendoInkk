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
    //
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;
    //aqui pegamos as escolhas tanto o gameobject como o texto
    //variaves onde armazenaremos os textos das escolhas e o numero das escolhas
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    //aqui armazenaremos nossa historia
    private Story currentStory;
    //aqui e apenas uma variavel para verificarmos se o dialogo esta rodando
    public bool dialoguePlaying { get;private set;}

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    //apenas garante que apenas uma instâcia do DialogueManeger exita
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Encontrou mais de um Gerenciador de Diálogos na cena");
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

        layoutAnimator = dialoguePanel.GetComponent<Animator>();
        
        //aqui estamos declarando que o tamanho do nosso array de texto e o
        //tamanho de escolhas que temos no nosso gameobject choices
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            //aqui pegamos os componetes de textMesh e colocamos nos textos 
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

    }
    //verifica se ta rodando o dialogo
    private void Update()
    {
        if (!dialoguePlaying)
        {
            //se não tiver rodando continua o programa
            return;
        }
        if (currentStory.currentChoices.Count == 0 && Input.GetKeyDown(KeyCode.I))
        {
            //se a escolha atual não ouver escolha e apertou "I"
            //continue a historia
            ContinueStory();
        }
    }
    //metodo publico para pegarmos dialogos aleios :3
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        //passamos a historia para nosso currentStory q é nossa historia atual
        //e declaramos que nosso dialogo esta tocando
        //e ativamos nosso painel
        currentStory = new Story(inkJSON.text);
        dialoguePlaying = true;
        dialoguePanel.SetActive(true);
        
    }

    //criamos um metodo para que quando terminar a interação
    private IEnumerator ExitDialogueMode()
    {
        //aqui colocamos um cooldown para execultar a tarefa
        yield return new WaitForSeconds(0.3f);
        //é depois informamos que o dialogo "rodando" agora e falso
        //desativamos o painel e declaramos que o texto do dialogo e igual a ""(apagamos oq estava
        //escrito).
        dialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    //metodo para continuar a historia
    private void ContinueStory()
    {
        //verificamos se exite a possibilidade de continuar a historia
        if (currentStory.canContinue)
        {
            //aqui declaramos que nosso texto mudara para a proxima linha ou parte da historia
            dialogueText.text = currentStory.Continue();
            //display choices
            DisplayChoices();
            //metodo para pegar as tegs da minha historia atual
            HandleTags(currentStory.currentTags);
            
        }
        else
        {
            //aqui caso não tenha como continuar a historia ele saia.
            StartCoroutine(ExitDialogueMode());
        }
    }
    //metodo para pegar a tag 
    //fazemos uma lista de string para receber todas as tags do doc ink.

    private void HandleTags(List<string> currentTags)
    {
        /*
        fiz isso para verificar os items que eram retornados
        e deduzi que a informação da variavel currentTags retorna uma lista de tags
        que estão gravadas no diaologo atual.
        foreach (string tags in currentTags )
        {
            Debug.Log(tags);
        }
        */
        foreach (string tag in currentTags) 
        {
            //aqui dividimos o as tags ou seja em vez de ficar retornado a tag e o valor
            //jutos vamos retornar a tag dps o valor ou seja no index [0] vai vir o nome
            //da tag e no index [1] o valor que atribuimos a tag.
            string[] splitTag = tag.Split(':');
            /*
            outro debug para analizar oq tava realmente acontecendo
            foreach (string tags in splitTag)
            {
                Debug.Log(tags);
            }
            */
            //o splitTag sempre deve retornar o nome da tag e o valor dentro dela
            //caso o tamanho seja diferente de dois signifacaria que a tag ta recebendo
            //mais de um valor ou valor nem um, isso não deveria acontecer
            if (splitTag.Length != 2)
            {
                Debug.LogError("A tag não pôde apropriadamente:" + tag);
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey) 
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("A tag chegou, mas não está sendo tratada no momento:" + tag);
                    break;
            }
        }
    }

    //metodos das exposição das escolhas
    private void DisplayChoices()
    {
        //criamos uma lista de Choice para armazenarmos as escolhas atuais
        List<Choice> currentChoices = currentStory.currentChoices;
        //aqui fazemos um tratamento de erro caso exista mais escolhase do que botões ele de um erro
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Mais opções foram dadas do que a interface do usuário pode suportar. Número de opções dar: " + currentChoices.Count);
        }
        //aqui passamos as escolhas que temos no
        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            //para cada escolha que exita vamos abilitar o botão
            //depois vamos pegar o texto do mesmo botão e colocar o texto do botão 
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        //depois desative todos os botões
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
    //metodo publico para utilizarmos no OnClick() serve para informamos ao ink qual foi
    //a escolha que foi selecionada
    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        
    }
}
