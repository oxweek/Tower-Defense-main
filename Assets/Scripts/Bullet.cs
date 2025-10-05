using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _bulletPower;
    private float _bulletSpeed;
    private float _bulletSplashRadius;

    private Enemy _targetEnemy;

    // Start는 첫 프레임 전에 호출됩니다
    void Start()
    {
        
    }

    // Update는 매 프레임마다 호출됩니다
    void Update()
    {
        
    }

    // FixedUpdate는 일정한 간격으로 호출됩니다
    // 물리 연산(Rigidbody 등)이 있는 경우에 적합합니다
    private void FixedUpdate ()
    {
        if (LevelManager.Instance.IsOver)
        {
            return;
        }

        if (_targetEnemy != null)
        {
            if (!_targetEnemy.gameObject.activeSelf)
            {
                gameObject.SetActive (false);
                _targetEnemy = null;
                return;
            }
            transform.position = Vector3.MoveTowards (transform.position, _targetEnemy.transform.position, _bulletSpeed * Time.fixedDeltaTime);
            Vector3 direction = _targetEnemy.transform.position - transform.position;
            float targetAngle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, targetAngle - 90f));
        }
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (_targetEnemy == null)
        {
            return;
        }
        
        if (collision.gameObject.Equals (_targetEnemy.gameObject))
        {
            gameObject.SetActive (false);

            // 범위 공격 효과가 있는 총알
            if (_bulletSplashRadius > 0f)
            {
                LevelManager.Instance.ExplodeAt (transform.position, _bulletSplashRadius, _bulletPower);
            }

            // 단일 타겟 총알
            else
            {
                _targetEnemy.ReduceEnemyHealth (_bulletPower);
            }
            _targetEnemy = null;
        }
    }

    // 총알의 속성 설정
    public void SetProperties (int bulletPower, float bulletSpeed, float bulletSplashRadius)
    {
        _bulletPower = bulletPower;
        _bulletSpeed = bulletSpeed;
        _bulletSplashRadius = bulletSplashRadius;
    }

    // 총알의 타겟 적 설정
    public void SetTargetEnemy (Enemy enemy)
    {
        _targetEnemy = enemy;
    }
}
