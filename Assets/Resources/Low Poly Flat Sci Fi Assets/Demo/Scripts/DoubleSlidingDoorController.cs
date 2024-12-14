using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Door status
public enum DoubleSlidingDoorStatus {
    Closed,
    Open,
    Animating
}

[RequireComponent(typeof(AudioSource))]
public class DoubleSlidingDoorController : MonoBehaviour
{
    private DoubleSlidingDoorStatus status = DoubleSlidingDoorStatus.Closed;

    [SerializeField]
    private Transform halfDoorLeftTransform;    // Left panel of the sliding door
    [SerializeField]
    public Transform halfDoorRightTransform;   // Right panel of the sliding door

    [SerializeField]
    private float slideDistance = 0.88f;       // Sliding distance to open each panel the door

    private Vector3 leftDoorClosedPosition;
    private Vector3 leftDoorOpenPosition;

    private Vector3 rightDoorClosedPosition;
    private Vector3 rightDoorOpenPosition;

    [SerializeField]
    private float speed = 1f;                  // Speed for opening and closing the door

    private int objectsOnDoorArea = 0;

    private bool isLocked = true;            // New field to control if the door is locked

    // Sound Fx
    [SerializeField]
    private AudioClip doorOpeningSoundClip;
    [SerializeField]
    public AudioClip doorClosingSoundClip;

    private AudioSource audioSource;

    private bool isAnimationInProgress = false; // Variable to check if an animation is in progress

    void Start()
    {
        leftDoorClosedPosition = new Vector3(0f, 0f, 0f);
        leftDoorOpenPosition = new Vector3(0f, 0f, slideDistance);

        rightDoorClosedPosition = new Vector3(0f, 0f, 0f);
        rightDoorOpenPosition = new Vector3(0f, 0f, -slideDistance);

        audioSource = GetComponent<AudioSource>();
        isLocked = true;
    }

    void Update()
    {
        if (status != DoubleSlidingDoorStatus.Animating && !isLocked)
        {
            if (status == DoubleSlidingDoorStatus.Open && objectsOnDoorArea == 0)
            {
                StartCoroutine("CloseDoors");
            }
        }
        Debug.Log("Estado de la puerta (isLocked): " + isLocked);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLocked) return; // Do nothing if the door is locked

        if (status != DoubleSlidingDoorStatus.Animating)
        {
            if (status == DoubleSlidingDoorStatus.Closed)
            {
                StartCoroutine("OpenDoors");
            }
        }

        if (other.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Characters"))
        {
            objectsOnDoorArea++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isLocked) return; // Do nothing if the door is locked

        if (other.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Characters"))
        {
            objectsOnDoorArea--;
        }
    }

    IEnumerator OpenDoors()
    {
        isLocked=false;
        // Verifica primero si la puerta está bloqueada
        if (isAnimationInProgress || isLocked) 
        {
            // Opcional: puedes agregar un sonido de puerta bloqueada aquí
            yield break; // No hacer nada si la puerta está bloqueada o si ya hay una animación
        }
        isAnimationInProgress = true; // Marca que la animación está en progreso

        if (doorOpeningSoundClip != null)
        {
            audioSource.PlayOneShot(doorOpeningSoundClip, 0.7F);
        }

        status = DoubleSlidingDoorStatus.Animating;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            halfDoorLeftTransform.localPosition = Vector3.Slerp(leftDoorClosedPosition, leftDoorOpenPosition, t);
            halfDoorRightTransform.localPosition = Vector3.Slerp(rightDoorClosedPosition, rightDoorOpenPosition, t);

            yield return null;
        }

        status = DoubleSlidingDoorStatus.Open;
        isAnimationInProgress = false;
    }


IEnumerator CloseDoors()
    {
        if (isAnimationInProgress || isLocked) 
        {
            yield break; // No hacer nada si la puerta está bloqueada o si ya está en animación
        }
        isAnimationInProgress = true; // Marca que la animación está en progreso

        if (doorClosingSoundClip != null)
        {
            audioSource.PlayOneShot(doorClosingSoundClip, 0.7F);
        }

        status = DoubleSlidingDoorStatus.Animating;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            halfDoorLeftTransform.localPosition = Vector3.Slerp(leftDoorOpenPosition, leftDoorClosedPosition, t);
            halfDoorRightTransform.localPosition = Vector3.Slerp(rightDoorOpenPosition, rightDoorClosedPosition, t);

            yield return null;
        }

        status = DoubleSlidingDoorStatus.Closed;
        isAnimationInProgress = false;
    }
    public bool DoOpenDoor()
    {
        if (isLocked) return false; // Do nothing if the door is locked

        if (status != DoubleSlidingDoorStatus.Animating)
        {
            if (status == DoubleSlidingDoorStatus.Closed)
            {
                StartCoroutine("OpenDoors");
                return true;
            }
        }

        return false;
    }

    public bool DoCloseDoor()
    {
        if (isLocked) return false; // Do nothing if the door is locked

        if (status != DoubleSlidingDoorStatus.Animating)
        {
            if (status == DoubleSlidingDoorStatus.Open)
            {
                StartCoroutine("CloseDoors");
                return true;
            }
        }

        return false;
    }

    // Lock the door to prevent it from opening or closing
    public void LockDoor()
    {
        isLocked = true;
    }

    // Unlock the door to allow normal behavior
    public void UnlockDoor()
    {
        isLocked = false;
    }
}