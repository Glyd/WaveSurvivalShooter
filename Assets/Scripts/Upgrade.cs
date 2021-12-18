using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Upgrade 
{
    public string upgradeName;
    public string upgradeAnnounceText;
    public string upgradeAnnounceDescription;
    public int numberAcquired;
    public bool preventDuplicates;
    public string preventDropIfAcquiredUpgrade;


    public Upgrade(string name, string announceText, string announceDescription) {
        this.upgradeName = name;
        this.numberAcquired = 0;
        this.upgradeAnnounceText = announceText;
        this.upgradeAnnounceDescription = announceDescription;
        this.preventDuplicates = false;
        preventDropIfAcquiredUpgrade = "**NONE**";
    }

    public Upgrade(string name, string announceText, string announceDescription, bool preventDuplicates) {
        this.upgradeName = name;
        this.numberAcquired = 0;
        this.upgradeAnnounceText = announceText;
        this.upgradeAnnounceDescription = announceDescription;
        this.preventDuplicates = preventDuplicates;
        preventDropIfAcquiredUpgrade = "**NONE**";
    }

    public Upgrade(string name, string announceText, string announceDescription, bool preventDuplicates, string preventDropIfAcquiredUpgrade) {
        this.upgradeName = name;
        this.numberAcquired = 0;
        this.upgradeAnnounceText = announceText;
        this.upgradeAnnounceDescription = announceDescription;
        this.preventDuplicates = preventDuplicates;
        this.preventDropIfAcquiredUpgrade = preventDropIfAcquiredUpgrade;
    }
}
