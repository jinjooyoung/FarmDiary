using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text objectNameText; // 오브젝트 이름을 표시할 UI Text

    void Update()
    {
        DisplayObjectUnderMouse();
    }

    private void DisplayObjectUnderMouse()
    {
        // 마우스 위치에서 레이캐스트 수행
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // 레이캐스트에 맞은 오브젝트가 있을 경우
        if (hit.collider != null)
        {
            // 히트된 오브젝트의 이름을 UI 텍스트에 표시
            objectNameText.text = "Mouse Over: " + hit.collider.gameObject.name;
        }
        else
        {
            // 오브젝트가 없을 경우 텍스트를 초기화
            objectNameText.text = "Mouse Over: None";
        }
    }
}
