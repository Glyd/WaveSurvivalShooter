using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text deathScreenText;
    public Text healthText;

    private GameObject _playerObject;
    private PlayerController _playerController;


    // Start is called before the first frame update
    void Start()
    {
        _playerObject = GameObject.FindWithTag("Player");
        _playerController = _playerObject.GetComponent<PlayerController>();
        deathScreenText.enabled = false;
        healthText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        int playerHealth = _playerController.getHealth();

        healthText.text = "Health: " + playerHealth;

        if (playerHealth <= 0) {
            deathScreenText.enabled = true;
            healthText.enabled = false;
            Destroy(_playerObject);
        }
    }
}
