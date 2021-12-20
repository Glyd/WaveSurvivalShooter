using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;

    public GameObject shield;
    public GameController gameController;
    public StatTracker statTracker;

    private int _basePlayerHealth = 100;
    private int _maxPlayerHealth = 100;
    private int _previousMaxHealth = 100;
    private int _currentPlayerHealth;

    private int _maxShieldAmount = 0;
    private int _currentShieldAmount = 0;

    // movement 
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    private int _movementDirection = 1;
    private bool _isBeingKnockedBack;

    private float _runSpeed;
    private float _baseRunSpeed = 15.0f;

    void Start() {
        body = GetComponent<Rigidbody2D>();

        _currentPlayerHealth = _basePlayerHealth;
        gameController.setHealthText(_currentPlayerHealth);

        _runSpeed = _baseRunSpeed;

        shield.SetActive(false);
    }

    void Update() {

        if (Input.GetKey(KeyCode.A)) {
            _movementDirection = -1;
        }

        if (Input.GetKey(KeyCode.D)) {
            _movementDirection = 1;
        }

        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down
    }

    void FixedUpdate() {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        if (!_isBeingKnockedBack)
            body.velocity = new Vector2(horizontal * _runSpeed, vertical * _runSpeed);
    }

    public void takeDamage(int damageAmount, Collider2D enemyCollider) {
        if (!_isBeingKnockedBack) {
            if (_currentShieldAmount > 0) {
                _currentShieldAmount -= 1;
                StartCoroutine(_knockbackHandling(enemyCollider));
            }
            else {
                shield.SetActive(false);
                _currentPlayerHealth -= damageAmount;
                statTracker.addDamageTaken((int)damageAmount);

                if (_currentPlayerHealth > 0) {
                    gameController.setHealthText(_currentPlayerHealth);
                    StartCoroutine(_knockbackHandling(enemyCollider));
                } else {
                    gameController.startGameOver();
                }
            }
        }
    }

    public int getDirection() {
        return _movementDirection;
    }

    public int getHealth() {
        return _currentPlayerHealth;
    }

    public void heal(int amount) {
        _currentPlayerHealth = (_currentPlayerHealth + amount > _maxPlayerHealth) ? _maxPlayerHealth : _currentPlayerHealth + amount;
        gameController.setHealthText(_currentPlayerHealth);
    }

    private IEnumerator _knockbackHandling(Collider2D enemyCollider) {
        Vector3 moveDirection = body.transform.position - enemyCollider.transform.position;
        body.AddForce(moveDirection.normalized * 200f, ForceMode2D.Impulse);
        _isBeingKnockedBack = true;
        yield return new WaitForSeconds(0.1f);
        body.velocity = Vector3.zero;
        _isBeingKnockedBack = false;
    }

    public void checkUpgrades() {
        _maxPlayerHealth = _basePlayerHealth + 20 * WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_PLAYER_HEALTH);
        if (_maxPlayerHealth > _previousMaxHealth) {
            heal(20);
        }
        _previousMaxHealth = _maxPlayerHealth;
        gameController.setHealthText(_currentPlayerHealth);

        _runSpeed = _baseRunSpeed + 7 * WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_PLAYER_SPEED);

        _maxShieldAmount = 3 * WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_PLAYER_SHIELD);
        _currentShieldAmount = _maxShieldAmount;
        if (_currentShieldAmount > 0) shield.SetActive(true);
    }
}
