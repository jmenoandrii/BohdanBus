using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private AudioClip[] _songs;
    private AudioSource _audioSource; 
    private int _currentSongIndex = 0;

    [SerializeField]
    private AudioSource _audioButton;

    private float _curentVolume;


    private bool _isActive = true;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        PlayNextSong();
    }

    private void PlayNextSong()
    {
        if (_songs.Length == 0) return;

        _audioSource.clip = _songs[_currentSongIndex];
        _audioSource.Play();

        _currentSongIndex++;
        if (_currentSongIndex >= _songs.Length)
        {
            _currentSongIndex = 0;
        }
    }

    public void ForcePlaySound(AudioClip clip)
    {
        _curentVolume = _audioSource.volume;

        _audioSource.volume = 1f;

        _isActive = true;
        _audioSource.clip = clip;
        _audioSource.Stop();
        _audioSource.Play();
    }

    private void Update()
    {
        if (!_audioSource.isPlaying && _isActive)
        {
            _audioSource.volume = _curentVolume;
            PlayNextSong();
        }
    }

    public void ForceTurnOff()
    {
        _isActive = false;

        _audioSource.Pause();
    }

    public void Interact()
    {
        _isActive = !_isActive;
        _audioButton.Play();

        if (!_isActive)
            _audioSource.Pause();
        else
            _audioSource.UnPause();
    }
}
