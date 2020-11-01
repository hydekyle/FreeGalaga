using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public BoostType boostType;
    Rigidbody2D myRB;
    float nextTimeBounce;
    float minX, maxX;

    private void Awake ()
    {
        myRB = myRB ?? GetComponent<Rigidbody2D> ();
    }

    private void Start ()
    {
        minX = GameManager.Instance.minPosX;
        maxX = GameManager.Instance.maxPosX;
    }

    private void Update ()
    {
        if (transform.position.x > maxX || transform.position.x < minX) Bounce ();
    }

    void Bounce ()
    {
        if (Time.time < nextTimeBounce) return;
        myRB.velocity = new Vector2 (myRB.velocity.x * -1, myRB.velocity.y);
        nextTimeBounce = Time.time + 1.5f;
    }

    private void OnEnable ()
    {
        Vector2 forceDirection = Vector2.up * 2 + Vector2.right * Random.Range (-1.2f, 1.2f);
        myRB.AddForce (forceDirection, ForceMode2D.Impulse);
    }

}
