using UnityEngine;
using cherrydev;


public class DialogoInicio : MonoBehaviour
{
    [SerializeField]
    public DialogBehaviour dialogBehaviour;

    [SerializeField]
    public DialogNodeGraph dialogGraph;


    private void Start()
    {
        dialogBehaviour.BindExternalFunction("Test", DebugExternal);

        dialogBehaviour.StartDialog(dialogGraph);
    }

    private void DebugExternal()
    {
        Debug.Log("External function works!");
    }
}
