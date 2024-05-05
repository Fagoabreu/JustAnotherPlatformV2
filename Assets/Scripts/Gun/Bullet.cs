using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletVFX;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockBackThrust =20f;

    private Vector2 _fireDirection;
    private Rigidbody2D _rigidBody;
    private Gun _gun;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = _fireDirection * _moveSpeed;
    }

    public void Init(Gun gun, Vector2 bulletSpawnPosition,Vector2 mousePosition ) {
        this._gun = gun;
        transform.position = bulletSpawnPosition;
        _fireDirection = (mousePosition - bulletSpawnPosition).normalized;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Instantiate(_bulletVFX,transform.position,Quaternion.identity);

        IHitable hitable = other.gameObject.GetComponent<IHitable>();
        hitable?.TakeHit();

        IDamageable iDamageable = (IDamageable)(hitable is IDamageable? hitable:null);
        iDamageable?.TakeDamage(_damageAmount, _knockBackThrust);
        _gun.ReleaseBulletFromPool(this);
    }
}