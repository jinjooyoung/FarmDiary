using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public enum PotState
    {
        Empty,                  // 1. 아무것도 들어있지 않은 상태
        Selecting,              // 2. 재료 선택 중
        ReadyToStart,           // 3. 제작 시작 대기 상태
        Crafting,               // 4. 제작 중
        Completed               // 5. 제작 완료 상태
    }

    private GameObject panel;   // 포션 제작 패널
    private bool isInitialized = false; // 초기화 여부 플래그
    public int id = -1; // 솥의 고유 ID

    [Header("현재 솥의 상태")]
    public PotState currentState = PotState.Empty; // 현재 상태

    [Header("선택된 작물")]
    public int magicID = -1;    // 선택한 마법 작물 ID
    public List<int> basicMaterial = new List<int>(new int[3]);     // 선택한 일반 작물 ID 배열

    [Header("포션 제작 시간")]
    public float totalCraftingTime; // 포션 제작에 걸리는 총 시간
    [Header("남은 시간")]
    public float remainingTime; // 남은 제작 시간

    // ID를 설정하는 메서드
    public void SetID(int newID)
    {
        id = newID;
    }

    // ID를 해제하는 메서드
    public void ClearID()
    {
        id = -1;
    }

    public int GetID()
    {
        return id;
    }

    private void Update()
    {
        if (currentState == PotState.Crafting)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime; // 시간 감소
            }
            else
            {
                remainingTime = 0; // 남은 시간을 0으로 설정
                ChangeState(PotState.Completed);
            }
        }
    }

    public void ChangeState(PotState newState)
    {
        currentState = newState;
        PotionUIManager.instance.InitializePotionUI();
        Debug.Log($"솥 상태 변경: {newState}");
    }

    public void Initialize()    // 솥 초기화
    {
        isInitialized = true; // 설치 완료 시 호출
        panel = UIManager.instance.PotionPanel;
    }

    // 클릭 이벤트 처리
    private void OnMouseDown()
    {
        if (!isInitialized) return;     // 설치하자마자 패널 열리는 것 방지

        if (PlacementSystem.Instance.IsDeleteModeActive()) return;

        if (PotionManager.instance != null && UIManager.instance != null)
        {
            Debug.Log($"솥 클릭됨. ID: {id}");
            PotionUIManager.instance.currentPotID = id;     // 포션 UI 매니저로 클릭한 솥의 ID를 보냄
            UIManager.instance.TogglePanel(panel);          // 패널을 열음
        }
    }
}
