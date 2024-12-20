using cherrydev;
using SlimUI.ModernMenu;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DiagoloCapsula : MonoBehaviour
{
    [SerializeField]
    public DialogBehaviour dialogBehaviour;

    [SerializeField]
    public DialogNodeGraph dialogGraph;

    private bool dialogFinished = false;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        if (CheckAudioReady())
        {
            dialogBehaviour.BindExternalFunction("Test", DebugExternal);
            dialogBehaviour.StartDialog(dialogGraph);
            dialogBehaviour.AddListenerToDialogFinishedEvent(OnDialogFinished);
        }
    }

    public void inicio()
    {
        Start();
    }

    private void Update()
    {
        if (dialogFinished) return;

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (dialogBehaviour.gameObject.activeSelf && gameManager != null && gameManager.IsMenuSceneLoaded() && gameManager.IsGameSceneLoaded())
        {
            dialogBehaviour.PauseDialog();
            dialogBehaviour.gameObject.SetActive(false);
        }
        else if (gameManager != null && !gameManager.IsMenuSceneLoaded() && !dialogBehaviour.gameObject.activeSelf)
        {
            dialogBehaviour.gameObject.SetActive(true);
            dialogBehaviour.ResumeDialog();
        }
    }

    private bool CheckAudioReady()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        return gameManager != null && !gameManager.IsMenuSceneLoaded() && gameManager.IsGameSceneLoaded();

        //TODO cuando empezar

    }

    private void DebugExternal()
    {
        Debug.Log("External function works!");
    }

    private void OnDialogFinished()
    {
        dialogFinished = true;
    }

    public void ReiniciarDialogo()
    {
        dialogFinished = false;
        dialogBehaviour.StartDialog(dialogGraph);
    }
}