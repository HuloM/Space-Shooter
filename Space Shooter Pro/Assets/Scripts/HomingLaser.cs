using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLaser : MonoBehaviour
{
    //speed variable of 8
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private Transform _player;
    [SerializeField] private float _overlapRadius = 10.0f;

    private Transform _nearestEnemy;
    private int _enemyLayer;
    private void Start()
    {
        _player = FindObjectOfType<Player>().transform;
        
        if(_player == null)
            Debug.Log("no player found");
        
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        Debug.Log(_enemyLayer);
        
        FindClosestEnemy();
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        var position = transform.position;
        var step = _speed * Time.deltaTime;

        if (_nearestEnemy != null)
        {
            var targetPos = _nearestEnemy.position;
            var offset = targetPos - position;
            transform.position = Vector3.MoveTowards(position, targetPos, step);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
        }
        else
            transform.Translate(Vector3.up * (_speed * Time.deltaTime));
        
        if (position.y > 8.0f || position.y < -6.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    private void FindClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_player.position, _overlapRadius, 1 << _enemyLayer);
        float minDist = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            float dist = Vector2.Distance(_player.position, hitCollider.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                _nearestEnemy = hitCollider.transform;
            }
        }
    }
}
