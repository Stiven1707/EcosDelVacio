using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class terminal : MonoBehaviour
{
    [Header("Configuraciones del Puzzle")]
    public string correctCode = "NEXUSS"; 
    public float resetFeedbackDelay = 2f;
    public int intentosMaximos = 3; // 

    [Header("Referencias de UI")]
    public InputField inputField; // Campo de texto para ingresar el código
    public Text feedbackText; // Texto para mostrar retroalimentación al jugador
    public GameObject rewardPanel; // Panel que contiene las recompensas

    [Header("Efectos Visuales y Sonoros")]
    public AudioSource successAudio; // Sonido de éxito
    public AudioSource errorAudio; // Sonido de error
    public Light terminalLight; // Luz de la terminal (indicador de estado)

    [Header("Interacción del Jugador")]
    public GameObject player; // Jugador para detectar proximidad
    public float interactionDistance = 3f; // Distancia para permitir interacción
    public GameObject interactionPrompt; // Indicador de interacción

    // Estado interno
    private int attempts = 0; // Contador de intentos
    private bool isPlayerNear = false; // Bandera para saber si el jugador está cerca

    void Start()
    {
        // Configuración inicial
        feedbackText.text = "";
        rewardPanel.SetActive(false);
        interactionPrompt.SetActive(false);
        terminalLight.color = Color.yellow; // Luz amarilla inicial para indicar "esperando"
    }

    void Update()
    {
        HandlePlayerProximity();
    }

    // Maneja la proximidad del jugador para activar la interacción
    void HandlePlayerProximity()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= interactionDistance)
        {
            if (!isPlayerNear)
            {
                isPlayerNear = true;
                interactionPrompt.SetActive(true); // Mostrar indicador de interacción
            }

            if (Input.GetKeyDown(KeyCode.E)) // Tecla E para interactuar
            {
                ActivateTerminal();
            }
        }
        else if (isPlayerNear)
        {
            isPlayerNear = false;
            interactionPrompt.SetActive(false); // Ocultar indicador
        }
    }

    // Activa la interfaz de la terminal
    void ActivateTerminal()
    {
        inputField.gameObject.SetActive(true); // Mostrar campo de entrada
        feedbackText.text = "Introduce el código y presiona Enter.";
        inputField.ActivateInputField(); // Hacer foco en el campo
    }

    // Validar código ingresado
    public void CheckCode()
    {
        string enteredCode = inputField.text.ToUpper().Trim(); // Normalizar entrada
        attempts++;

        if (enteredCode == correctCode)
        {
            UnlockRewards();
        }
        else
        {
            if (attempts >= intentosMaximos)
            {
                LockTerminal();
            }
            else
            {
                IncorrectCode();
            }
        }

        inputField.text = ""; // Limpiar el campo de entrada
    }

    // Manejar código correcto
    void UnlockRewards()
    {
        feedbackText.text = "Código correcto. Recompensas desbloqueadas.";
        feedbackText.color = Color.green;

        rewardPanel.SetActive(true); // Mostrar recompensas
        terminalLight.color = Color.green; // Cambiar luz a verde
        successAudio.Play(); // Reproducir audio de éxito

        DisableInteraction();
    }

    // Manejar código incorrecto
    void IncorrectCode()
    {
        feedbackText.text = $"Código incorrecto. Intentos restantes: {intentosMaximos - attempts}.";
        feedbackText.color = Color.red;

        errorAudio.Play(); // Reproducir sonido de error
        StartCoroutine(ResetFeedback());
    }

    // Bloquear la terminal después de demasiados intentos
    void LockTerminal()
    {
        feedbackText.text = "Terminal bloqueada. Demasiados intentos fallidos.";
        feedbackText.color = Color.red;

        terminalLight.color = Color.red; // Cambiar luz a rojo
        errorAudio.Play(); // Sonido de error crítico

        DisableInteraction();
    }

    // Resetear el feedback visual después de un tiempo
    IEnumerator ResetFeedback()
    {
        yield return new WaitForSeconds(resetFeedbackDelay);
        feedbackText.text = "Introduce el código y presiona Enter.";
        feedbackText.color = Color.white;
    }

    // Desactivar interacción después de desbloquear o bloquear
    void DisableInteraction()
    {
        inputField.interactable = false;
        interactionPrompt.SetActive(false);
        isPlayerNear = false;
    }
}
