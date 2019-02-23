using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{

    AudioSource _audioSource;

    [SerializeField] AudioData[] _themes;
    Dictionary<string, AudioClip> _nameToClip = new Dictionary<string, AudioClip>();

    [SerializeField] float _transitionSpeed = 1;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        foreach (var theme in _themes)
            _nameToClip[theme.name] = theme.clip;

        var necro = FindObjectOfType<Necromancer>();
        necro.OnBossDeath += () => PlayTheme("Main");
        necro.OnBattleStarted += () => PlayTheme("Battle");

        var gem = FindObjectOfType<Gem>();
        gem.OnPowerPicked += () => PlayTheme("Battle");
        gem.OnPowerUsed += () => PlayTheme("Main");

        PlayTheme("Main");
    }

    public void PlayTheme(string wich)
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.loop = true;
            _audioSource.PlayOneShot(_nameToClip[wich]);
            return;
        }

        StartCoroutine(ThemeTransition(wich));
    }

    IEnumerator ThemeTransition(string theme)
    {
        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= Time.deltaTime * _transitionSpeed;
            yield return null;
        }

        _audioSource.Stop();
        _audioSource.PlayOneShot(_nameToClip[theme]);

        while (_audioSource.volume < 1)
        {
            _audioSource.volume += Time.deltaTime * _transitionSpeed;
            yield return null;
        }        
    }
}
