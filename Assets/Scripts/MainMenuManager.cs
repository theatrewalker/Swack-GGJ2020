using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private bool hasClicked = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasClicked)
        {
            hasClicked = true;
            await SceneChangeManager.FadeToScene("Level1");
        }
    }
}
