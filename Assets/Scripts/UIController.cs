using UnityEngine;

public class UIController : MonoBehaviour
{

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform spawnPosition;

    private GameObject currentPrefab;
    private Vector3 baseScale;
    private float scaleMultiplier = 1f;

    public void SpawnPrefab()
    {
        if (prefab == null || spawnPosition == null)
        {
            Debug.LogWarning("Asignar un prefab y Spawn Position.", this);
            return;
        }
        if (currentPrefab != null)
        {
            return;
        }

        currentPrefab = Instantiate(prefab, spawnPosition.position, spawnPosition.rotation);
        baseScale = currentPrefab.transform.localScale;
        ApplyScale();
    }

    private void ApplyScale()
    {
        if (currentPrefab != null)
        {
            currentPrefab.transform.localScale = baseScale * scaleMultiplier;
        }
    }

    public void SetPrefabScale(float value)
    {
        scaleMultiplier = Mathf.Clamp(value, 0.5f, 2f);
        ApplyScale();
    }

}
