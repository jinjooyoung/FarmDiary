/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmFieldClickHandler : MonoBehaviour
{
    private FarmField farmField;  // FarmField 컴포넌트 참조

    private void Start()
    {
        // TryGetComponent를 사용하여 안전하게 FarmField 컴포넌트를 가져옴
        if (!gameObject.TryGetComponent<FarmField>(out farmField))
        {
            Debug.LogError("FarmField 컴포넌트를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        HandleMouseClick();
    }

    // 마우스 클릭 이벤트 처리 메서드
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))  // 좌클릭 체크
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.transform.gameObject;
                Debug.Log($"클릭한 오브젝트: {clickedObject.name}");

                // 클릭한 오브젝트가 현재 스크립트가 붙어 있는 밭(FarmField)인지 확인
                if (clickedObject == gameObject)
                {
                    // FarmField가 유효한지 확인 후 씨앗 심기
                    if (farmField != null)
                    {
                        farmField.PlantSeed();  // 현재 밭의 위치에 씨앗을 심음
                        Debug.Log($"씨앗을 심었습니다: {farmField.fieldPosition}");
                    }
                    else
                    {
                        Debug.LogWarning("FarmField가 없습니다.");
                    }
                }
                else
                {
                    Debug.Log("밭을 클릭하지 않았습니다.");
                }
            }
            else
            {
                Debug.Log("아무 것도 클릭되지 않았습니다.");
            }
        }
    }
}
*/