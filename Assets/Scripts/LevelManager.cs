using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    static private LevelManager gm;
    public bool hasWon = false;
    public bool hasStarted = false;
    public AudioClip victorySound;
    public AudioClip explodeSound;

    static public LevelManager CurrentLevel()
    {
        return gm;
    }

    // Start is called before the first frame update
    async void Start()
    {
        if (!gm) gm = this;

        var satalites = FindObjectsOfType<Satalite>();
        foreach (Satalite s in satalites)
        {
            var rb = s.GetComponent<Rigidbody2D>();
            rb.AddTorque(15f);
            s.shaking = true;
        }
        await Task.Delay(TimeSpan.FromSeconds(3));

        AudioSource.PlayClipAtPoint(explodeSound, this.transform.position);
        foreach (Satalite s in satalites) { 
            s.BreakUp();
            var rb = s.GetComponent<Rigidbody2D>();
            var vx = UnityEngine.Random.Range(-1, 1) * 10f;
            var vy = UnityEngine.Random.Range(-1, 1) * 10f;
            rb.AddForce(new Vector2(vx, vy));
            rb.AddTorque(UnityEngine.Random.Range(-80.0f, 80.0f));
        }
        await Task.Delay(TimeSpan.FromSeconds(2));
        foreach (Satalite s in satalites)
        {
            s.StopBreakingUp();
        }
        hasStarted = true;
    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (hasStarted && !hasWon)
        {
            var satalites = FindObjectsOfType<Satalite>();
            foreach (Satalite s in satalites)
            {
                if (s.childCount != s.targetChildCount) return;
            }
            await Victory();
        }
    }

    async Task Victory()
    {
        AudioSource.PlayClipAtPoint(victorySound, this.transform.position);
        hasWon = true;
        var satalites = FindObjectsOfType<Satalite>();
        for(var i = 0; i < 30; i++)
        {
            foreach (Satalite s in satalites)
            {
                s.GetComponent<SpriteRenderer>().color *= new Color(0.98f, 0.98f, 0.98f);
                s.transform.localScale *= .99f;
            }
            await Task.Delay(TimeSpan.FromSeconds(0.01));
        }
        for (var i = 0; i < 50; i++)
        {
            foreach (Satalite s in satalites)
            {
                var rb = s.GetComponent<Rigidbody2D>();
                rb.AddForce(new Vector3(-20f, 0, 0));
            }
            await Task.Delay(TimeSpan.FromSeconds(0.01));
        }
    }
}
