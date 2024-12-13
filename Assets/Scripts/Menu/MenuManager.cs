using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button btnContinue; // Referencia al bot�n Btn_Continue

    void Start()
    {
        // Asegurarse de que el bot�n est� desactivado al inicio
        if (btnContinue != null)
        {
            btnContinue.gameObject.SetActive(false);
        }

        // Activar el bot�n si el juego est� pausado
        if (IsJuegoSceneLoaded())
        {
            if (btnContinue != null)
            {
                btnContinue.gameObject.SetActive(true);
            }
        }
    }

    public void ContinueGame()
    {
        StartCoroutine(UnloadMenuScene());
    }

    private IEnumerator UnloadMenuScene()
    {
        // Descargar la escena del men� solo si est� cargada
        if (SceneManager.GetSceneByName("Demo1").isLoaded)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("Demo1");
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }

        // Reanudar el tiempo del juego
        FindObjectOfType<GameManager>().ResumeGame();
    }
    private bool IsJuegoSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "edwinespj")
            {
                return true;
            }
        }
        return false;
    }
}
