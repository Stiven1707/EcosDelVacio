using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;  

public class GameManager : MonoBehaviour
{
    private bool isGamePaused = false;
    public bool IsGamePaused()
    {
        return isGamePaused;
    }
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
        if (IsMenuSceneLoaded() && IsEdwinespjSceneLoaded())
        {
            
            GameObject astronautIdle = GameObject.Find("AstronautIdle");
            if (astronautIdle != null)
            {
                astronautIdle.GetComponent<PlayerMovement>().enabled = false;
            }

            // Buscar las cámaras en la escena "edwinespj"
            Scene edwinespjScene = SceneManager.GetSceneByName("edwinespj");
            if (edwinespjScene.IsValid())
            {
                GameObject[] rootObjects = edwinespjScene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    Camera camera = obj.GetComponentInChildren<Camera>();
                    if (camera != null)
                    {
                        AudioSource audioSource = camera.GetComponent<AudioSource>();
                        if (audioSource != null)
                        {
                            audioSource.enabled = false;
                            break; // Salir del bucle una vez que se encuentra la cámara correcta
                        }
                    }
                }

                // Buscar el canvas en la escena "edwinespj"
                foreach (GameObject obj in rootObjects)
                {
                    Canvas canvas = obj.GetComponentInChildren<Canvas>();
                    if (canvas != null)
                    {
                        canvas.enabled = false;

                        // Buscar el OxygenBar dentro del Canvas
                        Transform imaOxygenBar = canvas.transform.Find("ImaOxygenBar");
                        if (imaOxygenBar != null)
                        {
                            GameObject oxygenBar = imaOxygenBar.Find("OxygenBar")?.gameObject;
                            if (oxygenBar != null)
                            {
                                // Desactivar los componentes del OxygenBar
                                foreach (var component in oxygenBar.GetComponents<MonoBehaviour>())
                                {
                                    component.enabled = false;
                                }
                            }
                        }
                        break; // Salir del bucle una vez que se encuentra el canvas
                    }
                }
            }
        }
        else
        {
            // Dejar todo como estaba
            GameObject astronautIdle = GameObject.Find("AstronautIdle");
            if (astronautIdle != null)
            {
                astronautIdle.GetComponent<PlayerMovement>().enabled = true;
            }

            // Buscar las cámaras en la escena "edwinespj"
            Scene edwinespjScene = SceneManager.GetSceneByName("edwinespj");
            if (edwinespjScene.IsValid())
            {
                GameObject[] rootObjects = edwinespjScene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    Camera camera = obj.GetComponentInChildren<Camera>();
                    if (camera != null)
                    {
                        AudioSource audioSource = camera.GetComponent<AudioSource>();
                        if (audioSource != null)
                        {
                            audioSource.enabled = true;
                            break; // Salir del bucle una vez que se encuentra la cámara correcta
                        }
                    }
                }

                // Buscar el canvas en la escena "edwinespj"
                foreach (GameObject obj in rootObjects)
                {
                    Canvas canvas = obj.GetComponentInChildren<Canvas>();
                    if (canvas != null)
                    {
                        canvas.enabled = true;

                        // Buscar el OxygenBar dentro del Canvas
                        Transform imaOxygenBar = canvas.transform.Find("ImaOxygenBar");
                        if (imaOxygenBar != null)
                        {
                            GameObject oxygenBar = imaOxygenBar.Find("OxygenBar")?.gameObject;
                            if (oxygenBar != null)
                            {
                                // Activar los componentes del OxygenBar
                                foreach (var component in oxygenBar.GetComponents<MonoBehaviour>())
                                {
                                    component.enabled = true;
                                }
                            }
                        }
                        break; // Salir del bucle una vez que se encuentra el canvas
                    }
                }
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
        StartCoroutine(LoadAndSetActiveScene("Demo1"));
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
        SetActiveScene("edwinespj");
    }

    public bool IsMenuSceneLoaded()
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

    // Quiero saber cuando la escena edwinespj esté cargada
    public bool IsEdwinespjSceneLoaded()
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
    private IEnumerator LoadAndSetActiveScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Esperar a que la escena se cargue completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Establecer la escena como activa
        SetActiveScene(sceneName);
    }
    private void SetActiveScene(string sceneName)
    {
        Scene newActiveScene = SceneManager.GetSceneByName(sceneName);
        if (newActiveScene.IsValid() && newActiveScene.isLoaded)
        {
            SceneManager.SetActiveScene(newActiveScene);
            Debug.Log("Active scene set to: " + sceneName);
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " is not loaded or valid.");
        }
    }
}

