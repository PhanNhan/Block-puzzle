using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;

    [SerializeField]
    private AudioSource[] _regularSoundAudioSources = null;
    [SerializeField]
    private AudioSource _soundAudioSource = null;
	[SerializeField]
	private AudioSource _musicAudioSource = null;
	[SerializeField]
	private AudioSource _soundLoopAudioSource = null;
    [SerializeField]
    private AudioClipData[] _audioClipData = null;

	// [SerializeField]
	// private AudioClipData[] _musicClipData = null;

    private bool _isEnableSound;
    private bool _isEnableMusic;

	private bool _isPauseSound = false;
	private bool _isPauseLoopSound = false;
	private bool _isPauseMusic = false;

    private string _currentMusicId = null;

    private Dictionary<string, AudioClip> _audioClipCaches = new Dictionary<string, AudioClip>();
    private List<string> _regularAudioIds = new List<string>();
    private List<AudioPlayedTime> _times = new List<AudioPlayedTime>();

    public bool IsEnableSound
    {
        get
        {
            return _isEnableSound;
        }
        set
        {
            _isEnableSound = value;
            callSoundSetter();
            PlayerPrefs.SetInt(C.PlayerPrefKeys.IsEnableSound, _isEnableSound ? 1 : 0);
        }
    }

    public bool IsEnableMusic
    {
        get
        {
            return _isEnableMusic;
        }
        set
        {
            _isEnableMusic = value;
            callMusicSetter();
            PlayerPrefs.SetInt(C.PlayerPrefKeys.IsEnableMusic, _isEnableMusic ? 1 : 0);
        }
    }

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void Ready(List<string> regularAudioIds)
    {
        _regularAudioIds = regularAudioIds;
        IsEnableSound = PlayerPrefs.GetInt(C.PlayerPrefKeys.IsEnableSound, 1) > 0;
        IsEnableMusic = PlayerPrefs.GetInt(C.PlayerPrefKeys.IsEnableMusic, 1) > 0;
    }

    public void PlaySound(string audioId)
    {
        if (!_isEnableSound) return;

        var audioClip = findAudioClipById(audioId);
        if (_regularAudioIds.Contains(audioId))
        {
            playRegularAudio(audioId, audioClip);
        }
        else
        {
            _soundAudioSource.PlayOneShot(audioClip, 1.0f);
        }
    }

	public void PlaySoundLoop(string audioId)
	{
		if (!_isEnableSound) return;

		var audioClip = findAudioClipById(audioId);
		if (_soundLoopAudioSource.clip != audioClip)
		{
			_soundLoopAudioSource.clip = audioClip;
		}
		_soundLoopAudioSource.Play ();
	}

	public void StopSoundLoop()
	{
		_soundLoopAudioSource.Stop ();
	}

	public void PauseAllSound(bool pause)
	{
		if (pause) {
			PauseSound ();
			PauseMusic ();
		} else {
			ResumeSound ();
			ResumeMusic ();
		}
	}

	public void PauseSound()
	{
		if (_soundAudioSource.clip != null) {
			_isPauseSound = true;
			_soundAudioSource.Pause ();
		}

		if (_soundLoopAudioSource.clip != null) {
			_isPauseLoopSound = true;
			_soundLoopAudioSource.Pause ();
		}
	}

	public void ResumeSound()
	{
		if (_isPauseSound && _isEnableSound) {
			_soundAudioSource.Play ();
			_isPauseSound = false;
		}

		if (_isPauseLoopSound && _isEnableSound) {
			_soundLoopAudioSource.Play ();
			_isPauseLoopSound = false;
		}
	}

	public void PauseMusic()
	{
		if (_musicAudioSource.clip != null) {
			_isPauseMusic = true;
			_musicAudioSource.Pause ();
		}
	}

	public void ResumeMusic()
	{
		if (_isPauseMusic && _isEnableMusic) {
			_musicAudioSource.Play ();
			_isPauseMusic = false;
		}
	}

    // public void PlayBackgroundMusic(string audioId)
    // {
    //     _currentMusicId = audioId;
    //     if (!_isEnableMusic) return;

	// 	var audioClip = findMusicClipById(audioId);
    //      if (_musicAudioSource.clip != audioClip)
    //      {
    //          _musicAudioSource.clip = audioClip;
    //      }
    //      _musicAudioSource.Play();
    // }

	public void ResetAudio()
	{
	}

    public void StopBackgroundMusic()
    {
        _currentMusicId = null;
        _musicAudioSource.Stop();
		_musicAudioSource.clip = null;
    }

    private void playRegularAudio(string audioId, AudioClip clip)
    {
        AudioPlayedTime tmp = null;
        for (int i = 0; i < _times.Count; ++i)
        {
            if (_times[i].AudioId == audioId)
            {
                tmp = _times[i];
                break;
            }
        }
        if (tmp == null)
        {
            tmp = new AudioPlayedTime
            {
                AudioId = audioId,
                SimultaneousPlayCount = 0
            };
            _times.Add(tmp);
        }
        StartCoroutine(crPlaySound(tmp));
    }

    private IEnumerator crPlaySound(AudioPlayedTime sound, bool autoScaleVolume = true, float maxVolumeScale = 1f)
    {
        if (sound.SimultaneousPlayCount >= 10)
        {
            yield break;
        }

        sound.SimultaneousPlayCount++;

        float vol = maxVolumeScale;

        // Scale down volume of same sound played subsequently
        if (autoScaleVolume && sound.SimultaneousPlayCount > 0)
        {
            vol = vol * (1f - (sound.SimultaneousPlayCount - 1) / 10f);
            if (vol <= 0) vol = 0.1f;
        }

        var s = findAudioClipById(sound.AudioId);
        _regularSoundAudioSources[0].PlayOneShot(s, vol);

        // Wait til the sound almost finishes playing then reduce play count
        float delay = s.length * 0.4f;

        yield return new WaitForSeconds(delay);

        sound.SimultaneousPlayCount--;
    }

    private void callSoundSetter()
    {
    }

    private void callMusicSetter()
    {
        _musicAudioSource.clip = findAudioClipById(_currentMusicId);
        if (_isEnableMusic)
            _musicAudioSource.Play();
        else
            _musicAudioSource.Stop();
    }

    private AudioClip findAudioClipById(string audioId)
    {
        AudioClip tmp = null;
        if (audioId != null && !_audioClipCaches.TryGetValue(audioId, out tmp))
        {
            for (int i = 0, c = _audioClipData.Length; i < c; ++i)
            {
                if (audioId == _audioClipData[i].AudioId)
                {
                    tmp = _audioClipData[i].AudioClip;
                    break;
                }
            }
            _audioClipCaches.Add(audioId, tmp);
        }

        return tmp;
    }

	// private AudioClip findMusicClipById(string musicId)
	// {
	// 	AudioClip tmp = null;
	// 	if (musicId != null && !_audioClipCaches.TryGetValue(musicId, out tmp))
	// 	{
	// 		for (int i = 0, c = _musicClipData.Length; i < c; ++i)
	// 		{
	// 			if (musicId == _musicClipData[i].AudioId)
	// 			{
	// 				tmp = _musicClipData[i].AudioClip;
	// 				break;
	// 			}
	// 		}
	// 		_audioClipCaches.Add(musicId, tmp);
	// 	}

	// 	return tmp;
	// }

    public void UpdateSoundVolume(float soundValue)
    {
        _soundAudioSource.volume = soundValue;
        if(!IsEnableSound && soundValue > 0.0f)
            IsEnableSound = true;
    }

    public void UpdateMusicVolume(float musicValue)
    {
        _musicAudioSource.volume = musicValue;
        if(!IsEnableMusic && musicValue > 0.0f)
            IsEnableMusic = true;
    }

    [System.Serializable]
    public class AudioClipData
    {
        public AudioClip AudioClip;
        public string AudioId;
    }

    private class AudioPlayedTime
    {
        public string AudioId;
        public int SimultaneousPlayCount;
    }
}
