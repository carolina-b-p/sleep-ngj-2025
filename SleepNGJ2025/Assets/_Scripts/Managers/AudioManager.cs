using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;


public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioClip[]_customerCheers;
    [SerializeField] private AudioSource _customerSfxSource;
    
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;

    [SerializeField] private SfxScriptableObjects _drivingSfx;


    public static AudioManager Instance {get; private set;}

    [SerializeField] private CarController _carController;

    void Awake()
    {
        if (Instance != null && Instance == this)
            Destroy(this);
        else 
            Instance = this;
    }

    public void PlayCustomerSfx()
    {
      var customerIndex = Random.Range(0, _customerCheers.Count());
      _customerSfxSource.clip = _customerCheers[customerIndex];
      _customerSfxSource.Play();
    }

    // void Update()
    // {
    //     if(!_sfxSource.isPlaying)
    //         PlayEngineSfx();
    // }


    // private void PlayEngineSfx() 
    // {
    //   if (_carController.acceleration > 0) 
    //   {
    //     var randomPitch = Random.Range(0.5f, 0.8f);
    //     _sfxSource.clip = _drivingSfx.sfxClip;
    //     _sfxSource.Play(); 
    //     _sfxSource.pitch = randomPitch;
    //   }
    // }
}
