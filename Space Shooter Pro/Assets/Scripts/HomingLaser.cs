using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLaser : MonoBehaviour
{
    //speed variable of 8
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private bool _isPlayerLaser;
    [SerializeField] private GameObject _enemyPool;

    private Transform[] _enemyTransforms;
    private Transform _target;
    private void Start()
    {
        _enemyPool = GameObject.FindGameObjectWithTag("EnemyContainer");
        if(_enemyPool != null)
        {
            var count = 0;
            foreach (Transform child in _enemyPool.transform)
            {
                _enemyTransforms[count] = child;
                count++;
            }

            _target = GetClosestEnemy(_enemyTransforms);
        }
        else
            Debug.Log("unexpected error: could not find any enemies to home");
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        var step = _speed * Time.deltaTime;
        
        if(_target != null)
            transform.position = Vector3.MoveTowards(transform.position, _target.position, step);
        
        if (transform.position.y > 8.0f || transform.position.y < -6.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
    Transform GetClosestEnemy (Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach(Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
     
        return bestTarget;
    }
}
