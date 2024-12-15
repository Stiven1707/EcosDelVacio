using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
  
    public Transform posicion1,posicion3;
    private bool vista;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            vista = true;
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            vista = false;
        }

        if(vista == true)
        {
            transform.position = Vector3.Lerp(transform.position, posicion1.position, 5 * Time.deltaTime);
        }
        else
        {
             transform.position = Vector3.Lerp(transform.position, posicion3.position, 5 * Time.deltaTime);
        }
    }
}
