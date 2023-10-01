using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Redeem : MonoBehaviour
{
    public NFC nfc;
    public string ImageLink;
    public string apiURL;
    public string method;

    [System.Serializable]
    public class User
    {
        public string icon;

    }

    public void post()
    {
        StartCoroutine(PostRequest());
    }

    IEnumerator PostRequest()
    {
        string rfid = nfc.tag_output_text.text;



        // Membuat objek JSON untuk dikirim ke server
        string jsonRequestBody = "{\"rfid\":\"" + rfid + "\"}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        // Membuat objek UnityWebRequest untuk POST request
        UnityWebRequest request = new UnityWebRequest(apiURL, method);

        // Menetapkan header
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Mengirim request
        yield return request.SendWebRequest();

        // Memeriksa apakah ada error
        if (request.isNetworkError || request.isHttpError)
        {
            string responseText = request.downloadHandler.text;
            Debug.LogError("Response: " + responseText);
        }
        else
        {
            string responseText = request.downloadHandler.text; // JSON string

            // Menguraikan JSON menjadi objek User
            User user = JsonUtility.FromJson<User>(responseText);

            // Sekarang Anda dapat mengakses nama (name) dari objek User
            // string ResponseIcon = user.icon;

            ImageLink = user.icon;
            Debug.Log("Response: " + responseText);
        }
    }
}
