using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 7f;
    public float rotationSpeed = 250f;
    private Animator animator;
    private float x, y;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento del jugador
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);

        // Actualizar animaciones
        if (animator != null)
        {
            animator.SetFloat("VelX", x);
            animator.SetFloat("VelY", y);
        }
    }
}









