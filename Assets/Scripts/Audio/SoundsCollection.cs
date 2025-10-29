using UnityEngine;

[CreateAssetMenu()]
public class SoundsCollectionSO : ScriptableObject
{
    [Header("Music")]
    public SoundSO[] MenuMusic;
    public SoundSO[] LevelMusic;
    public SoundSO[] CelebrationMusic;

    [Header("SFX")]
    [Header("Player")]
    public SoundSO[] PlayerMove;
    public SoundSO[] PlayerJump;
    public SoundSO[] PlayerLand;
    public SoundSO[] PlayerDash;
    public SoundSO[] PlayerTakeDamage;
    public SoundSO[] PlayerDeath;
    public SoundSO[] GunShoot;
    public SoundSO[] GunShootEmpty;
    public SoundSO[] TNTThrow;
    public SoundSO[] TNTThrowFail;
    public SoundSO[] TNTExplode;
    public SoundSO[] TNTBoost;
    public SoundSO[] AmmoRefill;

    [Header("Enemies")]
    public SoundSO[] EnemyAlert;
    public SoundSO[] EnemyTakeDamage;
    public SoundSO[] BasherMove;
    public SoundSO[] BasherAttack;
    public SoundSO[] BasherHit;
    public SoundSO[] BasherDeath;
    public SoundSO[] PistolerMove;
    public SoundSO[] PistolerShoot;
    public SoundSO[] PistolerDeath;
    
    [Header("Environment")]
    public SoundSO[] TargetBreak;
    public SoundSO[] LandmineArm;
    public SoundSO[] LandmineExplode;
    public SoundSO[] BulletHitFlesh;
    public SoundSO[] BulletHitSolid;
}
