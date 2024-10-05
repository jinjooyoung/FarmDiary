using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmFieldClickHandler : MonoBehaviour
{
    private FarmField farmField;  // FarmField 컴포넌트 참조

    private void Start()
    {
        farmField = gameObject.GetComponent<FarmField>();
    }

    private void Update()
    {
        ClickMouse();
    }
    
    private void ClickMouse()  // 마우스 클릭 이벤트
    {
        if (Input.GetMouseButtonDown(0))  // 좌클릭 체크
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                Debug.Log("클릭한 오브젝트: " + hit.transform.gameObject.name);
                // 클릭한 오브젝트가 밭인 경우에만 씨앗 심기
                if (hit.transform.gameObject == gameObject)  // 현재 스크립트가 붙어 있는 오브젝트와 비교
                {
                    farmField.PlantSeed(farmField.fieldPosition);  // 현재 밭의 위치에 씨앗을 심음
                    Debug.Log("씨앗을 심었습니다: " + farmField.fieldPosition);
                }
                else
                {
                    Debug.Log("밭을 클릭하지 않았습니다.");
                }
            }
        }
    }
}