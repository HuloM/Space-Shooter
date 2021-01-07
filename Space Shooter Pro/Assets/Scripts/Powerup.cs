using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private PowerupID _powerupID; //0 = tripleShot, 1 = speed, 2 = shield
    [SerializeField] private AudioClip _powerupClip;

    private void Update()
    { 
        transform.Translate(Vector3.down * (Time.deltaTime * _speed));
       
       if(transform.position.y < -6f)
           Destroy(gameObject);
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case PowerupID.TripleShot:
                        player.OnTripleShotPickup();
                        Debug.Log("triple shot collected");
                        break;
                    case PowerupID.Speed:
                        player.OnSpeedPickup();
                        Debug.Log("speed collected");
                        break;
                    case PowerupID.Shield:
                        player.OnShieldPickup();
                        Debug.Log("shield collected");
                        break;
                    case PowerupID.AmmoRefill:
                        player.OnAmmoRefillPickup();
                        Debug.Log("ammo refill collected");
                        break;
                    case PowerupID.Heal:
                        player.OnHealPickup();
                        Debug.Log("heal collected");
                        break;
                    case PowerupID.MultiShot:
                        player.OnMultiShotPickup();
                        Debug.Log("multi shot collected");
                        break;
                    default:
                        Debug.Log("powerup does not have a valid ID");
                        break;
                }
                AudioSource.PlayClipAtPoint(_powerupClip, transform.position);
            }
            Destroy(gameObject);
        }
    }
}