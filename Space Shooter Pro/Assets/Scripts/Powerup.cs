using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private PowerupID _powerupID; //0 = tripleShot, 1 = speed, 2 = shield
    [SerializeField] private AudioClip _powerupClip;
    private Player _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if(_player == null)
            Debug.Log("player not found");
    }

    private void Update()
    {
        CalculateMovement();

        if(transform.position.y < -6f)
            Destroy(gameObject);
    }

    private void CalculateMovement()
    {
        if (Input.GetKey(KeyCode.C))
        {
            var direction = _player.transform.position - transform.position;
            transform.Translate(direction * Time.deltaTime);
        }
        else
            transform.Translate(Vector3.down * (Time.deltaTime * _speed));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                switch (_powerupID)
                {
                    case PowerupID.TripleShot:
                        _player.OnTripleShotPickup();
                        Debug.Log("triple shot collected");
                        break;
                    case PowerupID.Speed:
                        _player.OnSpeedPickup(2f);
                        Debug.Log("speed collected");
                        break;
                    case PowerupID.Shield:
                        _player.OnShieldPickup();
                        Debug.Log("shield collected");
                        break;
                    case PowerupID.AmmoRefill:
                        _player.OnAmmoRefillPickup();
                        Debug.Log("ammo refill collected");
                        break;
                    case PowerupID.Heal:
                        _player.OnHealPickup();
                        Debug.Log("heal collected");
                        break;
                    case PowerupID.MultiShot:
                        _player.OnMultiShotPickup();
                        Debug.Log("multi shot collected");
                        break;
                    case PowerupID.NegativeHeal:
                        _player.Damage();
                        Debug.Log("negative heal collected");
                        break;
                    case PowerupID.NegativeSpeed:
                        _player.OnSpeedPickup(0.5f);
                        Debug.Log("negative speed collected");
                        break;
                    default:
                        Debug.Log("powerup does not have a valid ID");
                        break;
                }
                AudioSource.PlayClipAtPoint(_powerupClip, transform.position);
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("EnemyLaser"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}