using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    // 타워 컴포넌트
    [SerializeField] private SpriteRenderer _towerPlace;
    [SerializeField] private SpriteRenderer _towerHead;

    // 타워 속성
    [SerializeField] private int _shootPower = 1;
    [SerializeField] private float _shootDistance = 1f;
    [SerializeField] private float _shootDelay = 5f;
    [SerializeField] private float _bulletSpeed = 1f;
    [SerializeField] private float _bulletSplashRadius = 0f;

    [SerializeField] private Bullet _bulletPrefab;
    
    private float _runningShootDelay;
    private Enemy _targetEnemy;
    private Quaternion _targetRotation;

    public Vector2? PlacePosition { get; private set; }


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 타워 헤드의 스프라이트를 가져오는 함수
    public Sprite GetTowerHeadIcon ()
    {
        return _towerHead.sprite;
    }

    public void SetPlacePosition(Vector2? newPosition)
    {
        PlacePosition = newPosition;
    }

    public void LockPlacement ()
    {
        transform.position = (Vector2) PlacePosition;
    }

    // 드래그 중인 타워의 레이어 순서를 변경
    public void ToggleOrderInLayer (bool toFront)
    {
        int orderInLayer = toFront ? 2 : 0;
        _towerPlace.sortingOrder = orderInLayer;
        _towerHead.sortingOrder = orderInLayer;
    }

    // 가장 가까운 적을 찾음
    public void CheckNearestEnemy (List<Enemy> enemies)
    {
        if (_targetEnemy != null)
        {
            if (!_targetEnemy.gameObject.activeSelf || Vector3.Distance (transform.position, _targetEnemy.transform.position) > _shootDistance)
            {
                _targetEnemy = null;
            }
            else
            {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;
        
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance (transform.position, enemy.transform.position);
            if (distance > _shootDistance)
            {
                continue;
            }
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        _targetEnemy = nearestEnemy;
    }

    // 저장된 타겟 적을 공격
    public void ShootTarget ()
    {
        if (_targetEnemy == null)
        {
            return;
        }

        _runningShootDelay -= Time.unscaledDeltaTime;
        if (_runningShootDelay <= 0f)
        {
            bool headHasAimed = Mathf.Abs (_towerHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) < 10f;
            if (!headHasAimed)
            {
                return;
            }
            Bullet bullet = LevelManager.Instance.GetBulletFromPool (_bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.SetProperties (_shootPower, _bulletSpeed, _bulletSplashRadius);
            bullet.SetTargetEnemy (_targetEnemy);
            bullet.gameObject.SetActive (true);
            _runningShootDelay = _shootDelay;
        }
    }

    // 타워가 항상 적을 바라보게 함
    public void SeekTarget ()
    {
        if (_targetEnemy == null)
        {
            return;
        }
        Vector3 direction = _targetEnemy.transform.position - transform.position;
        float targetAngle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler (new Vector3 (0f, 0f, targetAngle - 90f));
        _towerHead.transform.rotation = Quaternion.RotateTowards (_towerHead.transform.rotation, _targetRotation, Time.deltaTime * 180f);
    }
}
