
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Configuracion de escenas")]
    [SerializeField] private string nombreEscena;

    public void Iniciar()
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError("No se ha asignado el nombre de la escena.");
            return;
        }

        SceneManager.LoadScene(nombreEscena);
    }

    public void Salir()
    {
        Debug.Log("Cerrando juego...");

        Application.Quit();
    }
}
