using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioData[] _audios;
    Dictionary<string, AudioClip> _nameToClip = new Dictionary<string, AudioClip>();

    AudioSource _audioSource;

	void Start ()
    {
        foreach (var audio in _audios)
            _nameToClip[audio.name] = audio.clip;

        _audioSource = GetComponent<AudioSource>();
	}	

    public void PlayAudio(string wich)
    {
        _audioSource.PlayOneShot(_nameToClip[wich]);
    }
}
