using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class VRSceneLoader : MonoBehaviour
{
    [SerializeField] private InputActionAsset xriInputActions;

    // Hook this method up to your Canvas Button's OnClick() event
    public void LoadGameScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // Runs after the outgoing scene finishes unloading and disabling inputs
    private void Start()
    {
        if (xriInputActions != null)
        {
            xriInputActions.Enable();
        }
    }
}
