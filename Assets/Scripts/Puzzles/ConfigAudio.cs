using UnityEngine;

public class configAudio : MonoBehaviour
{
    public AudioClip audioClip; // Campo para asignar el clip de audio
    private AudioSource audioSource; // Componente para reproducir el audio

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
    }

    private void Update()
    {
        // Verificar si se presiona la tecla X y reproducir el audio
        if (Input.GetKeyDown(KeyCode.X) && audioClip != null)
        {
            audioSource.Play();
        }
    }
}
