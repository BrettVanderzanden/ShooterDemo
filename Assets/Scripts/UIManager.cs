using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Does this need to be a singleton?
public class UIManager : Singleton<UIManager>
{
    [SerializeField] float _alertCycleTimer = 4f;

    List<string> _alertMessages;
    TMP_Text _alertText;
    float _alertDeltaTime = 0f;
    int _alertIndex = 0;
    const string ALERT_TEXT = "Alert Text";

    [SerializeField] Image _TNTCooldownImage;
    [SerializeField] public float _TNTCooldownTime = 1f;
    [SerializeField] Image _TNTIconImage;
    [SerializeField] Sprite _IconTNT;
    [SerializeField] Sprite _IconPlunger;
    [SerializeField] Sprite _IconNoTNT;
    private float _TNTtimer = 1f;
    private int _TNTammo = 1;
    private bool _TNTDeployed = false;

    protected override void Awake()
    {
        base.Awake();
        _alertMessages = new List<string>();
        _TNTtimer = _TNTCooldownTime;
    }

    private void OnEnable()
    {
        _TNTIconImage.sprite = _IconTNT;
        
        SceneManager.sceneLoaded += OnLevelLoaded;
        EndPoint.OnExitReached += OnLevelExitReached;
        PlayerHealth.OnDeath += OnPlayerDeath;
        TNT.OnTNTExplode += OnTNTExplode;
        Gun.OnTNTThrow += OnThrowTNT;
        PlayerController.OnAmmoPickup += OnAmmoPickup;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        EndPoint.OnExitReached -= OnLevelExitReached;
        PlayerHealth.OnDeath -= OnPlayerDeath;
        TNT.OnTNTExplode -= OnTNTExplode;
        Gun.OnTNTThrow -= OnThrowTNT;
        PlayerController.OnAmmoPickup -= OnAmmoPickup;
    }

    private void Start()
    {
        _alertText = GameObject.Find(ALERT_TEXT).GetComponent<TMP_Text>();
        ClearAlerts();
    }

    private void Update()
    {
        UpdateTNTCooldown();
        UpdateAlerts();
    }

    private void AddAlert(string alert)
    {
        _alertMessages.Add(alert);
    }

    private void RemoveAlert(string alert)
    {
        _alertMessages.Remove(alert);
        if (_alertMessages.Count == 0)
        {
            ClearAlerts();
        }
    }

    private void ClearAlerts()
    {
        _alertMessages.Clear();
        _alertIndex = 0;
        _alertDeltaTime = 0f;
        ModifyCurrentAlertText("");
    }

    private void ModifyCurrentAlertText(string str)
    {
        _alertText.text = str;
    }

    private void UpdateAlerts()
    {
        // This feels like a coroutine. come back to it.
        if (_alertMessages.Count > 0)
        {
            _alertDeltaTime -= Time.deltaTime;
            if (_alertDeltaTime < 0f)
            {
                _alertDeltaTime = _alertCycleTimer;
                _alertIndex++;
                if (_alertIndex >= _alertMessages.Count)
                {
                    _alertIndex = 0;
                }
                ModifyCurrentAlertText(_alertMessages[_alertIndex]);
            }
        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        _alertText = GameObject.Find(ALERT_TEXT).GetComponent<TMP_Text>();
        ClearAlerts();
    }

    private void OnLevelExitReached()
    {
        AddAlert("Level Complete!");
    }

    private void OnPlayerDeath(PlayerHealth health)
    {
        AddAlert("You Died!");
    }

    private void UpdateTNTCooldown()
    {
        _TNTtimer += Time.deltaTime;
        if (_TNTtimer >= _TNTCooldownTime)
        {
            _TNTtimer = _TNTCooldownTime;
        }

        if (_TNTCooldownImage != null)
        {
            _TNTCooldownImage.fillAmount = _TNTtimer / _TNTCooldownTime;
        }
    }

    private void OnTNTExplode()
    {
        _TNTtimer = 0f;
        if (_TNTammo == 0)
        {
            _TNTIconImage.sprite = _IconNoTNT;
        }
        else
        {
            _TNTIconImage.sprite = _IconTNT;
        }
    }

    private void OnThrowTNT()
    {
        _TNTIconImage.sprite = _IconPlunger;
        _TNTammo = 0;
    }

    private void OnAmmoPickup()
    {
        _TNTammo = 1;
        if (_TNTDeployed == false)
        {
            _TNTIconImage.sprite = _IconTNT;
        }
    }

}
