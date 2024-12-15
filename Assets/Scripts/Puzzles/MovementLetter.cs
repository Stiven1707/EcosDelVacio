using UnityEngine;

public class MovementLetter : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float floatAmplitude = 0.5f; 
    public float floatSpeed = 2f; 
    public float rotationSpeed = 50f;

    [Header("Efecto de Escala")]
    public bool enableScaling = true; 
    public float scaleAmplitude = 0.1f; 
    public float scaleSpeed = 3f; 

    private Vector3 initialPosition; 
    private Vector3 initialScale; 

    void Start()
    {
        // Guardar la posición y escala inicial
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Movimiento flotante
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(initialPosition.x, initialPosition.y + offsetY, initialPosition.z);

        // Rotación constante
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Efecto de escalado (si está habilitado)
        if (enableScaling)
        {
            float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
            transform.localScale = initialScale + new Vector3(scaleOffset, scaleOffset, scaleOffset);
        }
    }
}
