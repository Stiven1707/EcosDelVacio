using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private bool isPlayerNearby = false; // Indica si el jugador está cerca
    private bool isCanvasActive = false; // Indica si el Canvas está visible

    private void Start()
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

        // Cargar la escena con la puerta
        SceneManager.LoadScene(sceneWithDoor, LoadSceneMode.Additive);

        // Buscar la puerta y bloquearla desde el inicio
        if (puerta == null)
        {
            puerta = GameObject.Find("Door_001");

            if (puerta != null)
            {
                Debug.Log("La puerta ha sido encontrada.");
            }
            else
            {
                Debug.LogWarning("No se encontró la puerta en la escena.");
            }
        }
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
        }
    }

    private void CheckCode()
    {
        if (codeInputField == null || feedbackText == null) return;

        string enteredCode = codeInputField.text.ToUpper(); // Convertir el código ingresado a mayúsculas

        if (enteredCode == correctCode.ToUpper()) // Comparar con el código correcto
        {
            feedbackText.text = "Código correcto. ¡Acceso concedido!";
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
        }
        else
        {
            feedbackText.text = "Código incorrecto. Inténtalo de nuevo.";
            Debug.Log("Código incorrecto.");
        }
    }
}
