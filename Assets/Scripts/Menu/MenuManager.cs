using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button btnContinue; // Referencia al botón Btn_Continue

    void Start()
    {
        // Asegurarse de que el botón esté desactivado al inicio
        if (btnContinue != null)
        {
            btnContinue.gameObject.SetActive(false);
        }

        // Activar el botón si el juego está pausado
        if (PlayerPrefs.GetInt("IsGamePaused", 0) == 1)
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
        // Descargar la escena del menú solo si está cargada
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
}
