using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public GameObject MensajeTerminal;  
    public GameObject codeInputUI;         
    public InputField codigo;              
    public Button aceptarButton;           
    public string correctCode = "NEXXUS";  
    public Text feedbackMessage;           
    private bool isPlayerNearby = false;   
    private GameObject player;             
    private bool isCodeInputActive = false;

    private void Start()
    {
        player = GameObject.Find("AstronautIdle");
        if (player == null)
        {
            Debug.LogError("Player with name 'AstronautIdle' not found in the scene.");
        }

        if (MensajeTerminal != null)
        {
            MensajeTerminal.SetActive(false);
        }
        else
        {
            Debug.LogError("Interaction Message UI not assigned.");
        }

        if (codeInputUI != null)
        {
            codeInputUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Code Input UI not assigned.");
        }

        if (aceptarButton != null)
        {
            aceptarButton.onClick.AddListener(() => SubmitCode(codigo.text));
        }
        else
        {
            Debug.LogError("Submit Button (Aceptar) not assigned.");
        }

        if (feedbackMessage != null)
        {
            feedbackMessage.text = "";
            feedbackMessage.gameObject.SetActive(false); // Asegurarse de que inicie desactivado
        }
        else
        {
            Debug.LogError("Feedback Message UI not assigned.");
        }
    }

    private void Update()
    {
        // Mostrar UI de entrada si el jugador presiona E y la UI no está activa
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isCodeInputActive)
        {
            ShowCodeInputUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNearby = true;
            if (MensajeTerminal != null)
            {
                MensajeTerminal.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNearby = false;
            if (MensajeTerminal != null)
            {
                MensajeTerminal.SetActive(false);
            }

            if (codeInputUI != null)
            {
                codeInputUI.SetActive(false);
                isCodeInputActive = false;
            }

            if (feedbackMessage != null)
            {
                feedbackMessage.gameObject.SetActive(false); // Ocultar retroalimentación
            }
        }
    }

    private void ShowCodeInputUI()
    {
        if (codeInputUI != null)
        {
            codeInputUI.SetActive(true); 
            if (codigo != null)
            {
                codigo.text = ""; 
                codigo.ActivateInputField(); 
            }

            if (feedbackMessage != null)
                feedbackMessage.gameObject.SetActive(false); // Ocultar retroalimentación previa

            isCodeInputActive = true; 
        }
    }

    public void SubmitCode(string enteredCode)
    {
        if (feedbackMessage != null)
        {
            feedbackMessage.gameObject.SetActive(true); // Mostrar mensaje de retroalimentación

            if (enteredCode == correctCode)
            {
                feedbackMessage.text = "Código correcto. Recompensa desbloqueada.";
                Debug.Log("Recompensa desbloqueada.");
            }
            else
            {
                feedbackMessage.text = "Código incorrecto. Inténtalo de nuevo.";
                Debug.Log("Código incorrecto.");
            }
        }

        if (codeInputUI != null)
        {
            codeInputUI.SetActive(false);
            isCodeInputActive = false; 
        }
    }
}
