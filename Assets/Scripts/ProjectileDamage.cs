using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour {

    private float knockback = 0.75f; //feel free to change value
    
    private int damage = 15;

    public bool isPlayer = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Enemy") && isPlayer)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.collider.SendMessageUpwards("TakeDamage", damage);
            collision.collider.SendMessageUpwards("TakeKnockback", knockback);
        }
        else if (collision.collider.CompareTag("Player") && !isPlayer)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.collider.SendMessageUpwards("TakeDamage", damage);
            collision.collider.SendMessageUpwards("TakeKnockback", knockback);
        }

        Destroy(gameObject);
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy") && isPlayer)
        {
            //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.SendMessageUpwards("TakeDamage", damage);
            other.SendMessageUpwards("TakeKnockback", knockback);
            Destroy(transform.parent.parent.gameObject);
        }
        else if (other.CompareTag("Player") && !isPlayer)
        {
            //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.SendMessageUpwards("TakeDamage", damage);
            other.SendMessageUpwards("TakeKnockback", knockback);
            Destroy(transform.parent.parent.gameObject);
        }

        if (!other.tag.Contains("Player") && !other.tag.Contains("Enemy") && !other.name.Contains("Projectile"))
        {
            Destroy(transform.parent.parent.gameObject);
        }
    }
}
