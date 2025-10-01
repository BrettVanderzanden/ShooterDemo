using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    private int _currentBulletAmmo = 10;
    private int _maxBulletAmmo = 10;
    //private int _currentTntAmmo = 1;
    //private int _maxTntAmmo = 1;
    TMP_Text _ammoText;

    private void Awake()
    {
        _ammoText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateAmmoDisplay();
    }

    private void OnEnable()
    {
        PlayerController.OnAmmoPickup += RefillAmmo;
        Gun.OnShoot += UseAmmo;
    }

    private void OnDisable()
    {
        PlayerController.OnAmmoPickup -= RefillAmmo;
        Gun.OnShoot -= UseAmmo;
    }

    private void UseAmmo()
    {
        _currentBulletAmmo--;
        UpdateAmmoDisplay();
    }

    private void RefillAmmo()
    {
        _currentBulletAmmo = _maxBulletAmmo;
        //_currentTntAmmo = _maxTntAmmo;
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
    {
        string ammoString = _currentBulletAmmo.ToString() + "/" + _maxBulletAmmo.ToString();
        _ammoText.text = ammoString;
    }
}
