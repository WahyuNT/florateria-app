using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public UserProfile User;
    public InputField Name;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Name.text = User.name;
    }

    public void Logout()
    {
        User.id = 0;
        User.id_role = 0;
        User.name = null;
        User.password = null;
        User.created_at = null;
        User.updated_at = null;
    }
}
