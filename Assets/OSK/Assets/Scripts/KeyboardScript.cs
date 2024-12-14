using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardScript : MonoBehaviour
{
    public InputField TextField;
    public GameObject RusLayoutSml, RusLayoutBig, EngLayoutSml, EngLayoutBig, SymbLayout;
    public Button closeButton; // Botón de cerrar

    // Método para cerrar el teclado (al presionar "X")
    public void CloseKeyboard()
    {
        gameObject.SetActive(false); // Desactiva el teclado
    }

    // Método para abrir el teclado
    public void OpenKeyboard()
    {
        gameObject.SetActive(true); 
    }

    public void alphabetFunction(string alphabet)
    {
        TextField.text = TextField.text + alphabet;
    }

    public void BackSpace()
    {
        if (TextField.text.Length > 0)
            TextField.text = TextField.text.Remove(TextField.text.Length - 1);
    }

    public void CloseAllLayouts()
    {
        RusLayoutSml.SetActive(false);
        RusLayoutBig.SetActive(false);
        EngLayoutSml.SetActive(false);
        EngLayoutBig.SetActive(false);
        SymbLayout.SetActive(false);
    }

    public void ShowLayout(GameObject SetLayout)
    {
        CloseAllLayouts();
        SetLayout.SetActive(true);
    }

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseKeyboard);
        }

        // Asocia el campo de texto para abrir el teclado cuando se selecciona
        EventTrigger eventTrigger = TextField.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((eventData) => OpenKeyboard());

        eventTrigger.triggers.Add(entry);
    }
}
