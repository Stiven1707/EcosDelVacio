using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu
{
    public class VolverMenu : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}

