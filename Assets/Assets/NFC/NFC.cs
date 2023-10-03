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

    public Text tag_output_text;
    public bool tagFound = false;
    public bool scanned = false;
    public bool rfidFound = false;
    public bool scanning = true;
    public GameObject RedeemBox;
    public RfidOld RfidOld;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;

    public string ImageLink;
    public string apiURL;
    public string redeemURL;
    public string nama_tanaman;
    public string RedemMethod;
    public string RFID;
    public InputField CustomName;
    public UserProfile UserProfileVar;
    public Text consoleText;
    public Text rfidtext;
    public Text log;
    public Text jenisPlant;
    public RawImage iconImage;
    AndroidJavaObject[] records = null;
    AndroidJavaObject[] rawMsg;

    [System.Serializable]
    public class User
    {
        public string icon;
        public string name;

    }

    void Start()
    {
        tag_output_text.text = "Scan a NFC tag to make the cube disappear...";
        UserProfileVar = transform.parent.parent.parent.GetComponent<UserProfile>();
        RfidOld = transform.parent.GetComponent<RfidOld>();
    }
    public void reset()
    {
        RFID = null;
        tagFound = false;
        RedeemBox.SetActive(false);
        tagID = null;
        ImageLink = null;
        nama_tanaman = null;
        records = null;
        CustomName.text = null;
        scanned = false;
        scanning = false;

    }

    public void MulaiScan()
    {
        scanning = true;
    }
    void Update()
    {

        if (!string.IsNullOrEmpty(ImageLink) && RfidOld.rfidOld != RFID)
        {
            RedeemBox.SetActive(true);
            RfidOld.rfidOld = RFID;
            StartCoroutine(LoadImage());

        }
        if (!string.IsNullOrEmpty(RFID) && scanned == false && RfidOld.rfidOld != RFID)
        {

            StartCoroutine(PostFind(RFID));

            scanned = true;

        }

        if (scanning == true)
        {
            scan();
        }
        if (ImageLink == "none")
        {
            reset();
        }

    }
    public void scan()
    {
        Debug.Log($"scan jalan");

        try
        {
            // Create new NFC Android object
            mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
            mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
            sAction = mIntent.Call<String>("getAction"); // resulte are returned in the Intent object
            if (sAction == "android.nfc.action.NDEF_DISCOVERED")
            {
                Debug.Log("Tag of type NDEF");
                rawMsg = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
                records = rawMsg[0].Call<AndroidJavaObject[]>("getRecords");

                if (records.Length > 0)
                {

                    AndroidJavaObject record = records[0];
                    byte[] payload = record.Call<byte[]>("getPayload");

                    // Extract the Text Encoding
                    String textEncoding = ((payload[0] & 0x80) == 0) ? "UTF-8" : "UTF-16";

                    // Extract the Language Code
                    int languageCodeLength = payload[0] & 0x3F;
                    String languageCode = System.Text.Encoding.ASCII.GetString(payload, 1, languageCodeLength);

                    // Extract the Text
                    String text = System.Text.Encoding.GetEncoding(textEncoding).GetString(payload, languageCodeLength + 1, payload.Length - languageCodeLength - 1);



                    RFID = text;
                    tag_output_text.text = RFID;

                    scanning = false;

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
        StartCoroutine(PostRedeem());
    }
    private IEnumerator LoadImage()
    {
        using (WWW www = new WWW(ImageLink))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("Error loading image: " + www.error);
            }
            else
            {
                Texture2D tex = new Texture2D(2, 2); // Buat texture baru
                www.LoadImageIntoTexture(tex); // Muat gambar ke texture
                iconImage.texture = tex; // Set texture pada komponen RawImage
            }
        }
    }

    IEnumerator PostFind(string isiRFID)
    {

        string url;
        url = apiURL + isiRFID;
        rfidtext.text = url;


        UnityWebRequest request = UnityWebRequest.Get(url);
        request.chunkedTransfer = false;
        yield return request.Send();


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
            nama_tanaman = user.name;
            jenisPlant.text = nama_tanaman;
            if (string.IsNullOrEmpty(ImageLink))
            {
                ImageLink = "none";

            }

            Debug.Log("Response: " + responseText);
            consoleText.text = "Response: " + responseText;
        }
    }

    IEnumerator PostRedeem()
    {
        int id_player = UserProfileVar.id;
        string custom_name = CustomName.text;
        string DataRFID = RFID;

        // Membuat objek JSON untuk dikirim ke server
        string jsonRequestBody = "{\"rfid\":\"" + DataRFID + "\",\"custom_name\":\"" + custom_name + "\",\"id_player\":\"" + id_player + "\"}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        Debug.Log(jsonRequestBody);

        // Membuat objek UnityWebRequest untuk POST request
        UnityWebRequest request = new UnityWebRequest(redeemURL, RedemMethod);

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
            reset();
            consoleText.text = "Response: " + responseText + "body : " + jsonRequestBody;
            Debug.Log("Response: " + responseText);
        }
    }
}