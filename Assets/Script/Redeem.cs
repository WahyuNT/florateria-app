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
    public GameObject RedeemBox;
    public string method;
    public string TestRFID;
    public string RFID;
    public InputField CustomName;
    public UserProfile UserProfileVar;
    public Text consoleText;
    public Text rfidtext;

    [System.Serializable]
    public class User
    {
        public string icon;

    }

    public void find()
    {
        StartCoroutine(PostFind());
    }
    public void redeem()
    {
        StartCoroutine(PostRedeem());
    }


    IEnumerator PostFind()
    {
        // string rfid = TestRFID;
        // string rfid = nfc.rfid;


        // Membuat objek JSON untuk dikirim ke server
        string jsonRequestBody = "{\"rfid\":\"" + RFID + "\"}";
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
            Debug.LogError("Response: " + responseText + "body : " + jsonRequestBody);
            // rfidtext.text = nfc.rfid;
            consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
        }
        else
        {
            string responseText = request.downloadHandler.text; // JSON string

            // Menguraikan JSON menjadi objek User
            User user = JsonUtility.FromJson<User>(responseText);

            // Sekarang Anda dapat mengakses nama (name) dari objek User
            // string ResponseIcon = user.icon;

            ImageLink = user.icon;
            // rfidtext.text = nfc.rfid;
            Debug.Log("Response: " + responseText);
            consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
        }
    }

    IEnumerator PostRedeem()
    {
        int id_player = UserProfileVar.id;
        string custom_name = CustomName.text;
        // string rfid = TestRFID;
        // string rfid = nfc.rfid;

        // Membuat objek JSON untuk dikirim ke server
        string jsonRequestBody = "{\"rfid\":\"" + RFID + "\",\"custom_name\":\"" + custom_name + "\",\"id_player\":\"" + id_player + "\"}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        Debug.Log(jsonRequestBody);

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
            consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
        }
        else
        {
            // Menampilkan respons di console log
            string responseText = request.downloadHandler.text;
            nfc.reset();
            consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
            Debug.Log("Response: " + responseText);
        }
    }
}
