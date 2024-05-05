using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] private float _paralaxOffset = 0.9f;
    private Vector2 _startPos;
    private Camera _mainCamera;

    private Vector2 _travel => (Vector2) _mainCamera.transform.position - _startPos;

    private void Awake() {
        _mainCamera = Camera.main;
    }

    private void Start() {
        _startPos = transform.position;
    }

    private void FixedUpdate() {
        Vector2 newPosition = _startPos + new Vector2(_travel.x * _paralaxOffset , 0f);
        transform.position = new Vector2 (newPosition.x, transform.position.y);
    }
}
