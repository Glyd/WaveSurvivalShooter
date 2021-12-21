using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public GameObject heartPickupPrefab;
    public Material defaultSpriteMat;

    private StatTracker _statTracker;
    private float _health = 100;
    private int _contactDamage = 10;
    private AIDestinationSetter destinationSetter;
    private LineRenderer lineRenderer;

    private AIPath _aiPath;
    private Transform _playerTransform;
    private Rigidbody2D _body;
    private Collider2D _collider;
    private GameDirector _gameDirector;
    private SpriteRenderer _spriteRenderer;
    private bool _reportedDeath = false;
    private int heartDropRate = 5; // percentage
    private int _currentLevel = 0;
    private bool hasTakenElectricDamage = false;
    private int electricDamage = 15;

    // Upgrades that need setting
    public bool playerHasElectric = false;

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
        _statTracker = GameObject.Find("Stats").GetComponent<StatTracker>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>(); ;

        lineRenderer.material = defaultSpriteMat;


        if (_currentLevel % 5 == 0) {
            _increaseStats();
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

    public void takeDamage(float damageAmount) {
        _health -= damageAmount;
        _statTracker.addDamageDealt((int)damageAmount);
        if ( playerHasElectric &&  !hasTakenElectricDamage) { 
            hasTakenElectricDamage = true; 
            handleElectricUpgrade(); 
        }

        if (_health <= 0) {
            handleDeath();
        }
        else {
            StartCoroutine(_flashOnHit());
        }
    }

    //Delayed damage to create visible chain effect rather than instananeous mass-hits
    public IEnumerator takeElectricDamage(float damageAmount) {
        yield return new WaitForSeconds(0.2f);

        _health -= damageAmount;
        _statTracker.addDamageDealt((int)damageAmount);

        if ( !hasTakenElectricDamage) { 
            hasTakenElectricDamage = true;
            handleElectricUpgrade();
        }

        if (_health <= 0) {
            handleDeath();
        }
        else {
            StartCoroutine(_flashOnHit());
        }
    }

    public void takeDamage(float damageAmount, Collider2D projectileCollider) {
        _health -= damageAmount;
        _statTracker.addDamageDealt((int)damageAmount);
        if (playerHasElectric && !hasTakenElectricDamage) { 
            hasTakenElectricDamage = true; 
            handleElectricUpgrade(); 
        }

        if (_health <= 0) {
            handleDeath();
        } else {
            StartCoroutine(_knockbackHandling(projectileCollider));
            StartCoroutine(_flashOnHit());
        }
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

    private void handleElectricUpgrade() {
        if (Random.Range(0, 100) <= 25) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 50f);

            foreach (Collider2D collider in colliders) {
                if (collider.gameObject.CompareTag("Enemy") && !collider.gameObject.GetComponent<Enemy>().hasTakenElectricDamage) {
                    Enemy enemy = collider.gameObject.GetComponent<Enemy>();

                    StartCoroutine(enemy.takeElectricDamage(electricDamage));

                    StartCoroutine(drawElectricLine(collider.gameObject.transform.position));
                    StartCoroutine(resetElectricDamageDelayed(enemy));
                }
            }
        }
    }

    IEnumerator drawElectricLine(Vector3 enemyPosition) {
        lineRenderer.enabled = true;

        Vector3[] positions = { enemyPosition, transform.position };


        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.SetPositions(positions);
        lineRenderer.useWorldSpace = true;

        yield return new WaitForSeconds(0.2f);

        lineRenderer.enabled = false;
    }

    IEnumerator resetElectricDamageDelayed(Enemy enemy) {
        yield return new WaitForSeconds(0.2f);
        hasTakenElectricDamage = false;
        enemy.hasTakenElectricDamage = false;
    }
}
