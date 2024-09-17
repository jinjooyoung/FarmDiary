using UnityEngine;

public class StickyCanvas : MonoBehaviour
{
    private RectTransform rectTransform;

    // 캔버스의 크기와 위치를 설정하는 함수
    public void SetCanvasSize(int scale)
    {
        rectTransform = GetComponent<RectTransform>();

        if (SaveData.instance.verticalMode)  // 가로 모드인지 확인
        {
            // 가로 모드에서 캔버스의 크기와 위치를 설정합니다.
            float size2 = (float)Screen.width * (1f - Mathf.Abs(GameManager.instance.mainCam.rect.x)) * 2f / (float)scale;  // 화면 너비와 카메라의 rect 값을 사용하여 가로 크기 결정
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 296f);  // 세로 크기 설정 (고정값)
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size2);  // 가로 크기 설정
            rectTransform.anchoredPosition = new Vector3(0f, 0.25f, 90f);  // 위치 설정
        }
    }
}