using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu
{
    public class VolverMenu : MonoBehaviour
    {
        private string menuSceneName = "Demo1"; // Nombre de la escena del menú
        private string mainSceneName = "edwinesp"; // Nombre de la escena principal que se debe pausar

        private List<GameObject> gameObjects = new List<GameObject>();
        private Dictionary<GameObject, bool> objectActiveStates = new Dictionary<GameObject, bool>();

        [Header("LOADING SCREEN")]
        public GameObject loadingMenu;
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public KeyCode userPromptKey = KeyCode.Space;
        public bool waitForInput = true;

        private bool isMenuLoaded = false;

        void Start()
        {
            if (loadingMenu == null)
            {
                Debug.LogError("loadingMenu is not assigned in the inspector.");
            }
            if (loadingBar == null)
            {
                Debug.LogError("loadingBar is not assigned in the inspector.");
            }
            if (loadPromptText == null)
            {
                Debug.LogError("loadPromptText is not assigned in the inspector.");
            }

            if (PlayerPrefs.GetInt("MenuLoaded", 0) == 1)
            {
                Debug.Log("Menu was loaded, resuming game...");
                isMenuLoaded = true;
                ResumeGame();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isMenuLoaded)
            {
                Debug.Log("Escape key pressed, loading menu...");
                // Congelar el estado del juego
                FreezeGameState();

                // Cargar la escena del menú de manera aditiva
                SceneManager.LoadScene(menuSceneName, LoadSceneMode.Additive);
                PlayerPrefs.SetInt("MenuLoaded", 1);
                PlayerPrefs.Save();
                isMenuLoaded = true; // Marcar el menú como cargado
            }
        }

        private void FreezeGameState()
        {
            Debug.Log("Freezing game state...");
            gameObjects.Clear();
            objectActiveStates.Clear();

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.name == mainSceneName)
                {
                    gameObjects.Add(obj);
                    objectActiveStates[obj] = obj.activeSelf;
                    obj.SetActive(false);
                }
            }

            // Guardar el estado del juego
            PlayerPrefs.SetInt("GamePaused", 1);
            PlayerPrefs.Save();
            Debug.Log("Game state frozen and saved.");
        }

        private void UnfreezeGameState()
        {
            Debug.Log("Unfreezing game state...");
            foreach (GameObject obj in gameObjects)
            {
                if (objectActiveStates.ContainsKey(obj))
                {
                    obj.SetActive(objectActiveStates[obj]);
                }
            }

            // Restaurar el estado del juego
            PlayerPrefs.SetInt("GamePaused", 0);
            PlayerPrefs.Save();
            Debug.Log("Game state unfrozen and restored.");
        }

        public void ResumeGame()
        {
            if (PlayerPrefs.GetInt("MenuLoaded", 0) == 1)
            {
                Debug.Log("Resuming game...");
                // Descargar la escena del menú
                StartCoroutine(ShowLoadingScreenAndUnloadMenu());
            }
            else
            {
                Debug.Log("Menu is not loaded, cannot resume game.");
            }
        }

        public IEnumerator ShowLoadingScreenAndUnloadMenu()
        {
            Debug.Log("Showing loading screen and unloading menu...");

            if (loadingMenu != null)
            {
                Debug.Log("Activating loading menu...");
                // Mostrar la pantalla de carga
                loadingMenu.SetActive(true);
            }
            else
            {
                Debug.LogError("loadingMenu is null. Cannot show loading screen.");
            }

            // Descargar la escena del menú
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(menuSceneName);
            while (!unloadOperation.isDone)
            {
                yield return null;
            }

            // Descongelar el estado del juego
            UnfreezeGameState();

            // Esperar un momento para la transición
            yield return new WaitForSeconds(1f);

            if (loadingMenu != null)
            {
                Debug.Log("Deactivating loading menu...");
                // Ocultar la pantalla de carga
                loadingMenu.SetActive(false);
            }

            PlayerPrefs.SetInt("MenuLoaded", 0);
            PlayerPrefs.Save();
            isMenuLoaded = false; // Marcar el menú como descargado
            Debug.Log("Menu unloaded and game resumed.");
        }
    }
}






