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
        Target.OnTargetBreak += HandleTargetBreak;
    }

    private void OnDisable()
    {
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
    public void Target_OnBreak()
    {
        PlayRandomSound(_soundsCollectionSO.TargetBreak);
    }
    #endregion

    #region Music
    #endregion

    #region CustomSFXLogic
    private void HandleTargetBreak(Target target)
    {
        Target_OnBreak();
    }
    #endregion
}
