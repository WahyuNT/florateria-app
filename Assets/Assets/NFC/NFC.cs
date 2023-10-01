using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NFC : MonoBehaviour
{

    public string tagID;
    public int code = 0;
    // public string rfid;

    public Text tag_output_text;
    public bool tagFound = false;
    public bool scanned = false;
    public GameObject RedeemBox;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;

    public string ImageLink;
    public string apiURL;
    public string method;
    public string TestRFID;
    public InputField CustomName;
    public UserProfile UserProfileVar;
    public Text consoleText;
    public Text rfidtext;

    [System.Serializable]
    public class User
    {
        public string icon;

    }

    void Start()
    {
        tag_output_text.text = "Scan a NFC tag to make the cube disappear...";

    }
    public void reset()
    {
        tagFound = false;
        RedeemBox.SetActive(false);
        // rfid = null;
        tagID = null;
        CustomName.text = null;
    }
    public void scan()
    {

        try
        {
            // Create new NFC Android object
            mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
            mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
            sAction = mIntent.Call<String>("getAction"); // resulte are returned in the Intent object
            if (sAction == "android.nfc.action.NDEF_DISCOVERED")
            {
                Debug.Log("Tag of type NDEF");
                AndroidJavaObject[] rawMsg = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
                AndroidJavaObject[] records = rawMsg[0].Call<AndroidJavaObject[]>("getRecords");
                // byte[] payLoad = records[0].Call<byte[]>("getPayload");
                // string result = System.Text.Encoding.Default.GetString(payLoad); // not sure if it works for all encodings, but it works for me
                byte[] payLoad = records[0].Call<byte[]>("getPayload");
                string result = BitConverter.ToString(payLoad).Replace("-", "");

                tag_output_text.text = result;
                // rfid = result;

                // rfid = System.Text.Encoding.Default.GetString(payLoad);
                if (!string.IsNullOrEmpty(result))
                {
                    RedeemBox.SetActive(true);
                    StartCoroutine(PostFind(result));
                }
            }
            else if (sAction == "android.nfc.action.TECH_DISCOVERED")
            {
                Debug.Log("TAG DISCOVERED");
                // Get ID of tag
                AndroidJavaObject mNdefMessage = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
                if (mNdefMessage != null)
                {
                    byte[] payLoad = mNdefMessage.Call<byte[]>("getId");
                    string text = System.Convert.ToBase64String(payLoad);
                    tag_output_text.text += "This is your tag text: " + text;
                    Destroy(GetComponent("MeshRenderer")); //Destroy Box when NFC ID is displayed
                    tagID = text;
                }
                else
                {
                    tag_output_text.text = "No ID found !";
                }
                tagFound = true;
                // How to read multiple tags maybe with this line mIntent.Call("removeExtra", "android.nfc.extra.TAG");
                return;
            }
            else if (sAction == "android.nfc.action.TAG_DISCOVERED")
            {
                Debug.Log("This type of tag is not supported !");
            }
            else
            {
                tag_output_text.text = "Scan a NFC tag to make the cube disappear...";
                return;
            }
        }
        catch (Exception ex)
        {
            string text = ex.Message;
            tag_output_text.text = text;
        }
    }
    public void redeem()
    {
        // StartCoroutine(PostRedeem());
    }

    void Update()
    {
        // if (!string.IsNullOrEmpty(rfid))

        // {
        //     Debug.Log(rfid);
        //     StartCoroutine(PostFind());
        //     RedeemBox.SetActive(true);

        // }




        if (scanned == false)
        {
            // if (Application.platform == RuntimePlatform.Android)
            // {
            //     if (!tagFound)
            //     {
            //         try
            //         {
            //             // Create new NFC Android object
            //             mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
            //             mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
            //             sAction = mIntent.Call<String>("getAction"); // resulte are returned in the Intent object
            //             if (sAction == "android.nfc.action.NDEF_DISCOVERED")
            //             {
            //                 Debug.Log("Tag of type NDEF");
            //                 AndroidJavaObject[] rawMsg = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
            //                 AndroidJavaObject[] records = rawMsg[0].Call<AndroidJavaObject[]>("getRecords");
            //                 byte[] payLoad = records[0].Call<byte[]>("getPayload");
            //                 string result = System.Text.Encoding.Default.GetString(payLoad); // not sure if it works for all encodings, but it works for me


            //                 tag_output_text.text = result;
            //                 rfid = result;
            //                 rfidtext.text = result;
            //                 // rfid = System.Text.Encoding.Default.GetString(payLoad);
            //                 scanned = true;




            //             }
            //             else if (sAction == "android.nfc.action.TECH_DISCOVERED")
            //             {
            //                 Debug.Log("TAG DISCOVERED");
            //                 // Get ID of tag
            //                 AndroidJavaObject mNdefMessage = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
            //                 if (mNdefMessage != null)
            //                 {
            //                     byte[] payLoad = mNdefMessage.Call<byte[]>("getId");
            //                     string text = System.Convert.ToBase64String(payLoad);
            //                     tag_output_text.text += "This is your tag text: " + text;
            //                     Destroy(GetComponent("MeshRenderer")); //Destroy Box when NFC ID is displayed
            //                     tagID = text;
            //                 }
            //                 else
            //                 {
            //                     tag_output_text.text = "No ID found !";
            //                 }
            //                 tagFound = true;
            //                 // How to read multiple tags maybe with this line mIntent.Call("removeExtra", "android.nfc.extra.TAG");
            //                 return;
            //             }
            //             else if (sAction == "android.nfc.action.TAG_DISCOVERED")
            //             {
            //                 Debug.Log("This type of tag is not supported !");
            //             }
            //             else
            //             {
            //                 tag_output_text.text = "Scan a NFC tag to make the cube disappear...";
            //                 return;
            //             }
            //         }
            //         catch (Exception ex)
            //         {
            //             string text = ex.Message;
            //             tag_output_text.text = text;
            //         }
            //     }
            // }
        }
    }



    IEnumerator PostFind(string isiRFID)
    {
        // string rfid = TestRFID;
        // string DataRFID = rfid;



        // Membuat objek JSON untuk dikirim ke server
        // string jsonRequestBody = "{\"rfid\":\"" + isiRFID + "\"}";
        // byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        // Membuat objek UnityWebRequest untuk POST request
        // UnityWebRequest request = new UnityWebRequest(apiURL, method);

        // Menetapkan header
        string url;
        if (isiRFID.StartsWith("en") && isiRFID.Length > 2)
        {
            isiRFID = isiRFID.Substring(2);
            url = apiURL + isiRFID;
        }
        else
        {
            url = apiURL + isiRFID;
        }
        rfidtext.text = url;


        UnityWebRequest request = UnityWebRequest.Get(url);
        request.chunkedTransfer = false;
        yield return request.Send();

        // Mengirim request
        // yield return request.SendWebRequest();

        // Memeriksa apakah ada error
        if (request.isNetworkError || request.isHttpError)
        {
            string responseText = request.downloadHandler.text;
            // Debug.LogError("Response: " + responseText + "body : " + jsonRequestBody);
            consoleText.text = "Response: " + responseText;
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
            consoleText.text = "Response: " + responseText;
        }
    }

    // IEnumerator PostRedeem()
    // {
    //     int id_player = UserProfileVar.id;
    //     string custom_name = CustomName.text;
    //     // string rfid = TestRFID;
    //     // string rfid = nfc.rfid;
    //     string DataRFID = rfid;

    //     // Membuat objek JSON untuk dikirim ke server
    //     string jsonRequestBody = "{\"rfid\":\"" + DataRFID + "\",\"custom_name\":\"" + custom_name + "\",\"id_player\":\"" + id_player + "\"}";
    //     byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
    //     Debug.Log(jsonRequestBody);

    //     // Membuat objek UnityWebRequest untuk POST request
    //     UnityWebRequest request = new UnityWebRequest(apiURL, method);

    //     // Menetapkan header
    //     request.SetRequestHeader("Content-Type", "application/json");
    //     request.uploadHandler = new UploadHandlerRaw(jsonBytes);
    //     request.downloadHandler = new DownloadHandlerBuffer();

    //     // Mengirim request
    //     yield return request.SendWebRequest();

    //     // Memeriksa apakah ada error
    //     if (request.isNetworkError || request.isHttpError)
    //     {
    //         string responseText = request.downloadHandler.text;
    //         Debug.LogError("Response: " + responseText);
    //         consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
    //     }
    //     else
    //     {
    //         // Menampilkan respons di console log
    //         string responseText = request.downloadHandler.text;
    //         reset();
    //         consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
    //         Debug.Log("Response: " + responseText);
    //     }
    // }
}