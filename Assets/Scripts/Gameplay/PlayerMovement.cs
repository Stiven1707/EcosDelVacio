using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 7;
    public float rotationSpeed = 250;
    private Animator animator; // Agregar referencia al Animator

    private float x, y;

void Start()
{
    animator = GetComponent<Animator>();
}

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);

        animator.SetFloat("VelX",x);
        animator.SetFloat("VelY",y);

    }
}
