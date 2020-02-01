using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        
        var satalites = FindObjectsOfType<Satalite>();
        foreach (Satalite s in satalites)
        {
            var rb = s.GetComponent<Rigidbody2D>();
            rb.AddTorque(15f);
            s.shaking = true;
        }
        await Task.Delay(TimeSpan.FromSeconds(3));
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
