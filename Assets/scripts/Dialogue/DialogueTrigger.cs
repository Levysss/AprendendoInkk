using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //aqui pegamos o game object ou sprite 
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    //aqui pegamos o doc ink
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    //e uma variavel boleana para ver se esta no Ranger da interação
    private bool playerInRanger;
    private void Awake()
    {
        //declaramos que no inicio que não estamos no ranger
        playerInRanger = false;
        //e desativamos o game object 
        visualCue.SetActive(false);
    }
    private void Update()
    {
        //verificamos se o player esta no range e se o dialo NÃO esta ativo
        if (playerInRanger && !DialogueManeger.GetInstace().dialoguePlaying)
        {
            //se passar ativamos a sprite de interação
            visualCue.SetActive(true);
            //verificando se a pessoa apertou "i" para interagir
            if (Input.GetKeyDown(KeyCode.I))
            {
                //mandando o ink para o dialogueManeger
                DialogueManeger.GetInstace().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            //se não passar nossa sprite continua desativada!
            visualCue.SetActive(false);
        }
    }

    //metodo para quando entrar no trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRanger = true;
        }
        
    }
    //metodo para quando sair do trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRanger = false;
        }
    }
}
