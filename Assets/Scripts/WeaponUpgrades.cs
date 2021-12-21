using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class WeaponUpgrades
{
    // Name, Rarity, no. acquired
    public static Upgrade[] allUpgrades;
    private static Upgrade[] _commonUpgrades, _uncommonUpgrades, _rareUpgrades;

    static WeaponUpgrades() {
        _commonUpgrades = new Upgrade[] {
             new Upgrade(STRINGS.UPGRADE_PROJECTILE_DAMAGE, STRINGS.UPGRADE_TITLE_PROJECTILE_DAMAGE, STRINGS.UPGRADE_DESCRIPTION_PROJECTILE_DAMAGE),
             new Upgrade(STRINGS.UPGRADE_PROJECTILE_SPEED, STRINGS.UPGRADE_TITLE_PROJECTILE_SPEED, STRINGS.UPGRADE_DESCRIPTION_PROJECTILE_SPEED),
             new Upgrade(STRINGS.UPGRADE_PROJECTILE_RANGE, STRINGS.UPGRADE_TITLE_PROJECTILE_RANGE, STRINGS.UPGRADE_DESCRIPTION_PROJECTILE_RANGE),
             new Upgrade(STRINGS.UPGRADE_WEAPON_FIRE_RATE, STRINGS.UPGRADE_TITLE_WEAPON_FIRE_RATE, STRINGS.UPGRADE_DESCRIPTION_WEAPON_FIRE_RATE),
             new Upgrade(STRINGS.UPGRADE_PLAYER_SPEED, STRINGS.UPGRADE_TITLE_PLAYER_SPEED, STRINGS.UPGRADE_DESCRIPTION_PLAYER_SPEED),
             new Upgrade(STRINGS.UPGRADE_PLAYER_HEALTH, STRINGS.UPGRADE_TITLE_PLAYER_HEALTH, STRINGS.UPGRADE_DESCRIPTION_PLAYER_HEALTH)
        };

        _uncommonUpgrades = new Upgrade[] {
             new Upgrade(STRINGS.UPGRADE_WEAPON_TRIAD, STRINGS.UPGRADE_TITLE_WEAPON_TRIAD, STRINGS.UPGRADE_DESCRIPTION_WEAPON_TRIAD, true, STRINGS.UPGRADE_WEAPON_PENTAD),
             new Upgrade(STRINGS.UPGRADE_WEAPON_ELECTRIC, STRINGS.UPGRADE_TITLE_WEAPON_ELECTRIC, STRINGS.UPGRADE_DESCRIPTION_WEAPON_ELECTRIC, true),
             new Upgrade(STRINGS.UPGRADE_WEAPON_BOUNCY, STRINGS.UPGRADE_TITLE_WEAPON_BOUNCY, STRINGS.UPGRADE_DESCRIPTION_WEAPON_BOUNCY)
        };

        _rareUpgrades = new Upgrade[] {
             new Upgrade(STRINGS.UPGRADE_WEAPON_PENTAD, STRINGS.UPGRADE_TITLE_WEAPON_PENTAD, STRINGS.UPGRADE_DESCRIPTION_WEAPON_PENTAD, true),
             new Upgrade(STRINGS.UPGRADE_PLAYER_SHIELD, STRINGS.UPGRADE_TITLE_PLAYER_SHIELD, STRINGS.UPGRADE_DESCRIPTION_PLAYER_SHIELD)
        };

        allUpgrades = new Upgrade[_commonUpgrades.Length + _uncommonUpgrades.Length + _rareUpgrades.Length];

        for (int i = 0; i < _commonUpgrades.Length; i++) {
            allUpgrades[i] = _commonUpgrades[i];
        }

        for (int i = 0; i < _uncommonUpgrades.Length; i++) {
            allUpgrades[i + _commonUpgrades.Length] = _uncommonUpgrades[i];
        }

        for (int i = 0; i < _rareUpgrades.Length; i++) {
            allUpgrades[i + _commonUpgrades.Length + _uncommonUpgrades.Length] = _rareUpgrades[i];
        }
    }

    public static void addUpgrade(string upgradeName) {
        for(int i = 0; i < allUpgrades.Length; i++) {
            if (allUpgrades[i].upgradeName.Equals(upgradeName)) {
                allUpgrades[i].numberAcquired++;
            }
        }
    }

    public static void removeAllUpgrades() {
        for (int i = 0; i < allUpgrades.Length; i++) {
                allUpgrades[i].numberAcquired = 0;
        }
    }

    public static int getUpgradeCountByName(string upgradeName) {
        int count = 0;

        for (int i = 0; i < allUpgrades.Length; i++) {
            if (allUpgrades[i].upgradeName.Equals(upgradeName)) {
                count = allUpgrades[i].numberAcquired;
            }
        }

        return count;
    }

    public static Upgrade[] retrieveUpgradeList() {
        return allUpgrades;
    }

    public static Upgrade giveRandomUpgrade() {
        int randomValue = Random.Range(0, 100);
        Upgrade upgradeGiven;

        if (randomValue < 10) {
            Upgrade upgrade = _getRandomRareUpgrade();

            Debug.Log("Chose " + upgrade.upgradeAnnounceText);
            Debug.Log("Currently have " + getUpgradeCountByName(upgrade.upgradeName));
            Debug.Log("Does it allow duplicates? " + !upgrade.preventDuplicates);

            while (!isValidUpgrade(upgrade)) {
                upgrade = _getRandomRareUpgrade();
            }

            addUpgrade(upgrade.upgradeName);
            upgradeGiven = upgrade;
        } 
        else if (randomValue < 30) {
            Upgrade upgrade = _getRandomUncommonUpgrade();
            while (!isValidUpgrade(upgrade)) upgrade = _getRandomUncommonUpgrade();
            addUpgrade(upgrade.upgradeName);
            upgradeGiven = upgrade;
        }
        else {
            Upgrade upgrade = _getRandomCommonUpgrade();
            while (!isValidUpgrade(upgrade)) upgrade = _getRandomCommonUpgrade();
            addUpgrade(upgrade.upgradeName);
            upgradeGiven = upgrade;
        }

        Debug.Log("Given player " + upgradeGiven.upgradeAnnounceText);

        return upgradeGiven;
    }

    private static Upgrade _getRandomCommonUpgrade() {
        int random = Random.Range(0, _commonUpgrades.Length);

        return _commonUpgrades[random];
    }

    private static Upgrade _getRandomUncommonUpgrade() {
        int random = Random.Range(0, _uncommonUpgrades.Length);

        return _uncommonUpgrades[random];
    }

    private static Upgrade _getRandomRareUpgrade() {
        int random = Random.Range(0, _rareUpgrades.Length);

        return _rareUpgrades[random];
    }

    private static bool isValidUpgrade(Upgrade upgrade) {
        bool hasDisqualifyingItem = getUpgradeCountByName(upgrade.preventDropIfAcquiredUpgrade) == 0;


        return upgrade.preventDuplicates == false || getUpgradeCountByName(upgrade.upgradeName) == 0;
    }
}
