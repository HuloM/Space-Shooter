using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //speed variable of 8
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private bool _isEnemyLaser; 
    private void Update()
    {
        if(_isEnemyLaser)
            CalculateMovement(Vector3.down);
        else
            CalculateMovement(Vector3.up);
    }

    private void CalculateMovement(Vector3 vector3)
    {
        transform.Translate(vector3 * (_speed * Time.deltaTime));

        if (transform.position.y > 8.0f || transform.position.y < -6.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}
