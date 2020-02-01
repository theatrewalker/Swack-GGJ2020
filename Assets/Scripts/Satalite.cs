using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satalite : MonoBehaviour
{
    public float positionalSnapThreshold = 0.5f;
    public float rotationalSnapThreshold = 10f;
    public float topBottomBounds = 4.5f;
    public float TopBottomForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform o in transform)
        {
            if (o.name == "COM")
            {
                var com = o.transform.localPosition;
                var rb = GetComponent<Rigidbody2D>();
                rb.centerOfMass = com;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (this.transform.position.y > topBottomBounds)
        {
            rb.AddForce(new Vector2(0, -TopBottomForce));
        }
        if (this.transform.position.y < -1 * topBottomBounds)
        {
            rb.AddForce(new Vector2(0, TopBottomForce));
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
        Debug.Log(this.getName()+" -> "+other.getName());
        other.transform.position = transform.position;
        other.GetComponent<Rigidbody2D>().rotation = GetComponent<Rigidbody2D>().rotation;
        var fj = this.gameObject.AddComponent<FixedJoint2D>();
        fj.connectedBody = other.GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        var other = col.collider.GetComponent<Satalite>();
        if (other)
        {
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
            };
            snap(other);
        }
    }
}
