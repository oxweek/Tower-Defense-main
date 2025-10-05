﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _towerIcon;
    
    private Tower _towerPrefab;
    private Tower _currentSpawnedTower;

    // Start는 첫 프레임 전에 호출됩니다
    void Start()
    {
        
    }

    // Update는 매 프레임마다 호출됩니다
    void Update()
    {
        
    }

    public void SetTowerPrefab (Tower tower)
    {
        _towerPrefab = tower;
        _towerIcon.sprite = tower.GetTowerHeadIcon ();
    }

    // IBeginDragHandler 인터페이스 구현
    // UI를 드래그하기 시작할 때 한 번 호출됩니다
    public void OnBeginDrag (PointerEventData eventData)
    {
        GameObject newTowerObj = Instantiate (_towerPrefab.gameObject);
        _currentSpawnedTower = newTowerObj.GetComponent<Tower> ();
        _currentSpawnedTower.ToggleOrderInLayer (true);
    }

    // IDragHandler 인터페이스 구현
    // UI를 드래그하는 동안 계속 호출됩니다
    public void OnDrag (PointerEventData eventData)
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint (mousePosition);
        _currentSpawnedTower.transform.position = targetPosition;
    }

    // IEndDragHandler 인터페이스 구현
    // 드래그를 끝냈을 때 한 번 호출됩니다
    public void OnEndDrag (PointerEventData eventData)
    {
        if (_currentSpawnedTower.PlacePosition == null)
        {
            Destroy (_currentSpawnedTower.gameObject);
        }
        else
        {
            _currentSpawnedTower.LockPlacement ();
            _currentSpawnedTower.ToggleOrderInLayer (false);
            LevelManager.Instance.RegisterSpawnedTower (_currentSpawnedTower);
            _currentSpawnedTower = null;
        }
    }
}
