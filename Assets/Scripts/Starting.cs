﻿using System.Collections;
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
        if ( Input.GetKeyDown("escape") ) Back_to_GameStart();
    }

    public void GetRandomValue()
    {
        int index = Random.Range(1, 888); // Min: inclusive, Max: exclusive.
        PlayerParam.Server.Player_Index = index;
        PlayerParam.Client.Player_Index = index;
        GameObject.Find("PlayerIndex").GetComponent<Text>().text = index.ToString();
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

    public void Back_to_GameStart()
    {
        SceneManager.LoadScene("Starting");
    }
}
