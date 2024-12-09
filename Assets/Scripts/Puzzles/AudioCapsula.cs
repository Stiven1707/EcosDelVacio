using UnityEngine;

public class PlayAudioOnKeyPress : MonoBehaviour
{
    public AudioClip audioClip; 
    public GameObject interactionMessageCanvas; 
    private AudioSource audioSource; 
    private bool isPlayerNearby = false; 

    private void Start()
    {
        // Agregar un componente AudioSource si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar el AudioSource con el clip asignado
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        else
        {
            Debug.LogWarning("No se ha asignado un audio clip en el inspector.");
        }

        // Asegurarse de que el mensaje de interacción está oculto al inicio
        if (interactionMessageCanvas != null)
        {
            interactionMessageCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se ha asignado un Canvas para el mensaje de interacción.");
        }
    }

    private void Update()
    {
        // Verificar si el jugador está cerca y presiona la tecla X para reproducir el audio
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.X) && audioClip != null)
        {
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tiene la etiqueta "Player"
        {
            isPlayerNearby = true;

            // Mostrar el mensaje de interacción
            if (interactionMessageCanvas != null)
            {
                interactionMessageCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Ocultar el mensaje de interacción
            if (interactionMessageCanvas != null)
            {
                interactionMessageCanvas.SetActive(false);
            }
        }
    }
}
