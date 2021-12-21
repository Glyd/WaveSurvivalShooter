using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public Vector3 rotation;

    private Rigidbody2D _body;
    private Collider2D _collider;
    private Vector3 movementDirection;
    private bool _shouldDeflect = false;
    private int _deflectionTimes = 0; 
    private float _speed;
    private float _baseSpeed = 75f;
    private float _projectileDamage = 25;
    private float _projectileBaseMaxTravelTime = 1f;
    private float _projectileMaxTravelTime = 1f;
    private float _projectileCurrentTravelTime;
    private float _maxDeflections = 1;

    private Vector3 lastFrameVelocity;

    [SerializeField] private PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _collider = GetComponent<Collider2D>();
        movementDirection = _player.getDirection() == 1 ? transform.right : -transform.right;
        _body.velocity = transform.right * _speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      lastFrameVelocity = _body.velocity;
    }

    void Update() {
        _projectileCurrentTravelTime += Time.deltaTime;

        if (_projectileCurrentTravelTime >= _projectileMaxTravelTime) Destroy(gameObject);
    }

    // TODO refactor
    private bool isWorldHit(Collision2D collision) {

        bool isWorldHit;

        if (_shouldDeflect && _deflectionTimes <= _maxDeflections) {
            isWorldHit = gameObject != null
            && !collision.gameObject.CompareTag("Projectile")
            && !collision.gameObject.CompareTag("Player")
            && !collision.gameObject.CompareTag("Water");
        } else {
            isWorldHit = gameObject != null
            && !collision.gameObject.CompareTag("Projectile")
            && !collision.gameObject.CompareTag("Player")
            && !collision.gameObject.CompareTag("Enemy")
            && !collision.gameObject.CompareTag("Water");
        }

        return isWorldHit;
    }

    private bool isEnemyHit(Collision2D collision) {
        return gameObject != null && collision.gameObject.CompareTag("Enemy");
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isWorldHit(collision)) {
            if (_shouldDeflect && _deflectionTimes <= _maxDeflections) {
                deflectHitHandle(collision.contacts[0].normal);
                enemyHitHandle(collision);
            }
            else {
                Destroy(gameObject);
            }
        } else {
            enemyHitHandle(collision);
        }
    }

    private void deflectHitHandle(Vector3 collisionNormal) {
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
        _body.velocity = direction * _speed;
        _deflectionTimes++;
    }

    private void enemyHitHandle(Collision2D collision) {
        if (isEnemyHit(collision)) {
            collision.gameObject.GetComponent<Enemy>().takeDamage(_projectileDamage, _collider);
            if (!_shouldDeflect || _deflectionTimes == _maxDeflections)
                Destroy(gameObject);
        }
    }

    //used by PrimaryWeapon to cut down on calls
    public void setupProjectile(int damage, int speedUpgradeCount, int rangeUpgradeCount, int bounceUpgradeCount) {
        this._projectileDamage = damage;

        _speed = _baseSpeed + 15 * speedUpgradeCount;
        _projectileMaxTravelTime = _projectileBaseMaxTravelTime + 0.3f * rangeUpgradeCount;
        _maxDeflections = 1 + bounceUpgradeCount;
        _shouldDeflect = _maxDeflections > 1;

    }
}
