using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Starting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetRandomValue()
    {
        PlayerParam.Player1.Index = Random.Range(1, 888); // Min: inclusive, Max: exclusive.
        GameObject.Find("PlayerIndex").GetComponent<Text>().text = PlayerParam.Player1.Index.ToString();
    }

    public void ChooseServer()
    {
        PlayerParam.ipAddress = GameObject.Find("ipAddress").GetComponent<InputField>().text;
        // Debug.Log(PlayerParam.ipAddress);
        SceneManager.LoadScene("Server");
    }

    public void ChooseClient()
    {
        PlayerParam.ipAddress = GameObject.Find("ipAddress").GetComponent<InputField>().text;
        // Debug.Log(PlayerParam.ipAddress);
        SceneManager.LoadScene("Client");
    }
}
