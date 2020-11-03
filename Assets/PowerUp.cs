using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public BoostType boostType;
    Rigidbody2D myRB;
    float minX, maxX, minY;
    bool horizontalBounce, verticalBounce;

    private void Awake ()
    {
        myRB = myRB ?? GetComponent<Rigidbody2D> ();
    }

    private void Start ()
    {
        minX = GameManager.Instance.minPosX;
        maxX = GameManager.Instance.maxPosX;
        minY = GameManager.Instance.minPosY;
    }

    private void Update ()
    {
        if (transform.position.x > maxX || transform.position.x < minX) BounceHorizontal ();
        if (transform.position.y < minY) BounceVertical ();
    }

    void BounceHorizontal ()
    {
        if (!horizontalBounce) return;
        myRB.velocity = new Vector2 (myRB.velocity.x * -1, myRB.velocity.y);
        horizontalBounce = false;
    }

    void BounceVertical ()
    {
        if (!verticalBounce) return;
        myRB.velocity = new Vector2 (myRB.velocity.x, myRB.velocity.y * -0.4f);
        verticalBounce = false;
    }

    private void OnEnable ()
    {
        Vector2 forceDirection = Vector2.up * 2 + Vector2.right * Random.Range (-1.2f, 1.2f);
        myRB.AddForce (forceDirection, ForceMode2D.Impulse);
        horizontalBounce = verticalBounce = true;
    }

}
