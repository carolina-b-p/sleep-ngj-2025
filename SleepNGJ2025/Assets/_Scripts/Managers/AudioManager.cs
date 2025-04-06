using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class AudioManager : MonoBehaviour
{
    
    [SerializeField] private AudioSource _costumerReactionSound;
    [SerializeField] private SfxScriptableObjects _drivingSfx;
    [SerializeField] private AudioClip[] customerSfxArray;
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
        if(!_costumerReactionSound.isPlaying)
            PlayEngineSfx();
    }
    
    private void PlayEngineSfx() 
    {
      if (_carController.acceleration > 0) 
      {
        var randomPitch = Random.Range(0.5f, 0.8f);
        _costumerReactionSound.clip = _drivingSfx.sfxClip;
        _costumerReactionSound.Play(); 
        _costumerReactionSound.pitch = randomPitch;
      }
    }
    
    public void PlayCustomerCheerSfx()
    {
        var randIndex = Random.Range(0, customerSfxArray.Length);
        _costumerReactionSound.clip = customerSfxArray[randIndex];
        _costumerReactionSound.Play();
    }
}
