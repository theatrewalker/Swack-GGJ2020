﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTeleporter : MonoBehaviour
{
    public int vectorDirection = 1;
    public float teleportDistance = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.GetComponent<Rigidbody2D>();
        if (Mathf.Sign(rb.velocity.x) == Mathf.Sign(vectorDirection))
        {
            rb.position -= new Vector2(vectorDirection * teleportDistance, 0);
        }
    }

}