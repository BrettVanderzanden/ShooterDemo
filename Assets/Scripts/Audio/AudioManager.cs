using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Range(0f, 3f)]
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private SoundsCollectionSO _soundsCollectionSO;

    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;

    private AudioSource _currentMusic;

    #region Unity Methods
    private void OnEnable()
    {
        PlayerController.OnJump += PlayerJump;
        PlayerController.OnDash += PlayerDash;
        PlayerHealth.OnTakeDamage += HandlePlayerTakeDamage;
        PlayerHealth.OnDeath += HandlePlayerDeath;
        Gun.OnShoot += GunShoot;
        Gun.OnShootEmpty += GunShootEmpty;
        Gun.OnTNTThrow += TNTThrow;
        Gun.OnTNTThrowFail += TNTThrowFail;
        TNT.OnTNTExplode += TNTExplode;
        PlayerController.OnAmmoPickup += AmmoRefill;
        Target.OnTargetBreak += HandleTargetBreak;
        Landmine.OnLandmineArmed += LandmineArm;
        Landmine.OnLandmineExplode += LandmineExplode;
        Bullet.OnBulletHitFlesh += BulletHitFlesh;
        Bullet.OnBulletHitSolid += BulletHitSolid;
    }

    private void OnDisable()
    {
        PlayerController.OnJump -= PlayerJump;
        PlayerController.OnDash -= PlayerDash;
        PlayerHealth.OnTakeDamage -= HandlePlayerTakeDamage;
        PlayerHealth.OnDeath -= HandlePlayerDeath;
        Gun.OnShoot -= GunShoot;
        Gun.OnShootEmpty -= GunShootEmpty;
        Gun.OnTNTThrow -= TNTThrow;
        Gun.OnTNTThrowFail -= TNTThrowFail;
        TNT.OnTNTExplode -= TNTExplode;
        PlayerController.OnAmmoPickup -= AmmoRefill;
        Target.OnTargetBreak -= HandleTargetBreak;
        Landmine.OnLandmineArmed -= LandmineArm;
        Landmine.OnLandmineExplode -= LandmineExplode;
        Bullet.OnBulletHitFlesh -= BulletHitFlesh;
        Bullet.OnBulletHitSolid -= BulletHitSolid;
    }

    #endregion

    #region Sound Methods
    private void PlayRandomSound(SoundSO[] sounds)
    {
        if (sounds != null && sounds.Length > 0)
        {
            SoundSO soundSO = sounds[Random.Range(0, sounds.Length)];
            SoundToPlay(soundSO);
        }
    }

    private void SoundToPlay(SoundSO soundSO)
    {
        AudioClip clip = soundSO.Clip;
        float pitch = soundSO.Pitch;
        float volume = soundSO.Volume * _masterVolume;
        bool loop = soundSO.Loop;
        AudioMixerGroup audioMixerGroup;

        pitch = RandomizePitch(soundSO, pitch);
        audioMixerGroup = DetermineAudioMixerGroup(soundSO);

        PlaySound(clip, pitch, volume, loop, audioMixerGroup);
    }

    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO)
    {
        AudioMixerGroup audioMixerGroup;
        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;
            case SoundSO.AudioTypes.Music:
                audioMixerGroup = _musicMixerGroup;
                break;
            default:
                audioMixerGroup = null;
                break;
        }

        return audioMixerGroup;
    }

    private static float RandomizePitch(SoundSO soundSO, float pitch)
    {
        if (soundSO.RandomizePitch)
        {
            float randomPitchModifier = Random.Range(-soundSO.RandomPitchRangeModifier, soundSO.RandomPitchRangeModifier);
            pitch = soundSO.Pitch + randomPitchModifier;
        }

        return pitch;
    }

    private void PlaySound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup)
    {
        GameObject soundObject = new GameObject("Temp Audio Source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.Play();

        if (!loop) { Destroy(soundObject, clip.length); } // object pooling would be nice
    }

    #endregion

    #region SFX
    #region Player
    public void PlayerMove()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerMove);
    }

    public void PlayerJump()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerJump);
    }

    public void PlayerLand()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerLand);
    }

    public void PlayerDash()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerDash);
    }

    public void PlayerTakeDamage()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerTakeDamage);
    }

    public void PlayerDeath()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerDeath);
    }

    public void GunShoot()
    {
        PlayRandomSound(_soundsCollectionSO.GunShoot);
    }

    public void GunShootEmpty()
    {
        PlayRandomSound(_soundsCollectionSO.GunShootEmpty);
    }

    public void TNTThrow()
    {
        PlayRandomSound(_soundsCollectionSO.TNTThrow);
    }

    public void TNTThrowFail()
    {
        PlayRandomSound(_soundsCollectionSO.TNTThrowFail);
    }

    public void TNTExplode()
    {
        PlayRandomSound(_soundsCollectionSO.TNTExplode);
    }

    public void AmmoRefill()
    {
        PlayRandomSound(_soundsCollectionSO.AmmoRefill);
    }
    #endregion // Player
    #region Enemies
    public void EnemyAlert()
    {
        PlayRandomSound(_soundsCollectionSO.EnemyAlert);
    }

    public void EnemyTakeDamage()
    {
        
    }

    public void BasherMove()
    {
        PlayRandomSound(_soundsCollectionSO.BasherMove);
    }

    public void BasherAttack()
    {
        PlayRandomSound(_soundsCollectionSO.BasherAttack);
    }

    public void BasherHit()
    {
        PlayRandomSound(_soundsCollectionSO.BasherHit);
    }

    public void BasherDeath()
    {
        PlayRandomSound(_soundsCollectionSO.BasherDeath);
    }

    public void PistolerMove()
    {
        PlayRandomSound(_soundsCollectionSO.PistolerMove);
    }

    public void PistolerShoot()
    {
        PlayRandomSound(_soundsCollectionSO.PistolerShoot);
    }

    public void PistolerDeath()
    {
        PlayRandomSound(_soundsCollectionSO.PistolerDeath);
    }
    #endregion // Enemies

    #region Environment
    public void Target_OnBreak()
    {
        PlayRandomSound(_soundsCollectionSO.TargetBreak);
    }

    public void LandmineArm()
    {
        PlayRandomSound(_soundsCollectionSO.LandmineArm);
    }

    public void LandmineExplode()
    {
        PlayRandomSound(_soundsCollectionSO.LandmineExplode);
    }

    public void BulletHitFlesh()
    {
        PlayRandomSound(_soundsCollectionSO.BulletHitFlesh);
    }

    public void BulletHitSolid()
    {
        PlayRandomSound(_soundsCollectionSO.BulletHitSolid);
    }
    #endregion // Environment

    #endregion // SFX

    #region Music
    #endregion // Music

    #region CustomSFXLogic
    private void HandleTargetBreak(Target target)
    {
        Target_OnBreak();
    }

    private void HandlePlayerTakeDamage(PlayerHealth health)
    {
        PlayerTakeDamage();
    }

    private void HandlePlayerDeath(PlayerHealth health)
    {
        PlayerDeath();
    }

    #endregion // CustomSFXLogic
}
