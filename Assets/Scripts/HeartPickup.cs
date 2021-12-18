using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    int _healAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerController>().heal(_healAmount);
            Destroy(gameObject);
        }
    }
}
