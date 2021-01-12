using UnityEngine;

public class Laser : MonoBehaviour
{
    //speed variable of 8
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private bool _isEnemyLaser;
    [SerializeField] private bool _isPlayerLaser;
    private GameObject _player;
    private Rigidbody2D _rb;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(_isEnemyLaser)
            CalculateMovement(Vector3.down);
        else if(_isPlayerLaser)
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
