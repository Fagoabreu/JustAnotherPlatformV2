using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Range(0f,2f)]
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private SoundsCollectionSO _soundsCollectionSO;

    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;

    private AudioSource _currentMusic;

    #region Unity Methods

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        FightMusic();
    }

    private void OnEnable() {
        Gun.OnShoot += Gun_Onshoot;
        Gun.OnGrenadeShoot += Gun_OnGrenadeShoot;
        PlayerController.OnJump += PlayerController_OnJump;
        PlayerController.OnJetpack += PlayerController_OnJetpack;
        Health.OnDeath += Health_OnDeath;
        Health.OnDeath += HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent += DiscoBallMusic;
    }

    private void OnDisable() {
        Gun.OnShoot -= Gun_Onshoot;
        Gun.OnGrenadeShoot -= Gun_OnGrenadeShoot;
        PlayerController.OnJump -= PlayerController_OnJump;
        PlayerController.OnJetpack -= PlayerController_OnJetpack;
        Health.OnDeath -= Health_OnDeath;
        Health.OnDeath -= HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent -= DiscoBallMusic;
    }
    #endregion

    #region Sound Methods
    private void PlayRandomSound(SoundSO[] sounds) {
        if (sounds != null && sounds.Length > 0) {
            SoundSO soundSO = sounds[Random.Range(0,sounds.Length)];
            SoundToPlay(soundSO);
        }
    }

    private void SoundToPlay(SoundSO soundSO) {
        AudioClip clip = soundSO.Clip;
        float pitch = soundSO.Pitch;
        float volume = soundSO.Volume * _masterVolume;
        bool loop = soundSO.Loop;
        AudioMixerGroup audioMixerGroup;

        pitch = RandomizePitch(soundSO, pitch);
        audioMixerGroup = DetermineAudioMixerGroup(soundSO);

        Playsound(clip, pitch, volume, loop, audioMixerGroup);
    }

    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO) {
        AudioMixerGroup audioMixerGroup;
        switch (soundSO.AudioType) {
            case SoundSO.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;
            case SoundSO.AudioTypes.MUSIC:
                audioMixerGroup = _musicMixerGroup;
                break;
            default:
                audioMixerGroup = null;
                break;
        }

        return audioMixerGroup;
    }

    private static float RandomizePitch(SoundSO soundSO, float pitch) {
        if (soundSO.RandomizePitch) {
            float randomPitchModifier = Random.Range(-soundSO.RandomPitchRangeModifier, +soundSO.RandomPitchRangeModifier);
            pitch = pitch + randomPitchModifier;
        }

        return pitch;
    }

    private void Playsound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup) {
        GameObject soundObject = new GameObject("Temp Audio Source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.Play();

        if (!loop) {
            Destroy(audioSource, clip.length);
        }

        DetermineMusic(audioMixerGroup, audioSource);
    }

    private void DetermineMusic(AudioMixerGroup audioMixerGroup, AudioSource audioSource) {
        if (audioMixerGroup == _musicMixerGroup) {
            if (_currentMusic != null) {
                _currentMusic.Stop();
            }

            _currentMusic = audioSource;
        }
    }

    #endregion

    #region SFX
    private void Gun_Onshoot() {
        PlayRandomSound(_soundsCollectionSO.GunShoot);

    }
    private void Gun_OnGrenadeShoot() {
        PlayRandomSound(_soundsCollectionSO.GrenadeShoot);
    }

    private void PlayerController_OnJump() {
        PlayRandomSound(_soundsCollectionSO.Jump);
    }

    private void Health_OnDeath(Health health) {
        PlayRandomSound(_soundsCollectionSO.Splat);
    }

    private void Health_OnDeath() {
        PlayRandomSound(_soundsCollectionSO.Splat);
    }

    private void PlayerController_OnJetpack() {
        PlayRandomSound(_soundsCollectionSO.Jetpack);
    }

    public void Grenade_OnBeep() {
        PlayRandomSound(_soundsCollectionSO.GrenadeBeep);
    }
    public void Grenade_OnExplode() {
        PlayRandomSound(_soundsCollectionSO.GrenadeExplode);
    }

    public void Enemy_OnPlayerHit() {
        PlayRandomSound(_soundsCollectionSO.PlayerHit);
    }

    private void AudioManager_Megakill() {
        PlayRandomSound(_soundsCollectionSO.Megakill);
    }
    #endregion

    #region Music
    private void FightMusic() {
        PlayRandomSound(_soundsCollectionSO.FightMusic);
    }

    private void DiscoBallMusic() {
        PlayRandomSound(_soundsCollectionSO.DiscoPartyMusic);
        float soundLenght = _soundsCollectionSO.DiscoPartyMusic[0].Clip.length;
        Utils.RunAfterDelay(this, soundLenght, FightMusic);
    }
    #endregion

    #region CustomSFXLogic
    private List<Health> _deathList = new();
    private Coroutine _deathCoroutine;

    private void HandleDeath(Health health) {
        bool isEnemy = health.GetComponent<Enemy>();

        if (isEnemy) {
            _deathList.Add(health);
        }

        if(_deathCoroutine==null){
            _deathCoroutine = StartCoroutine(DeathWindowRoutine());
        }
    }

    private IEnumerator DeathWindowRoutine() {
        yield return null;
        int megakiilAmount = 3;

        if (_deathList.Count >= megakiilAmount) {
            AudioManager_Megakill();
        }
        Health_OnDeath();

        _deathList.Clear();
        _deathCoroutine = null;
    }
    #endregion
}
