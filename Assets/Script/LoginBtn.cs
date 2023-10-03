using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginBtn : MonoBehaviour
{
    public InputField Username;
    public InputField Password;
    public string apiURL;
    public string method;
    public GameObject WrongText;
    public Text ErorText;
    public GameObject[] Panel;
    public string[] PanelBoolean;
    public UserProfile UserProfileScript;
    [System.Serializable]
    public class User
    {
        public int id;
        public int id_role;
        public string name;
        public string password;
        public string created_at;
        public string updated_at;
    }
    public void PostLogin()
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
            string responseText = request.downloadHandler.text; // JSON string

            // Menguraikan JSON menjadi objek User
            User user = JsonUtility.FromJson<User>(responseText);

            // Sekarang Anda dapat mengakses nama (name) dari objek User
            int ResponseId = user.id;
            string ResponseName = user.name;

            WrongText.SetActive(false);
            UserProfileScript.id = ResponseId; // Mengatur nilai id
            UserProfileScript.name = ResponseName; // Mengatur nilai name
            Debug.Log("Response: " + responseText);
            success();
        }
    }
    void success()
    {
        Username.text = null;
        Password.text = null;

        
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
