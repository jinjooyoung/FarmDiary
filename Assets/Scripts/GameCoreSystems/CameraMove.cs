using System;
using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera cam;
    
    private int scale = 2;  // 카메라의 스케일을 결정하는 변수
    
    private float[] zoomLevels = { 3f, 4f, 5f, 10f };  // 카메라 줌 단계
    private int currentZoomLevel;  // 현재 줌 단계 인덱스
    
    private Vector2 mouseClickPos;  // 마우스 클릭 시의 위치를 저장할 변수
    private Vector2 mouseCurrentPos;  // 현재 마우스 위치를 저장할 변수
    private Vector2 initialPanelSize;  // 패널의 초기 크기 저장

    public RectTransform uiPanel;

    public void Start()
    {
        cam = GetComponent<Camera>();
        OrthographicSize();
        ZoomInOutUIPanel();
    }

    void Update()
    {
        CursorClick();  // 카메라 이동 함수 호출
        ZoomInOut();  // 줌 인/아웃 처리
    }
    
    // 카메라 이동 및 드래그 기능을 처리하는 함수
    public void CursorClick()
    {
        // 마우스 오른쪽 버튼(1) 또는 휠 버튼(2)이 클릭되면 드래그 시작
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            SetInitialDragAnchor();  // 드래그의 시작 위치를 설정합니다.
        }
        
        // 마우스 오른쪽 버튼(1) 또는 휠 버튼(2)이 눌리고 있을 때 드래그 처리
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            DragCamera();  
        }
    }
    
    // 드래그의 시작 위치를 설정하는 함수
    private void SetInitialDragAnchor()
    {
        mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // 현재 마우스 위치를 월드 좌표로 변환하여 저장
    }

    // 카메라를 드래그하여 이동시키는 함수
    private void DragCamera()
    {
        mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // 현재 마우스 위치를 월드 좌표로 변환
        Vector2 vector = mouseCurrentPos - mouseClickPos;  // 드래그 거리 계산
    
        // 세로 모드인지 여부에 따라 이동 방향 결정
        if (SaveData.instance.verticalMode)
        {
            base.transform.position -= new Vector3(0f, vector.y, 0f);  // 카메라의 Y 좌표 이동
        }
        else
        {
            base.transform.position -= new Vector3(vector.x, 0f, 0f);  // 카메라의 X 좌표 이동
        }
    
        ClampCamera();  // 카메라의 위치를 제한하는 함수 호출
    }
    
    // 카메라의 위치를 제한하는 함수
    private void ClampCamera()
    {
        if (SaveData.instance.verticalMode)
        {
            // 카메라 위치를 X축 기준으로 제한
            base.transform.position = new Vector3(Mathf.Clamp(base.transform.position.x, -81f, 81f), base.transform.position.y, base.transform.position.z);
        }
    }

    void OrthographicSize()
    {
        // 카메라의 현재 orthographicSize에 맞춰 초기 줌 레벨 설정
        float currentSize = cam.orthographicSize;
        for (int i = 0; i < zoomLevels.Length; i++)
        {
            if (Mathf.Approximately(currentSize, zoomLevels[i]))
            {
                currentZoomLevel = i;
                break;
            }
        }
    }
    
    // 줌 인/아웃을 처리하는 함수
    private void ZoomInOut()
    {
        // N 키를 눌러 줌 인 (다음 줌 단계로 증가)
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentZoomLevel = Mathf.Min(currentZoomLevel + 1, zoomLevels.Length - 1);  // 다음 줌 단계로 이동
            cam.orthographicSize = zoomLevels[currentZoomLevel];  // 해당 줌 레벨 값으로 카메라 크기 설정
            ZoomInOutUIPanel();
        }

        // M 키를 눌러 줌 아웃 (이전 줌 단계로 감소)
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentZoomLevel = Mathf.Max(currentZoomLevel - 1, 0);  // 이전 줌 단계로 이동
            cam.orthographicSize = zoomLevels[currentZoomLevel];  // 해당 줌 레벨 값으로 카메라 크기 설정
            ZoomInOutUIPanel();
        }

        switch (currentZoomLevel)
        {
            case 0: // 줌 레벨 3
                    // 카메라의 Y 좌표를 고정하여 바닥 위치 유지
                float bottomFixedY1 = cam.orthographicSize - 4.837f;  // 바닥 위치를 고정할 Y 값
                base.transform.position = new Vector3(base.transform.position.x, bottomFixedY1, base.transform.position.z);
                break;
            case 1: // 줌 레벨 4
                    // 카메라의 Y 좌표를 고정하여 바닥 위치 유지
                float bottomFixedY2 = cam.orthographicSize - 4.915f;  // 바닥 위치를 고정할 Y 값
                base.transform.position = new Vector3(base.transform.position.x, bottomFixedY2, base.transform.position.z);
                break;
            case 2: // 줌 레벨 5 - 기본 상태로 복귀
                    // 카메라의 Y 좌표를 고정하여 바닥 위치 유지
                float bottomFixedY = cam.orthographicSize - 5f;  // 바닥 위치를 고정할 Y 값
                base.transform.position = new Vector3(base.transform.position.x, bottomFixedY, base.transform.position.z);
                break;
            case 3: // 줌 레벨 10 - 가장 작은 레벨
                    // 카메라의 Y 좌표를 고정하여 바닥 위치 유지
                float bottomFixedY3 = cam.orthographicSize - 5.41f;  // 바닥 위치를 고정할 Y 값
                base.transform.position = new Vector3(base.transform.position.x, bottomFixedY3, base.transform.position.z);
                break;
        }
    }

    // 줌 레벨에 맞게 UI 패널의 위치와 크기를 조정하는 함수
    private void ZoomInOutUIPanel()
    {
        if (uiPanel == null) return;

        // 줌 레벨에 따른 RectTransform 위치와 스케일 조정
        switch (currentZoomLevel)
        {
            case 0: // 줌 레벨 3
                cam.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);
                uiPanel.anchoredPosition = new Vector2(-877f, 217f);
                uiPanel.localScale = Vector3.one * 98.63184f;  // 스케일을 적절히 조정
                break;
            case 1: // 줌 레벨 4
                cam.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);
                uiPanel.anchoredPosition = new Vector2(-651.8f, 91.9f);
                uiPanel.localScale = Vector3.one * 73.46822f;
                break;
            case 2: // 줌 레벨 5 - 기본 상태로 복귀
                cam.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);  // 기본 위치로 설정
                uiPanel.anchoredPosition = new Vector2(-524.4f, 19.2f);  // 기본 위치
                uiPanel.localScale = Vector3.one * 59.0375f;  // 기본 스케일
                break;
            /*case 3: // 줌 레벨 10 - 가장 작은 레벨
                cam.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);  // 기본 위치로 설정
                uiPanel.anchoredPosition = new Vector2(-267.5f, -125f);  // 기본 위치
                uiPanel.localScale = Vector3.one * 30.13809f;  // 기본 스케일
                break;*/
            case 3: // 줌 레벨 10 - 캔버스까지 줄이니까 글씨가 너무 작아짐
                // 이제와서 TMPro로 바꾸고 이것저것 하기엔 시간이 없음
                cam.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);  // 기본 위치로 설정
                uiPanel.anchoredPosition = new Vector2(-524.4f, 19.2f);  // 기본 위치
                uiPanel.localScale = Vector3.one * 59.0375f;  // 기본 스케일
                break;
        }
    }
}