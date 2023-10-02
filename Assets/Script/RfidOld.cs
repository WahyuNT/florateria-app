using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RfidOld : MonoBehaviour
{
    public string rfidOld;
    public Text rfidOldText;
    void Update()
    {
        rfidOldText.text = rfidOld;
    }

}
