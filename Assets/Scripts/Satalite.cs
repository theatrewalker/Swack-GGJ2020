using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading.Tasks;
using UnityEngine;

public class Satalite : MonoBehaviour
{
    public int targetChildCount = 0;
    public int childCount = 0;
    public float positionalSnapThreshold = 0.5f;
    public float rotationalSnapThreshold = 10f;
    public float topBottomBounds = 4.5f;
    public float leftRightBounds = 10f;
    public float TopBottomForce = 5f;
    public float ShakeSpeed = 1f;
    public float ShakeAmount = 0.01f;
    public bool breakingUp = false;
    public bool shaking = true;
    public float ExplosionForce = 5f;
    public AudioClip collisionSound;

    private List<GameObject> overlappingObjects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = getCOM();
    }

    Vector3 getCOM()
    {
        foreach (Transform o in transform)
        {
            if (o.name == "COM")
            {
                var com = o.transform.localPosition;
                return com;
            }
        }
        throw new System.Exception("Satalite has no Center of Mass object, must be named 'COM'");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GlobalGameManager().hasWon)
        {
            return;
        }
        var rb = GetComponent<Rigidbody2D>();
        if (this.transform.position.y > topBottomBounds)
        {
            rb.AddForce(new Vector2(0, -TopBottomForce));
        }
        if (this.transform.position.y < -1 * topBottomBounds)
        {
            rb.AddForce(new Vector2(0, TopBottomForce));
        }

        if(this.transform.position.x < -1 * leftRightBounds && rb.velocity.x < 0)
        {
            transform.position -= 2 * new Vector3(transform.position.x, 0);
        }
        if (this.transform.position.x > leftRightBounds && rb.velocity.x > 0)
        {
            transform.position -= 2 * new Vector3(transform.position.x, 0);
        }

        if (shaking)
        {
            transform.position += new Vector3(
                Mathf.Sin(Time.time * ShakeSpeed) * ShakeAmount,
                Mathf.Sin(Time.time * ShakeSpeed * Mathf.PI) * ShakeAmount
            );
        }
    }

    public string getName()
    {
        var sprite =  GetComponent<SpriteRenderer>().sprite;
        return sprite.name;
    }

    public bool canConnectTo(Satalite other)
    {
        return other.getName().Contains(getName());
    }

    private void snap(Satalite other)
    {
        Debug.Log("SNAP!");
        other.transform.position = transform.position;
        other.GetComponent<Rigidbody2D>().rotation = GetComponent<Rigidbody2D>().rotation;
        var fj = this.gameObject.AddComponent<FixedJoint2D>();
        fj.connectedBody = other.GetComponent<Rigidbody2D>();
        this.childCount++;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        overlappingObjects.Add(col.gameObject);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        overlappingObjects.Remove(col.gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (breakingUp) return;
        var other = col.collider.GetComponent<Satalite>();
        if (other)
        {
            if (GameManager.GlobalGameManager().hasStarted)
            {
                AudioSource.PlayClipAtPoint(collisionSound, this.transform.position);
            }
            if (!canConnectTo(other)) return;
            var rb = GetComponent<Rigidbody2D>();
            var otherRb = other.GetComponent<Rigidbody2D>();
            var rotationDifference = Quaternion.Angle(
                Quaternion.AngleAxis(rb.rotation, Vector3.forward), 
                Quaternion.AngleAxis(otherRb.rotation, Vector3.forward)
            );
            if (rotationDifference > rotationalSnapThreshold)
            {
                Debug.Log("Rotation too different = "+rotationDifference);
                return;
            }
            var positionalDifference = (rb.transform.position - otherRb.transform.position).magnitude;
            if (positionalDifference > positionalSnapThreshold)
            {
                Debug.Log("positionalDifference too different = " + positionalDifference);
                return;
            }
            snap(other);
        }
    }

    public void BreakUp()
    {
        GetComponent<PolygonCollider2D>().isTrigger = true;
        this.childCount = 0;
        this.breakingUp = true;  
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();
        foreach (FixedJoint2D joint in joints)
        {
            Destroy(joint);
            Debug.Log(joint);
        }
        Vector3 vecToCenter =  getCOM() - this.transform.position;
        var rb = GetComponent<Rigidbody2D>();
        rb.AddForce(vecToCenter.normalized * ExplosionForce);
       // this.GetComponent<SpriteRenderer>().color = Color.red;
    }
    public async Task StopBreakingUp()
    {
        int successCount = 0;
        while (true)
        {
            if (overlappingObjects.Count != 0)
            {
                successCount = 0;
                await Task.Delay(TimeSpan.FromSeconds(0.01));
            }
            else
            {
                successCount++;
                if(successCount > 10)
                {
                    break;
                }
            }
        }
        this.GetComponent<SpriteRenderer>().color = Color.white;
        this.breakingUp = false;
        this.shaking = false;
        GetComponent<PolygonCollider2D>().isTrigger = false;
    }
}
