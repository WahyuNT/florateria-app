using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerNFC : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform parentObject;
    // Start is called before the first frame update


    public void StartScanning()
    {
        if (prefabToSpawn != null)
        {
            GameObject spawnedObject = Instantiate(prefabToSpawn);

            if (parentObject != null)
            {
                // Mengatur parentObject sebagai parent dari prefab yang di-spawn
                spawnedObject.transform.SetParent(parentObject, false);

                // Mengatur anchoredPosition dari RectTransform untuk membuat panel berada di tengah parentObject
                RectTransform rectTransform = spawnedObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = Vector2.zero; // Mengatur ke (0, 0) akan menempatkan panel di tengah parentObject
                }
                else
                {
                    Debug.LogWarning("Prefab harus memiliki komponen RectTransform untuk diatur di tengah.");
                }
            }
            else
            {
                Debug.LogWarning("parentObject tidak diatur. Silakan tentukan parentObject di Inspector Unity.");
            }
        }
        else
        {
            Debug.LogWarning("Prefab tidak diatur. Silakan tentukan prefab di Inspector Unity.");
        }
    }

    public void stopScanning()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
