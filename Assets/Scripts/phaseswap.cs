using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class phaseswap : MonoBehaviour
{
    bool buildPhase = true;
    //i hate capital letters so they will not be used for my comments
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void KeyChungus()
    {
        //b key is the enter/exit build mode button
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (buildPhase)
            {
                buildPhase = false;
                Debug.Log("switched to play mode");
                InitRunPhase();
                return;
            }
            else
            {
                buildPhase = true;
                InitBuildPhase();
                Debug.Log("switched to build mode");
            }


        }
    }
    // Update is called once per frame
    void Update()
    {
        KeyChungus();
        if (buildPhase) 
        {
            BuildPhase();
        } else
        {
            PlayPhase();
        }
    }

    void InitBuildPhase()
        //happens upon entering build mode
    {
        //this is the thing that changes the cursor to being free (for being in build mode)
        Cursor.lockState = CursorLockMode.Locked;
        // Freeze all objects
        // Reset Character pos
    }
    void InitRunPhase()
        //happens upon entering run mode
    {
        //this is the thing that changes the cursor to being locked (for being in play mode)
        Cursor.lockState = CursorLockMode.None;
    }
    void BuildPhase()
        //thing on every frame that build mode is enabled
    {
        //build stuff here
    }
    void PlayPhase()
        //thing on every frame that play mode is enabled
    {
        //movement, win conditions, stats and blocks go here
    }

}
