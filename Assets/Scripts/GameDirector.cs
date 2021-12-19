using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public EnemySpawner topLeftSpawnPoint;
    public EnemySpawner topRightSpawnPoint;
    public EnemySpawner bottomLeftSpawnPoint;
    public GameObject upgradePanel;
    public Text enemiesRemainingText;
    public Text nextLevelCountdownText;
    public Text upgradeAnnouncementText;
    public Text upgradeItemText;
    public Text upgradeItemDescription;
    public Text waveNumberText;

    public PlayerController player;
    public PrimaryWeapon primaryWeapon;

    private int _currentLevel;
    private int _enemyBudget;
    private int maxSimultaneousSpawns = 1;
    private float _nextLevelTimer;
    private float _timeBetweenLevels = 5.0f;
    private float _defaultSpawnCooldown = 3.0f;
    private float _enemySpawnCooldown;
    private float _minimumEnemySpawnTime = 0.45f;
    private float _currentLevelTimer;
    private float _nextSpawnTime;
    private int _enemiesKilled;
    private int _initialEnemyBudget;
    private int _lastSpawnPoint;
    private bool _hasRerolledSpawn = false;
    private bool _hasPreparedNextLevel = false;
    private bool _levelInProgress;

    private string STRING_ENEMIES_REMAINING = "Enemies remaining: ";
    private string STRING_NEXT_LEVEL_COUNTDOWN = "Next wave in: ";
    private string STRING_WAVE_TEXT = "Wave ";

    // Start is called before the first frame update
    void Start()
    {
        _currentLevel = 1;
        _enemySpawnCooldown = _defaultSpawnCooldown;
        setupNextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        _currentLevelTimer += Time.deltaTime;

        if (_enemyBudget <= 0 && _enemiesKilled >= _initialEnemyBudget && !_hasPreparedNextLevel) {
            _currentLevel += 1;
            _levelInProgress = false;
            setupNextLevel();
        }

        if (_hasPreparedNextLevel) {
            // countdown text, timer etc
            _nextLevelTimer -= Time.deltaTime;
            handleLevelCountdown();
        }

        if (_levelInProgress && _enemyBudget > 0 && _currentLevelTimer > _nextSpawnTime) {
            spawnEnemy();
        }
    }

    private void setupNextLevel() {
        _enemyBudget = _currentLevel * 3;
        _initialEnemyBudget = _enemyBudget;


        _enemySpawnCooldown = _enemySpawnCooldown - 0.15f > _minimumEnemySpawnTime ? _defaultSpawnCooldown - (0.15f * _currentLevel) : _minimumEnemySpawnTime;

        // spawn powerup 
        if (_currentLevel != 1) {
            Upgrade upgradeRecieved = WeaponUpgrades.giveRandomUpgrade();
            upgradeItemText.text = upgradeRecieved.upgradeAnnounceText.Replace("{0}", WeaponUpgrades.getUpgradeCountByName(upgradeRecieved.upgradeName).ToString());
            upgradeItemDescription.text = upgradeRecieved.upgradeAnnounceDescription;
            upgradePanel.SetActive(true);
        }

        player.checkUpgrades();
        primaryWeapon.checkUpgrades();

        // Increase simultaneous spawns every x levels to maintain game pacing with increased enemy numbers
        if (_currentLevel % 7 == 0) {
            maxSimultaneousSpawns++;
        }

        Debug.Log("Max spawns: " + maxSimultaneousSpawns);
        Debug.Log("Spawn cooldown: " + _enemySpawnCooldown);

        // countdown
        _nextLevelTimer = _timeBetweenLevels;
        nextLevelCountdownText.enabled = true;

        // reset any vars 
        _enemiesKilled = 0;
        enemiesRemainingText.text = STRING_ENEMIES_REMAINING + _initialEnemyBudget;

        waveNumberText.text = STRING_WAVE_TEXT + _currentLevel;

        _hasPreparedNextLevel = true;
    }

    private void spawnEnemy() {
        int spawnPoint = Random.Range(0, 3);

        if (spawnPoint == _lastSpawnPoint && !_hasRerolledSpawn) {
            spawnPoint = Random.Range(0, 3);
            _hasRerolledSpawn = true;
        }

        int numberToSpawn = maxSimultaneousSpawns;

        if (numberToSpawn > _enemyBudget) numberToSpawn = _enemyBudget;

        switch (spawnPoint) {
            case 0:
                topLeftSpawnPoint.spawnMeleeEnemies(numberToSpawn, _currentLevel);
                break;
            case 1:
                topRightSpawnPoint.spawnMeleeEnemies(numberToSpawn, _currentLevel);
                break;
            case 2:
                bottomLeftSpawnPoint.spawnMeleeEnemies(numberToSpawn, _currentLevel);
                break;
        }

        _enemyBudget -= numberToSpawn;
        _nextSpawnTime = _currentLevelTimer + _enemySpawnCooldown;
        _hasRerolledSpawn = false;
    }


    // Used by enemies to notify game director of their death.
    public void reportDeath() {
        _enemiesKilled += 1;
        enemiesRemainingText.text = STRING_ENEMIES_REMAINING + (_initialEnemyBudget - _enemiesKilled);
    }

    private void handleLevelCountdown() {
        nextLevelCountdownText.text = STRING_NEXT_LEVEL_COUNTDOWN + _nextLevelTimer.ToString("0.0");

        if (_nextLevelTimer <= 0) {
            _levelInProgress = true;
            _hasPreparedNextLevel = false;
            nextLevelCountdownText.enabled = false;
            upgradePanel.SetActive(false);
        }
    }

    public int getCurrentLevel() {
        return _currentLevel;
    }
}
