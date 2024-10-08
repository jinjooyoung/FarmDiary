using System;
using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera cam;
    
    private int scale = 2;  // 카메라의 스케일을 결정하는 변수
    
    private float[] zoomLevels = { 3f, 4f, 5f };  // 카메라 줌 단계
    private int currentZoomLevel;  // 현재 줌 단계 인덱스
    
    [SerializeField] private RectTransform optionsPanelRect;  // 패널의 RectTransform 참조
    [SerializeField] private StickyCanvas stickyCanvas;  // 붙어있는 캔버스를 참조할 변수
    
    private Vector2 mouseClickPos;  // 마우스 클릭 시의 위치를 저장할 변수
    private Vector2 mouseCurrentPos;  // 현재 마우스 위치를 저장할 변수
    private Vector2 initialPanelSize;  // 패널의 초기 크기 저장

    public void Start()
    {
        cam = GetComponent<Camera>();
        OrthographicSize();
        UpdatePanelSize(); // 게임 시작 시 패널 크기 업데이트
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
            stickyCanvas.transform.position -= new Vector3(0f, vector.y, 0f);  // 캔버스의 Y 좌표 이동
        }
        else
        {
            base.transform.position -= new Vector3(vector.x, 0f, 0f);  // 카메라의 X 좌표 이동
            stickyCanvas.transform.position -= new Vector3(-vector.x, 0f, 0f);  // 캔버스의 X 좌표 이동
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
            stickyCanvas.transform.position = new Vector3(Mathf.Clamp(stickyCanvas.transform.position.x, -81f, 81f), 
                stickyCanvas.transform.position.y, stickyCanvas.transform.position.z);
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

        // 패널의 초기 크기 저장
        //initialPanelSize = optionsPanelRect.sizeDelta;
        // 오류 메세지 떠서 잠시 주석처리 해놓을게요!!!
    }
    
    // 줌 인/아웃을 처리하는 함수
    private void ZoomInOut()
    {
        // N 키를 눌러 줌 인 (다음 줌 단계로 증가)
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentZoomLevel = Mathf.Min(currentZoomLevel + 1, zoomLevels.Length - 1);  // 다음 줌 단계로 이동
            cam.orthographicSize = zoomLevels[currentZoomLevel];  // 해당 줌 레벨 값으로 카메라 크기 설정
            UpdatePanelSize();  // 패널 크기 업데이트
        }

        // M 키를 눌러 줌 아웃 (이전 줌 단계로 감소)
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentZoomLevel = Mathf.Max(currentZoomLevel - 1, 0);  // 이전 줌 단계로 이동
            cam.orthographicSize = zoomLevels[currentZoomLevel];  // 해당 줌 레벨 값으로 카메라 크기 설정
            UpdatePanelSize();  // 패널 크기 업데이트
        }

        // 카메라의 Y 좌표를 고정하여 바닥 위치 유지
        float bottomFixedY = cam.orthographicSize - 5f;  // 바닥 위치를 고정할 Y 값
        base.transform.position = new Vector3(base.transform.position.x, bottomFixedY, base.transform.position.z);
        stickyCanvas.transform.position = new Vector3(stickyCanvas.transform.position.x, bottomFixedY, stickyCanvas.transform.position.z);
    }

    // 패널 크기를 카메라 줌에 맞춰 조정하는 함수
    private void UpdatePanelSize()
    {
        float zoomFactor = zoomLevels[zoomLevels.Length - 1] / cam.orthographicSize;
        float clampedZoomFactor = Mathf.Clamp(zoomFactor, 1f, 2f);  // 패널 크기를 1배에서 2배로 제한
        optionsPanelRect.sizeDelta = initialPanelSize * clampedZoomFactor;
    }
}