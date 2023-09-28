using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public List<Sprite> imageList; // List yang berisi gambar-gambar
    void Start()
    {
        // Iterasi melalui List gambar menggunakan foreach
        foreach (Sprite image in imageList)
        {
            // Lakukan operasi pada setiap gambar di sini
            // Contoh: Ganti gambar menjadi gambar lain
            GetComponent<SpriteRenderer>().sprite = image;
        }
    }

}
