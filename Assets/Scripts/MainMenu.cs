using System.Collections;
using Unity.VectorGraphics;
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

        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(0.1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nombreEscena);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void Salir()
    {
        Debug.Log("Cerrando juego...");

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
