using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public class TerminalInteraction : MonoBehaviour
{
    public GameObject terminalCanvas; // Canvas que contiene la UI del terminal
    public InputField codeInputField; // Campo de entrada para el código
    public Text feedbackText; // Texto de retroalimentación (Correcto/Incorrecto)
    public Button acceptButton; // Botón "Aceptar" para verificar el código
    public string correctCode = "NEXXUS"; // Código correcto
    private GameObject puerta; // Objeto de la puerta
    public GameObject interactionMessage; // UI para mostrar "Presione E para interactuar"
    public string sceneWithDoor = "Sample Scene"; // Nombre de la escena con la puerta
    public GameObject bioMonitorRed; // Referencia al objeto BioMonitor Red
    public GameObject bioMonitorBlue; // Referencia al objeto BioMonitor Blue

    private string gameSceneName = "edwinespj";

    private bool isPlayerNearby = false; // Indica si el jugador está cerca
    private bool isCanvasActive = false; // Indica si el Canvas está visible

    private GameManager gameManager; // Referencia al GameManager
    private SliderOxygenBar sliderOxygenBar; // Referencia al SliderOxygenBar

    public bool IsCanvasActive
    {
        get { return isCanvasActive; }
    }

    private IEnumerator Start()
    {
        // Ocultar el Canvas y el mensaje de interacción al inicio
        if (terminalCanvas != null)
        {
            terminalCanvas.SetActive(false);
        }

        if (interactionMessage != null)
        {
            interactionMessage.SetActive(false);
        }

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        // Configurar el botón "Aceptar"
        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(CheckCode);
        }
        else
        {
            Debug.LogError("El botón 'Aceptar' no está asignado en el Inspector.");
        }

        // Esperar a que la escena esté completamente cargada
        while (!IsSceneLoaded(sceneWithDoor))
        {
            yield return null;
        }
        // Encontrar y bloquear la puerta
        FindAndLockDoor();

        // Encontrar el SliderOxygenBar
        sliderOxygenBar = FindObjectOfType<SliderOxygenBar>();
       
    }

    private void Update()
    {
        // Mostrar/ocultar el Canvas al presionar E si el jugador está cerca y no está escribiendo en el InputField
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !codeInputField.isFocused)
        {
            ToggleCanvas();
        }

        // Verificar si se presiona Enter cuando el InputField está activo
        if (isCanvasActive && Input.GetKeyDown(KeyCode.Return)) // Return equivale a Enter
        {
            CheckCode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tiene la etiqueta "Player"
        {
            isPlayerNearby = true;

            // Mostrar el mensaje de interacción
            if (interactionMessage != null)
            {
                interactionMessage.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Ocultar el Canvas y el mensaje de interacción
            if (interactionMessage != null)
            {
                interactionMessage.SetActive(false);
            }

            if (terminalCanvas != null && isCanvasActive)
            {
                terminalCanvas.SetActive(false);
                isCanvasActive = false;
            }
        }
    }

    private void ToggleCanvas()
    {
        if (terminalCanvas != null)
        {
            isCanvasActive = !isCanvasActive; // Cambiar el estado del Canvas
            terminalCanvas.SetActive(isCanvasActive);

            // Limpiar el campo de texto y mensajes al abrir el Canvas
            if (isCanvasActive && codeInputField != null)
            {
                codeInputField.text = "";
                codeInputField.ActivateInputField();
            }

            if (feedbackText != null)
            {
                feedbackText.text = "";
            }

            // Ocultar el mensaje de interacción cuando se abra el Canvas
            if (interactionMessage != null && isCanvasActive)
            {
                interactionMessage.SetActive(false);
            }

            // Deshabilitar la movilidad del jugador
            if (gameManager != null)
            {
                gameManager.SetPlayerMovement(false);
            }
        }
    }

    private void CheckCode()
    {
        if (codeInputField == null || feedbackText == null) return;

        string enteredCode = codeInputField.text.ToUpper(); // Convertir el código ingresado a mayúsculas

        if (enteredCode == correctCode.ToUpper()) // Comparar con el código correcto
        {
            codeInputField.text = "";
            codeInputField.ActivateInputField();
            feedbackText.text = "Código correcto. ¡Acceso concedido!";
            feedbackText.color = Color.green; // Cambiar el color del texto a verde
            feedbackText.gameObject.SetActive(true); // Asegurarse de que el texto de retroalimentación esté activo
            Debug.Log("Código correcto.");

            // Desbloquear la puerta
            if (puerta != null)
            {
                DoubleSlidingDoorController puertaScript = puerta.GetComponent<DoubleSlidingDoorController>();
                if (puertaScript != null)
                {
                    puertaScript.UnlockDoor(); // Desbloquear la puerta
                    Debug.Log("La puerta ha sido desbloqueada.");
                }
                else
                {
                    Debug.LogWarning("No se encontró el script 'DoubleSlidingDoorController' en el objeto puerta.");
                }
            }
            // Encontrar el SliderOxygenBar
            sliderOxygenBar = FindObjectOfType<SliderOxygenBar>();
            // Restaurar el oxígeno al máximo
            if (sliderOxygenBar != null)
            {
                sliderOxygenBar.RestoreOxygenToMax();
            }

            // Desactivar la interacción con la terminal
            isPlayerNearby = false;
            if (interactionMessage != null)
            {
                interactionMessage.SetActive(false);
            }
            if (terminalCanvas != null)
            {
                terminalCanvas.SetActive(false);
                isCanvasActive = false;
            }

            // Desactivar BioMonitor Red y activar BioMonitor Blue
            if (bioMonitorRed != null)
            {
                bioMonitorRed.SetActive(false);
            }
            if (bioMonitorBlue != null)
            {
                bioMonitorBlue.SetActive(true);
            }
        }
        else
        {
            codeInputField.text = "";
            codeInputField.ActivateInputField();
            feedbackText.text = "Código incorrecto. Inténtalo de nuevo.";
            feedbackText.color = Color.red; // Cambiar el color del texto a rojo
            feedbackText.gameObject.SetActive(true); // Asegurarse de que el texto de retroalimentación esté activo
            Debug.Log("Código incorrecto.");
            StartCoroutine(ResetFeedbackTextColor());
        }
    }

    private IEnumerator ResetFeedbackTextColor()
    {
        yield return new WaitForSeconds(1f); // Esperar 1 segundo
        feedbackText.color = Color.white; // Restaurar el color original del texto
        feedbackText.gameObject.SetActive(false); // Ocultar el texto de retroalimentación
    }



    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private void FindAndLockDoor()
    {
        if (puerta == null)
        {
            puerta = GameObject.Find("Door_001");

            if (puerta != null)
            {
                Debug.Log("La puerta ha sido encontrada.");
                DoubleSlidingDoorController puertaScript = puerta.GetComponent<DoubleSlidingDoorController>();
                if (puertaScript != null)
                {
                    puertaScript.LockDoor(); // Bloquear la puerta desde el inicio
                    Debug.Log("La puerta ha sido bloqueada.");
                }
                else
                {
                    Debug.LogWarning("No se encontró el script 'DoubleSlidingDoorController' en el objeto puerta.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró la puerta en la escena.");
            }
        }
    }
}