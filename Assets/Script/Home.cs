using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Networking;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param);
        });
    }
}

public class Home : MonoBehaviour
{
    [Serializable]
    public struct Game
    {
        public string custom_name;//same name on json
        public string rfid;//same name on json
        public Sprite Icon;
        public string icon;//same name on json
    }
    public GameObject prefabToSpawn;
    public Transform parentObject;
    public string link;
    public UserProfile UserProfileScript;

    Game[] allGames;
    [SerializeField] Sprite defaultIcon;

    void Start()
    {
        //fetch data from Json
        StartCoroutine(GetGames());
    }
    public void refresh()
    {
        DestroyAllChildren();
        Spawn();
        StartCoroutine(GetGames());
    }

    void DrawUI()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        int N = allGames.Length;

        for (int i = 0; i < N; i++)
        {
            g = Instantiate(buttonTemplate, transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = allGames[i].Icon;
            g.transform.GetChild(1).GetComponent<Text>().text = allGames[i].custom_name;
            g.transform.GetChild(2).GetComponent<Text>().text = allGames[i].rfid;

            g.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }

        Destroy(buttonTemplate);//remove first def
    }

    void ItemClicked(int itemIndex)
    {
        Debug.Log("name " + allGames[itemIndex].custom_name);
    }

    //***************************************************
    IEnumerator GetGames()
    {
        string url = link + UserProfileScript.id;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.chunkedTransfer = false;
        yield return request.Send();

        if (request.isNetworkError)
        {
            //message "no internet Access"
        }
        else
        {
            if (request.isDone)
            {
                allGames = JsonHelper.GetArray<Game>(request.downloadHandler.text);
                StartCoroutine(GetGamesIcones());
            }
        }
    }

    IEnumerator GetGamesIcones()
    {
        for (int i = 0; i < allGames.Length; i++)
        {
            WWW w = new WWW(allGames[i].icon);
            yield return w;

            if (w.error != null)
            {
                //error
                //show default image
                allGames[i].Icon = defaultIcon;
            }
            else
            {
                if (w.isDone)
                {
                    Texture2D tx = w.texture;
                    allGames[i].Icon = Sprite.Create(tx, new Rect(0f, 0f, tx.width, tx.height), Vector2.zero, 10f);
                }
            }
        }

        DrawUI();
    }
    void DestroyAllChildren()
    {
        // Loop melalui semua anak-anak GameObject dan hancurkan mereka satu per satu
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    void Spawn()
    {
        // Pastikan prefabToSpawn dan spawnPoint tidak null
        if (prefabToSpawn != null)
        {
            // Membuat instance baru dari prefab
            GameObject spawnedObject = Instantiate(prefabToSpawn);

            // Mengatur parentObject sebagai parent dari prefab yang di-spawn
            if (parentObject != null)
            {
                spawnedObject.transform.parent = parentObject;
            }

            // Opsional: Anda dapat melakukan operasi lain pada spawnedObject di sini jika perlu
        }
        else
        {
            Debug.LogWarning("Prefab atau spawnPoint tidak diatur. Silakan tentukan prefab dan spawnPoint di Inspector Unity.");
        }
    }

}
