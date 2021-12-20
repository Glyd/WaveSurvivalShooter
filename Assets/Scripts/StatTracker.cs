using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour
{
    private int _waveNumber;
    private int _kills;
    private int _damageDealt;
    private int _damageTaken;

    public void setWave(int wave) {
        _waveNumber = wave;
    }

    public void addKill() {
        _kills++;
    }

    public void addDamageDealt(int damage) {
        _damageDealt += damage;
    }

    public void addDamageTaken(int damage) {
        _damageTaken += damage;
    }

    public Dictionary<string,int> getStats() {
        Dictionary<string, int> stats = new Dictionary<string, int>();

        stats.Add("wave", _waveNumber);
        stats.Add("kills", _kills);
        stats.Add("damageDealt", _damageDealt);
        stats.Add("damageTaken", _damageTaken);

        return stats;
    }
}
