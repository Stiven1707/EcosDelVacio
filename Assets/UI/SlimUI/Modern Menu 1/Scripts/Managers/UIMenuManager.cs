﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


namespace SlimUI.ModernMenu
{
    public class UIMenuManager : MonoBehaviour
    {
        private Animator CameraObject;
        private bool isPaused = false;
        // un get set para saber si el juego esta pausado
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        // campaign button sub menu
        [Header("MENUS")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;
        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;
        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;
        [Tooltip("Optional 4th Menu")]
        public GameObject extrasMenu;

        public enum Theme { custom1, custom2, custom3 };
        [Header("THEME SETTINGS")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;
        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        public GameObject PanelControls;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject PanelVideo;
        [Tooltip("The UI Panel that holds the GAME window tab")]
        public GameObject PanelGame;
        [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
        public GameObject PanelKeyBindings;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
        public GameObject PanelMovement;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
        public GameObject PanelCombat;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
        public GameObject PanelGeneral;


        // highlights in settings screen
        [Header("SETTINGS SCREEN")]
        [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;
        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;
        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;
        [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
        public GameObject lineKeyBindings;
        [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineMovement;
        [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineCombat;
        [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineGeneral;

        [Header("LOADING SCREEN")]
        [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
        public bool waitForInput = true;
        public GameObject loadingMenu;
        [Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public KeyCode userPromptKey;

        [Header("SFX")]
        [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;
        [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
        public AudioSource sliderSound;
        [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;

        void Start()
        {
            CameraObject = transform.GetComponent<Animator>();

            playMenu.SetActive(false);
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            firstMenu.SetActive(true);
            mainMenu.SetActive(true);

            SetThemeColors();
        }

        void SetThemeColors()
        {
            switch (theme)
            {
                case Theme.custom1:
                    themeController.currentColor = themeController.custom1.graphic1;
                    themeController.textColor = themeController.custom1.text1;
                    themeIndex = 0;
                    break;
                case Theme.custom2:
                    themeController.currentColor = themeController.custom2.graphic2;
                    themeController.textColor = themeController.custom2.text2;
                    themeIndex = 1;
                    break;
                case Theme.custom3:
                    themeController.currentColor = themeController.custom3.graphic3;
                    themeController.textColor = themeController.custom3.text3;
                    themeIndex = 2;
                    break;
                default:
                    Debug.Log("Invalid theme selected.");
                    break;
            }
        }

        public void PlayCampaign()
        {
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            playMenu.SetActive(true);
        }

        public void PlayCampaignMobile()
        {
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            playMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void ReturnMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            exitMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void LoadScene(string scene)
        {
            if (scene != "")
            {
                StartCoroutine(LoadAsynchronously(scene));
            }
        }

        public void LoadScenes(string scenes)
        {
            if (string.IsNullOrEmpty(scenes))
            {
                Debug.LogWarning("No se proporcionaron escenas para cargar.");
                return;
            }

            List<string> sceneList = new List<string>(scenes.Split(','));
            sceneList = sceneList.Select(scene => scene.Trim()).Where(scene => !string.IsNullOrEmpty(scene)).ToList();

            if (sceneList.Count == 0)
            {
                Debug.LogWarning("No se proporcionaron escenas válidas para cargar.");
                return;
            }

            StartCoroutine(LoadScenesAsynchronously(sceneList));
        }
        IEnumerator LoadScenesAsynchronously(List<string> sceneNames)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            mainCanvas.SetActive(false);
            loadingMenu.SetActive(true);

            // Descarga todas las escenas activas excepto la actual
            List<AsyncOperation> unloadOperations = new List<AsyncOperation>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != currentScene && scene.isLoaded)
                {
                    AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(scene);
                    unloadOperations.Add(unloadOperation);
                    Debug.Log("Unloading scene: " + scene.name);
                }
            }

            // Espera a que todas las escenas se descarguen
            foreach (var unloadOperation in unloadOperations)
            {
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }

            List<AsyncOperation> loadOperations = new List<AsyncOperation>();

            // Carga las nuevas escenas en modo aditivo
            foreach (string sceneName in sceneNames)
            {
                AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadOperation.allowSceneActivation = false;
                loadOperations.Add(loadOperation);
                Debug.Log("Loading scene: " + sceneName);
            }

            bool allScenesLoaded = false;

            while (!allScenesLoaded)
            {
                float totalProgress = loadOperations.Sum(op => op.progress) / loadOperations.Count;
                loadingBar.value = Mathf.Clamp01(totalProgress / 0.9f);
                Debug.Log("Total progress: " + totalProgress);

                allScenesLoaded = loadOperations.All(op => op.progress >= 0.9f);

                yield return null;
            }

            Debug.Log("All scenes loaded.");
            if (waitForInput)
            {
                loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                loadingBar.value = 1;

                Debug.Log("Waiting for user input...");

                while (!Input.GetKeyDown(userPromptKey))
                {
                    yield return null;
                }

                Debug.Log("User input detected, continuing...");
            }

            // Activa las nuevas escenas
            foreach (var loadOperation in loadOperations)
            {
                loadOperation.allowSceneActivation = true;
            }

            // Espera a que las escenas se activen completamente
            yield return new WaitForSeconds(1f);

            // Cambia la escena activa, verificando que esté cargada
            Scene newActiveScene = SceneManager.GetSceneByName(sceneNames[0]);
            if (newActiveScene.IsValid() && newActiveScene.isLoaded)
            {
                SceneManager.SetActiveScene(newActiveScene);
                Debug.Log("Active scene set to: " + sceneNames[0]);
            }
            else
            {
                Debug.LogError("Scene " + sceneNames[0] + " is not loaded or valid.");
            }

            // Descarga la escena anterior
            if (!string.IsNullOrEmpty(currentScene))
            {
                Debug.Log("Unloading previous scene: " + currentScene);
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
                Debug.Log("Previous scene unloaded: " + currentScene);
            }

            // Desactiva el menú de carga
            loadingMenu.SetActive(false);
            Debug.Log("Loading menu deactivated.");
        }





        public void DisablePlayCampaign()
        {
            playMenu.SetActive(false);
        }

        public void Position2()
        {
            DisablePlayCampaign();
            CameraObject.SetFloat("Animate", 1);
        }

        public void Position1()
        {
            CameraObject.SetFloat("Animate", 0);
        }

        void DisablePanels()
        {
            PanelControls.SetActive(false);
            PanelVideo.SetActive(false);
            PanelGame.SetActive(false);
            PanelKeyBindings.SetActive(false);

            lineGame.SetActive(false);
            lineControls.SetActive(false);
            lineVideo.SetActive(false);
            lineKeyBindings.SetActive(false);

            PanelMovement.SetActive(false);
            lineMovement.SetActive(false);
            PanelCombat.SetActive(false);
            lineCombat.SetActive(false);
            PanelGeneral.SetActive(false);
            lineGeneral.SetActive(false);
        }

        public void GamePanel()
        {
            DisablePanels();
            PanelGame.SetActive(true);
            lineGame.SetActive(true);
        }

        public void VideoPanel()
        {
            DisablePanels();
            PanelVideo.SetActive(true);
            lineVideo.SetActive(true);
        }

        public void ControlsPanel()
        {
            DisablePanels();
            PanelControls.SetActive(true);
            lineControls.SetActive(true);
        }

        public void KeyBindingsPanel()
        {
            DisablePanels();
            MovementPanel();
            PanelKeyBindings.SetActive(true);
            lineKeyBindings.SetActive(true);
        }

        public void MovementPanel()
        {
            DisablePanels();
            PanelKeyBindings.SetActive(true);
            PanelMovement.SetActive(true);
            lineMovement.SetActive(true);
        }

        public void CombatPanel()
        {
            DisablePanels();
            PanelKeyBindings.SetActive(true);
            PanelCombat.SetActive(true);
            lineCombat.SetActive(true);
        }

        public void GeneralPanel()
        {
            DisablePanels();
            PanelKeyBindings.SetActive(true);
            PanelGeneral.SetActive(true);
            lineGeneral.SetActive(true);
        }

        public void PlayHover()
        {
            hoverSound.Play();
        }

        public void PlaySFXHover()
        {
            sliderSound.Play();
        }

        public void PlaySwoosh()
        {
            swooshSound.Play();
        }

        // Are You Sure - Quit Panel Pop Up
        public void AreYouSure()
        {
            exitMenu.SetActive(true);
            if (extrasMenu) extrasMenu.SetActive(false);
            DisablePlayCampaign();
        }

        public void AreYouSureMobile()
        {
            exitMenu.SetActive(true);
            if (extrasMenu) extrasMenu.SetActive(false);
            mainMenu.SetActive(false);
            DisablePlayCampaign();
        }

        public void ExtrasMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(true);
            exitMenu.SetActive(false);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        
            Application.Quit();
#endif
        }

        // Load Bar synching animation
        IEnumerator LoadAsynchronously(string sceneName)
        { // scene name is just the name of the current scene being loaded
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            mainCanvas.SetActive(false);
            loadingMenu.SetActive(true);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .95f);
                loadingBar.value = progress;

                if (operation.progress >= 0.9f && waitForInput)
                {
                    loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                    loadingBar.value = 1;

                    if (Input.GetKeyDown(userPromptKey))
                    {
                        operation.allowSceneActivation = true;
                    }
                }
                else if (operation.progress >= 0.9f && !waitForInput)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}


