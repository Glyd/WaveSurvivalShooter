using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject healthPanel;
    public GameObject wavesPanel;
    public GameObject gameOverPanel;
    public Text healthText;

    public Text waveStatsText;
    public Text killsStatsText;
    public Text damageDealtStatsText;
    public Text damageTakenStatsText;

    public StatTracker statTracker;

    private GameObject _playerObject;

    // Start is called before the first frame update
    void Start()
    {
        _playerObject = GameObject.FindWithTag("Player");
        healthPanel.SetActive(true);
        wavesPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void setHealthText(int health) {
        healthText.text = "Health: " + health; 
    }

    public void startGameOver() {
        Destroy(_playerObject);
        WeaponUpgrades.removeAllUpgrades();

        Dictionary<string,int> stats = statTracker.getStats();

        int wave, kills, damageDealt, damageTaken;

        stats.TryGetValue("wave", out wave);
        stats.TryGetValue("kills", out kills);
        stats.TryGetValue("damageDealt", out damageDealt);
        stats.TryGetValue("damageTaken", out damageTaken);

        waveStatsText.text = "Reached Wave " + wave;
        killsStatsText.text = "Kills: " + kills;
        damageDealtStatsText.text = "Damage dealt: " + damageDealt;
        damageTakenStatsText.text = "Damage taken: " + damageTaken;


        healthPanel.SetActive(false);
        wavesPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
