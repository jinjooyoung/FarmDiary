using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition = new Vector3(0, -3, 0);

    [SerializeField]
    private LayerMask placementLayermask;       // 설치 가능 레이어

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnClicked?.Invoke();        // OnClicked가 null이 아닐 경우에만 Invoke() 메서드를 호출
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(3) || Input.GetMouseButtonDown(4))
        {
            OnExit?.Invoke();       // OnExit가 null이 아닐 때만 Invoke()를 호출
        }
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();       // => 화살표 함수는 메서드의 본문이 하나의 표현식으로 구성될 때 사용하기 좋음

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;                         // 마우스 위치 (스크린 좌표)
        Vector3 worldPos = sceneCamera.ScreenToWorldPoint(mousePos);    // 마우스 위치를 월드 좌표로 변환
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100, placementLayermask);

        if (hit.collider != null)       // 레이캐스트에 충돌체가 잡히면
        {
            lastPosition = hit.point;   // 해당 충돌 위치를 저장
        }

        return lastPosition;            // 마지막으로 저장된 위치 리턴
    }
}
