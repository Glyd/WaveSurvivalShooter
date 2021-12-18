using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public GameObject heartPickupPrefab;

    private float _health = 100;
    private int _contactDamage = 10;
    private AIDestinationSetter destinationSetter;
    private AIPath _aiPath;
    private Transform _playerTransform;
    private Rigidbody2D _body;
    private Collider2D _collider;
    private GameDirector _gameDirector;
    private SpriteRenderer _spriteRenderer;
    private bool _reportedDeath = false;
    private int heartDropRate = 5; // percentage
    private int _currentLevel = 0;
    private int _levelLastBuffed;

    // Start is called before the first frame update
    void Start()
    {
        _body = gameObject.GetComponent<Rigidbody2D>();
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        _aiPath = gameObject.GetComponent<AIPath>();
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        destinationSetter.target = _playerTransform;
        _collider = GetComponent<Collider2D>();
        _gameDirector = GameObject.FindWithTag("GameDirector").GetComponent<GameDirector>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_health <= 0) {
            handleDeath();
        }

        if (_currentLevel % 5 == 0 && _levelLastBuffed != _currentLevel) {
            _increaseStats();
            _levelLastBuffed = _currentLevel;
        }
    }

    private void handleDeath() {
        _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        _aiPath.canSearch = false;
        setContactDamage(0);
        if (!_reportedDeath) _gameDirector.reportDeath();
        if (Random.Range(0, 100) < heartDropRate && !_reportedDeath) createHeartPickup();
        _reportedDeath = true;
        _body.simulated = false;
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
       if (isEnemyHit(collision) && !_reportedDeath) {
            collision.gameObject.GetComponent<PlayerController>().takeDamage(_contactDamage, _collider);
        }
    }

    private bool isEnemyHit(Collision2D collision) {
        return gameObject != null && collision.gameObject.CompareTag("Player");
    }

    public void takeDamage(float damageAmount, Collider2D projectileCollider) {
        _health -= damageAmount;
        StartCoroutine(_knockbackHandling(projectileCollider));
        StartCoroutine(_flashOnHit());
    }

    public void setHealth(int health) {
        this._health = health;
    }

    public void setContactDamage(int contactDamage) {
        this._contactDamage = contactDamage;
    }

    private IEnumerator _knockbackHandling(Collider2D projectileCollider) {
        Vector3 moveDirection = _body.transform.position - projectileCollider.transform.position;
        _body.simulated = false;
        yield return new WaitForSeconds(0.1f);
        _body.velocity = Vector3.zero;
        _body.simulated = true;
    }

    private IEnumerator _flashOnHit() {
        Color previousColour = _spriteRenderer.color;
        _spriteRenderer.color = new Color(255f, 1f, 1f);
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.color = previousColour;
    }

    private void createHeartPickup() {
        Instantiate(heartPickupPrefab, transform.position, Quaternion.identity);
    }

    private void _increaseStats() {
        _health = _health + 25 * _currentLevel / 5;
        _contactDamage = _contactDamage + 5 * _currentLevel / 5;
    }

    public void setCurrentLevel(int currentLevel) {
        _currentLevel = currentLevel;
    }
}
