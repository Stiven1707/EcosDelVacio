using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGamePaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsMenuSceneLoaded())
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;

        // Guardar el estado del juego
        PlayerPrefs.SetInt("IsGamePaused", 1);
        PlayerPrefs.Save();

        // Pausar el tiempo del juego
        //Time.timeScale = 0f;

        // Cargar la escena del menú de manera aditiva
        SceneManager.LoadScene("Demo1", LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        // Reanudar el tiempo del juego
        //Time.timeScale = 1f;

        // Descargar la escena del menú solo si está cargada
        if (IsMenuSceneLoaded())
        {
            SceneManager.UnloadSceneAsync("Demo1").completed += (AsyncOperation op) =>
            {
                // Guardar el estado del juego
                PlayerPrefs.SetInt("IsGamePaused", 0);
                PlayerPrefs.Save();
            };
        }
        else
        {
            // Guardar el estado del juego
            PlayerPrefs.SetInt("IsGamePaused", 0);
            PlayerPrefs.Save();
        }
    }

    private bool IsMenuSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "Demo1")
            {
                return true;
            }
        }
        return false;
    }
}
