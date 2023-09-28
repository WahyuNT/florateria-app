using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegisterBtn : MonoBehaviour
{
    public InputField Username;
    public InputField Password;
    public string apiURL;
    public string method;
    public GameObject WrongText;
    public GameObject[] Panel;
    public string[] PanelBoolean;
    public Text ErorText;

    

    public void PostRegister()
    {
        StartCoroutine(PostRequest());
    }

    IEnumerator PostRequest()
    {
        string username = Username.text;
        string password = Password.text;

        // Membuat objek JSON untuk dikirim ke server
        string jsonRequestBody = "{\"name\":\"" + username + "\",\"password\":\"" + password + "\"}";
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
            WrongText.SetActive(true);
            ErorText.text = responseText;
            Debug.LogError("Response: " + responseText);
        }
        else
        {
            // Menampilkan respons di console log
            string responseText = request.downloadHandler.text;
            WrongText.SetActive(false);

            Debug.Log("Response: " + responseText);
            success();
        }
    }
    void success()
    {

        for (int i = 0; i < Panel.Length; i++)
        {
            if (PanelBoolean[i] == "true")
            {
                Panel[i].SetActive(true);
            }
            else
            {
                Panel[i].SetActive(false);
            }
        }
    }
}
