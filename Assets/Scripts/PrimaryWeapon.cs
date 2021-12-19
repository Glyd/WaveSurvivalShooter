using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryWeapon : MonoBehaviour
{
    public GameObject projectile;

    private float _baseShotCooldown = 0.5f;
    private float _shotCooldown;
    private float _nextShotTimer = 0f;
    private float _timer = 0f;
    private int _baseDamage = 25;
    private int _damage;
    private PlayerController _player;
    private Vector3 _rotation;

    private int _projectileSpeedUpgradeCount;
    private int _projectileRangeUpgradeCount;
    private int _projectileDeflectionsUpgradeCount; 

    void Start() {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _shotCooldown = _baseShotCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        transform.right = mousePos - transform.position;

        if (Input.GetKey(KeyCode.Mouse0) && _timer > _nextShotTimer) {
            _rotation = transform.rotation.eulerAngles;

            if (WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_WEAPON_PENTAD) > 0)
                firePentadProjectile();
            else if (WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_WEAPON_TRIAD) > 0)
                fireTriadProjectile();
            else fireProjectile();


            _nextShotTimer = _timer + _shotCooldown;
        }
    }

    private void fireProjectile() {
        createProjectile();
    }

    private void fireTriadProjectile() {
        createProjectile(new Vector3(_rotation.x, _rotation.y, _rotation.z - 30));
        createProjectile();
        createProjectile(new Vector3(_rotation.x, _rotation.y, _rotation.z + 30));
    }

    private void firePentadProjectile() {
        createProjectile(new Vector3(_rotation.x, _rotation.y, _rotation.z - 60));
        createProjectile(new Vector3(_rotation.x, _rotation.y, _rotation.z - 30));
        createProjectile();
        createProjectile(new Vector3(_rotation.x, _rotation.y, _rotation.z + 30));
        createProjectile(new Vector3(_rotation.x, _rotation.y, _rotation.z + 60));
    }

    private GameObject createProjectile() {
        GameObject createdProjectile = Instantiate(projectile, transform.position, transform.rotation);

        createdProjectile.gameObject.GetComponent<Projectile>().setupProjectile(_damage, _projectileSpeedUpgradeCount, _projectileRangeUpgradeCount, _projectileDeflectionsUpgradeCount);

        return createdProjectile;
    }

    private GameObject createProjectile(Vector3 customRotation) {
        GameObject createdProjectile = Instantiate(projectile, transform.position, transform.rotation);

        createdProjectile.gameObject.GetComponent<Projectile>().setupProjectile(_damage, _projectileSpeedUpgradeCount, _projectileRangeUpgradeCount, _projectileDeflectionsUpgradeCount);
        createdProjectile.transform.eulerAngles = customRotation;

        return createdProjectile;
    }

    public void checkUpgrades() {
        _shotCooldown = _shotCooldown > 0.05f ? _baseShotCooldown - 0.05f * WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_WEAPON_FIRE_RATE) : 0.05f;
        _damage = _baseDamage + 10 * WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_PROJECTILE_DAMAGE);

        _projectileDeflectionsUpgradeCount = WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_WEAPON_BOUNCY);
        _projectileRangeUpgradeCount =  WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_PROJECTILE_RANGE);
        _projectileSpeedUpgradeCount =  WeaponUpgrades.getUpgradeCountByName(STRINGS.UPGRADE_PROJECTILE_SPEED);
    }
}
