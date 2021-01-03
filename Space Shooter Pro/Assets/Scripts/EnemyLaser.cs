using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    
    [SerializeField]private float _speed = 8.0f;

    private void Start()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        transform.Translate(Vector3.down * (Time.deltaTime * _speed));
       
        if(transform.position.y < -6f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
                player.Damage();
            
            Destroy(gameObject);
        }
    }
}
