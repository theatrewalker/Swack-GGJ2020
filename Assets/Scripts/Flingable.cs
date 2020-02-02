using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flingable : MonoBehaviour
{
    public float flingDistance = 1f;
    public float flingForce = 5f;
    public bool grabbed = false;
    private Vector3 offset;
    private Vector3 screenPoint;
    private Vector3 origPosition;

    private Vector2 retainedVel;
    private float retainedAngularVel;
    private float BASE_SCALE;


    // Start is called before the first frame update
    void Start()
    {
        this.BASE_SCALE = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        bool focus =  (!LevelManager.CurrentLevel().hasWon) && grabbed;
        var targetColor = focus ? Color.cyan : Color.white;
        var targetScaleAmount = BASE_SCALE * (focus ? 1.05f : 1);
        var targetScale = new Vector3(targetScaleAmount, targetScaleAmount, targetScaleAmount);
        GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, targetColor, 5f * Time.deltaTime);
        transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, 5f * Time.deltaTime);
    }

    void OnMouseDown()
    {
        if (!LevelManager.CurrentLevel().hasStarted)
        {
            return;
        }
        grabbed = true;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        origPosition = transform.position;
        var rb = GetComponent<Rigidbody2D>();
        retainedAngularVel = rb.angularVelocity;
        retainedVel = rb.velocity;
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
    }

    void OnMouseUp()
    {
        if (!grabbed) return;
        grabbed = false;
        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = retainedVel;
        rb.angularVelocity = retainedAngularVel;
    }

    void OnMouseDrag()
    {
        if (!grabbed) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        Vector3 diff = curPosition - transform.position;
        float diffMag = diff.magnitude;
        if (diffMag < flingDistance)
        {
       //     transform.position = curPosition;
        }
        else
        {
            grabbed = false;
            var rb = GetComponent<Rigidbody2D>();
            rb.velocity = diff * flingForce;
        }
    }
}
