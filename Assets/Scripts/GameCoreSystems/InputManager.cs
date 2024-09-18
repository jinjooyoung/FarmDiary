using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;                         // 마우스 위치 (스크린 좌표)
        Vector3 worldPos = sceneCamera.ScreenToWorldPoint(mousePos);    // 마우스 위치를 월드 좌표로 변환
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100, placementLayermask);

        //Debug.Log("GetSelectedMapPosition 함수 실행");

        if (hit.collider != null)       // 레이캐스트에 충돌체가 잡히면
        {
            lastPosition = hit.point;   // 해당 충돌 위치를 저장
            //Debug.Log("충돌 감지");
        }

        return lastPosition;            // 마지막으로 저장된 위치 리턴
    }
}
