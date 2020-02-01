using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var satalites = FindObjectsOfType<Satalite>();
        foreach(Satalite s in satalites){
            var rb = s.GetComponent<Rigidbody2D>();
            var vx = Random.Range(-1, 1) * 10f;
            var vy = Random.Range(-1, 1) * 10f;
            rb.AddForce(new Vector2(vx, vy));
            rb.AddTorque(Random.Range(-80.0f, 80.0f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
