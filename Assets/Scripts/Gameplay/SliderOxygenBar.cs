using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importar el namespace de TextMeshPro

public class SliderOxygenBar : MonoBehaviour
{
    [Header("Configuración del Oxígeno")]
    public float maxOxygen = 100f; // Nivel máximo de oxígeno
    public float depletionRate = 0.5f; // Velocidad de consumo de oxígeno por segundo
    private float currentOxygen;

    [Header("Referencias UI")]
    public Slider oxygenSlider;          // Referencia al Slider del oxígeno
    public TextMeshProUGUI oxygenText;   // Referencia al texto que muestra el nivel de oxígeno (TextMeshPro)

    [Header("Animator del Jugador")]
    public Animator playerAnimator;      // Referencia al Animator del jugador

    private bool isDead = false;         // Para asegurarnos de que la animación se active una sola vez

    void Start()
    {
        currentOxygen = maxOxygen;

        // Configurar el Slider al inicio
        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen; // Inicializar el Slider con el valor máximo
        }

        // Actualizar el texto al inicio
        UpdateOxygenText();
    }

    void Update()
    {
        // Reducir el oxígeno con el tiempo
        if (currentOxygen > 0)
        {
            currentOxygen -= depletionRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

            // Actualizar el Slider con el nivel actual de oxígeno
            if (oxygenSlider != null)
            {
                oxygenSlider.value = currentOxygen;
            }

            // Actualizar el texto con el nivel de oxígeno
            UpdateOxygenText();
        }

        // Si el oxígeno llega a 0, activar la animación de morir
        if (currentOxygen <= 0 && !isDead)
        {
            TriggerDeathAnimation();
        }
    }

    void UpdateOxygenText()
    {
        if (oxygenText != null)
        {
            oxygenText.text = $"{Mathf.RoundToInt(currentOxygen)} %"; // Mostrar el nivel de oxígeno como porcentaje
        }
    }

    void TriggerDeathAnimation()
    {
        isDead = true; // Evitar que se ejecute más de una vez
        Debug.Log("¡Oxígeno agotado! Activando animación de morir...");

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die"); // Activar el trigger 'Die' en el Animator
        }
        else
        {
            Debug.LogError("No se ha asignado el Animator en el script.");
        }
    }
}




