using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumppad : MonoBehaviour
{
   private float bounce = 15f;

private void OnCollisionEnter2D(Collision2D Collision)
{
    if(Collision.gameObject.CompareTag("Player"))
    {
        Collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
    }
}
}
