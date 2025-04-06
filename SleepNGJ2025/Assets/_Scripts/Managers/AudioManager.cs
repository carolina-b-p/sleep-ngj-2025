using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private AudioSource _customerSfxSource;

    

    public static AudioManager Instance {get; private set;}

    [SerializeField] private CarController _carController;

    void Awake()
    {
        if (Instance != null && Instance == this)
            Destroy(this);
        else 
            Instance = this;
    }

    void Update()
    {
        if(!_sfxSource.isPlaying)
            PlayEngineSfx();
    }


    private void PlayEngineSfx() 
    {
      if (_carController.acceleration > 0) 
      {
        var randomPitch = Random.Range(0.5f, 0.8f);
        _sfxSource.clip = _drivingSfx.sfxClip;
        _sfxSource.Play(); 
        _sfxSource.pitch = randomPitch;
      }
    }
}
