using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{
    [SerializeField] private float _discoBallPartyTime = 2f;
    [SerializeField] private float _discoGlobalLightIntensity = .2f;
    [SerializeField] private Light2D _globalLight;

    private float _defaultGlobalLightIntensity;
    public static Action OnDiscoBallHitEvent;
    private ColorSpotlight[] _allSpotlights;
    private Coroutine _discoCoroutine;

    private void Awake() {
        _defaultGlobalLightIntensity = _globalLight.intensity;
    }
    private void Start() {
        _allSpotlights = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }

    private void OnEnable() {
        OnDiscoBallHitEvent += DimTheLights;
    }

    private void OnDisable() {
        OnDiscoBallHitEvent -= DimTheLights;
    }

    public void DiscoBallParty() {
        if (_discoCoroutine != null) {
            return;
        }

        OnDiscoBallHitEvent?.Invoke();
    }

    private void DimTheLights() {
        foreach (ColorSpotlight colorSpotlight in _allSpotlights)
        {
            StartCoroutine(colorSpotlight.SpotlightDiscoParty(_discoBallPartyTime));
        }
        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    private IEnumerator GlobalLightResetRoutine() {
        _globalLight.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoBallPartyTime);
        _globalLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null;
    }
    
}
