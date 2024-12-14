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

        if (IsMenuSceneLoaded() && IsEdwinespjSceneLoaded())
        {
            HandleEdwinespjScene(false);
        }
        else
        {
            HandleEdwinespjScene(true);
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

    private void HandleEdwinespjScene(bool enable)
    {
        SetPlayerMovement(enable);
        SetCameraAudio(enable);
        SetCanvasAndOxygenBar(enable);
        SetPodAudio(enable);
    }

    private void SetPlayerMovement(bool enable)
    {
        GameObject astronautIdle = GameObject.Find("AstronautIdle");
        if (astronautIdle != null)
        {
            astronautIdle.GetComponent<PlayerMovement>().enabled = enable;
        }
    }

    private void SetCameraAudio(bool enable)
    {
        Scene edwinespjScene = SceneManager.GetSceneByName("edwinespj");
        if (edwinespjScene.IsValid())
        {
            foreach (GameObject obj in edwinespjScene.GetRootGameObjects())
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
        Scene edwinespjScene = SceneManager.GetSceneByName("edwinespj");
        if (edwinespjScene.IsValid())
        {
            foreach (GameObject obj in edwinespjScene.GetRootGameObjects())
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
                CanvasC.SetActive(enable);
                Debug.Log("Texto interactivo " + (enable ? "habilitado" : "deshabilitado") + ".");
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

    public void PauseGame()
    {
        isGamePaused = true;

        PlayerPrefs.SetInt("IsGamePaused", 1);
        PlayerPrefs.Save();

        StartCoroutine(LoadAndSetActiveScene("Demo1"));
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        if (IsMenuSceneLoaded())
        {
            SceneManager.UnloadSceneAsync("Demo1").completed += (AsyncOperation op) =>
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



