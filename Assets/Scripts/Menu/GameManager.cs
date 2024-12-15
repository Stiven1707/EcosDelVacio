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
    private bool wasPodAudioPlaying = false;
    private bool wasCanvasCActive = false;
    private bool wasCanvasMActive = false;
    private bool wasCanvasActive = false;
    private bool isDeathScreenShown = false; // Variable de estado para la pantalla de muerte

    [Header("Pantalla de Transición")]
    public GameObject transitionScreen; // Pantalla de transición
    public TMP_Text transitionText; // Texto de la pantalla de transición
    public Slider loadingBar; // Barra de carga
    public TMP_Text loadPromptText; // Texto de la pantalla de carga
    public KeyCode userPromptKey = KeyCode.Return; // Tecla para continuar
    public string deathMessage = "Game Over..."; // Mensaje de muerte
    // Mensaje de victoria
    public string victoryMessage = "¡You Win! - Chapter 1 Completed";

    // Nombres de las escenas
    private string menuSceneName = "Demo1";
    private string gameSceneName = "edwinespj";

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsMenuSceneLoaded())
        {
            TogglePause();
        }

        if (IsMenuSceneLoaded() && IsGameSceneLoaded())
        {
            HandleGameScene(false);
        }
        else
        {
            HandleGameScene(true);
        }
        // Verificar si el personaje ha muerto
        CheckForDeath();
    }

    private void CheckForDeath()
    {
        // Obtener la referencia al SliderOxygenBar
        SliderOxygenBar oxygenBar = FindObjectOfType<SliderOxygenBar>();
        if (oxygenBar != null && oxygenBar.IsDead && !isDeathScreenShown)
        {
            ShowDeathScreen();
        }
        //Prueba Input.GetKeyDown(KeyCode. oprimir M para mostrar pantalla de muerte
        if (Input.GetKeyDown(KeyCode.M) && !isDeathScreenShown)
        {
            ShowDeathScreen();
        }
    }

    private void ShowDeathScreen()
    {
        isDeathScreenShown = true; // Marcar que la pantalla de muerte se ha mostrado

        if (transitionScreen != null && transitionText != null)
        {
            transitionText.text = deathMessage;
            transitionScreen.SetActive(true);
        }
        SetPlayerMovement(false);
        SetCameraAudio(false);
        SetPodAudio(false);

        // Iniciar la transición a la escena del menú principal
        StartCoroutine(LoadMenuScene(menuSceneName));
    }

    private IEnumerator LoadMenuScene(string sceneName)
    {
        // Activar la pantalla de transición
        if (transitionScreen != null)
        {
            transitionScreen.SetActive(true);
        }

        // Iniciar la carga de la escena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Desactivar la activación automática de la escena
        asyncLoad.allowSceneActivation = false;

        // Actualizar la barra de carga
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }

            // Mostrar el mensaje de espera
            if (asyncLoad.progress >= 0.9f)
            {
                if (loadPromptText != null)
                {
                    loadPromptText.text = "Presiona " + userPromptKey.ToString().ToUpper() + " para continuar";
                }

                // Esperar la entrada del usuario
                while (!Input.GetKeyDown(userPromptKey))
                {
                    yield return null;
                }

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Desactivar la pantalla de transición
        if (transitionScreen != null)
        {
            transitionScreen.SetActive(false);
        }
    }

    private void TogglePause()
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

    private void HandleGameScene(bool enable)
    {
        SetPlayerMovement(enable);
        SetCameraAudio(enable);
        SetCanvasAndOxygenBar(enable);
        SetPodAudio(enable);
        SetBioMonitor(enable); // Añadir esta línea
    }

    public void SetPlayerMovement(bool enable)
    {
        GameObject astronautIdle = GameObject.Find("AstronautIdle");
        if (astronautIdle != null)
        {
            astronautIdle.GetComponent<PlayerMovement>().enabled = enable;
        }
    }

    private void SetCameraAudio(bool enable)
    {
        Scene gameScene = SceneManager.GetSceneByName(gameSceneName);
        if (gameScene.IsValid())
        {
            foreach (GameObject obj in gameScene.GetRootGameObjects())
            {
                Camera camera = obj.GetComponentInChildren<Camera>();
                if (camera != null)
                {
                    AudioSource audioSource = camera.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.enabled = enable;
                        break;
                    }
                }
            }
        }
    }

    private void SetCanvasAndOxygenBar(bool enable)
    {
        Scene gameScene = SceneManager.GetSceneByName(gameSceneName);
        if (gameScene.IsValid())
        {
            foreach (GameObject obj in gameScene.GetRootGameObjects())
            {
                Canvas canvas = obj.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    canvas.enabled = enable;
                    Transform imaOxygenBar = canvas.transform.Find("ImaOxygenBar");
                    if (imaOxygenBar != null)
                    {
                        GameObject oxygenBar = imaOxygenBar.Find("OxygenBar")?.gameObject;
                        if (oxygenBar != null)
                        {
                            foreach (var component in oxygenBar.GetComponents<MonoBehaviour>())
                            {
                                component.enabled = enable;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    private void SetPodAudio(bool enable)
    {
        GameObject pod = GameObject.Find("Pod_001 (6)");
        Debug.Log("Pod encontrado: " + pod);
        if (pod != null)
        {
            AudioSource audioSource = pod.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log("Audio source encontrado: " + audioSource.name);
                if (!enable)
                {
                    wasPodAudioPlaying = audioSource.isPlaying;
                    if (wasPodAudioPlaying)
                    {
                        audioSource.Pause();
                        Debug.Log("Pod audio pausado.");
                    }
                }
                else if (wasPodAudioPlaying)
                {
                    audioSource.Play();
                    Debug.Log("Pod audio reanudado.");
                }
            }
            GameObject CanvasC = pod.transform.Find("CanvasC")?.gameObject;
            if (CanvasC != null)
            {
                if (!enable)
                {
                    wasCanvasCActive = CanvasC.activeSelf;
                    CanvasC.SetActive(false);
                    Debug.Log("Texto interactivo deshabilitado.");
                }
                else if (wasCanvasCActive)
                {
                    CanvasC.SetActive(true);
                    Debug.Log("Texto interactivo habilitado.");
                }
            }

            PlayAudioOnKeyPress playAudioScript = pod.GetComponent<PlayAudioOnKeyPress>();
            if (playAudioScript != null)
            {
                playAudioScript.enabled = enable;
                Debug.Log("PlayAudioOnKeyPress " + (enable ? "habilitado" : "deshabilitado") + ".");
            }
        }
        else
        {
            Debug.LogWarning("Pod no encontrado.");
        }
    }

    private void SetBioMonitor(bool enable)
    {
        GameObject bioMonitor = GameObject.Find("BioMonitor Red");
        if (bioMonitor != null)
        {
            GameObject canvasM = bioMonitor.transform.Find("CanvasM")?.gameObject;
            if (canvasM != null)
            {
                if (!enable)
                {
                    wasCanvasMActive = canvasM.activeSelf;
                    canvasM.SetActive(false);
                    Debug.Log("CanvasM deshabilitado.");
                }
                else if (wasCanvasMActive)
                {
                    canvasM.SetActive(true);
                    Debug.Log("CanvasM habilitado.");
                }
            }

            GameObject canvas = bioMonitor.transform.Find("Canvas")?.gameObject;
            if (canvas != null)
            {
                if (!enable)
                {
                    wasCanvasActive = canvas.activeSelf;
                    canvas.SetActive(false);
                    Debug.Log("Canvas deshabilitado.");
                }
                else if (wasCanvasActive)
                {
                    canvas.SetActive(true);
                    Debug.Log("Canvas habilitado.");
                }
            }
        }
        else
        {
            Debug.LogWarning("BioMonitor Red no encontrado.");
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;

        PlayerPrefs.SetInt("IsGamePaused", 1);
        PlayerPrefs.Save();

        StartCoroutine(LoadAndSetActiveScene(menuSceneName));
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        if (IsMenuSceneLoaded())
        {
            SceneManager.UnloadSceneAsync(menuSceneName).completed += (AsyncOperation op) =>
            {
                PlayerPrefs.SetInt("IsGamePaused", 0);
                PlayerPrefs.Save();
            };
        }
        else
        {
            PlayerPrefs.SetInt("IsGamePaused", 0);
            PlayerPrefs.Save();
        }
        SetActiveScene(gameSceneName);
    }

    public bool IsMenuSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == menuSceneName)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsGameSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == gameSceneName)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator LoadAndSetActiveScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

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

