using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class SceneChangeManager : MonoBehaviour
{
    private static SceneChangeManager sm;
    private SpriteRenderer blackoutSprite;

    private bool fadingOut = false;
    private bool fadingIn = false;
    private Color trasparent = new Color(0, 0, 0, 0);
    public float FadeTime = 1f;
    public float WaitTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if (!sm)
        {
            sm = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        this.blackoutSprite = GetComponent<SpriteRenderer>();
        blackoutSprite.color = trasparent;
        DontDestroyOnLoad(blackoutSprite);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingOut)
        {
            blackoutSprite.color = Color.Lerp(blackoutSprite.color, Color.black, FadeTime * Time.deltaTime);
        }
        if (fadingIn)
        {
            blackoutSprite.color = Color.Lerp(blackoutSprite.color, trasparent, FadeTime * Time.deltaTime);
        }
    }

    public static async Task FadeToScene(string Scene)
    {
        sm.fadingOut = true;
        sm.fadingIn = false; ;
        await Task.Delay(TimeSpan.FromSeconds(sm.WaitTime));
        SceneManager.LoadScene(Scene);
        sm.fadingOut = false;
        sm.fadingIn = true;
    }
}
