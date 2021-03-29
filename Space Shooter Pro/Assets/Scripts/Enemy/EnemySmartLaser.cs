using UnityEngine;

public class EnemySmartLaser : MonoBehaviour
{
    [SerializeField] private float _speed = 2.0f;
    private GameObject _player;

    private float _timeToDestroy;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        gameObject.GetComponent<AudioSource>().Play();

        _timeToDestroy = 1.0f;
    }

    private void Update()
    {
        var targetPos = _player.transform.position;
        var position = transform.position;
        
        var step = _speed * Time.deltaTime;
        var offset = targetPos - position;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);
        position = Vector3.MoveTowards(position, targetPos, step);
        transform.position = position;

        _timeToDestroy -= Time.deltaTime;
        
        if(transform.position.y < -6f || _timeToDestroy <= 0)
            Destroy(gameObject);
    }
}
