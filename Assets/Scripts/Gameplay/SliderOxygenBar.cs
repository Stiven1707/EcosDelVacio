using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public bool IsDead
    {
        get { return isDead; }
    }

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

        // Verificar si la animación "Die" ha terminado
        if (isDead && playerAnimator != null)
        {
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0); // Estado actual en la capa 0
            if (stateInfo.IsName("Die") && stateInfo.normalizedTime >= 1.0f)
            {
                OnDeathAnimationComplete(); // Llamar al método una vez que la animación termina
            }
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

    void OnDeathAnimationComplete()
    {
        Debug.Log("La animación de muerte ha finalizado. Configurando estado final...");

        // Desactivar el Animator para que el personaje quede quieto
        playerAnimator.enabled = false;

        // Mantener la posición actual del personaje (si la animación lo deja acostado)
        // Si quieres ajustar manualmente la posición o rotación, hazlo aquí
    }
}
