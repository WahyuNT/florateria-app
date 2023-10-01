using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyChild : MonoBehaviour
{
    public void destroyChild()
    {
        DestroyAllChildren();
    }
    void DestroyAllChildren()
    {
        // Loop melalui semua anak-anak GameObject dan hancurkan mereka satu per satu
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
