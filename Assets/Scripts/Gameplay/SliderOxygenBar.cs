using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importar el namespace de TextMeshPro

public class SliderOxygenBar : MonoBehaviour
{
    public float maxOxygen = 100f; // Nivel máximo de oxígeno
    public float depletionRate = 5f; // Velocidad de consumo de oxígeno por segundo
    private float currentOxygen;

    public Slider oxygenSlider;          // Referencia al Slider del oxígeno
    public TextMeshProUGUI oxygenText;   // Referencia al texto que muestra el nivel de oxígeno (ahora usando TextMeshPro)

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
        else
        {
            Debug.Log("¡El oxígeno se ha agotado!");
        }
    }

    void UpdateOxygenText()
    {
        if (oxygenText != null)
        {
            oxygenText.text = $"{Mathf.RoundToInt(currentOxygen)} %"; // Mostrar el nivel de oxígeno como porcentaje
        }
    }
}



