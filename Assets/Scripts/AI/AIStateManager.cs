using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine에 대한 참조

    public FarmField farmField;  // 2x2 밭 필드
    public Transform waterPosition;  // 물 웅덩이 위치
    public Transform homePosition;   // 집의 위치 추가

    public float movementSpeed = 2f;  // AI 이동 속도

    public int maxWaterAmount = 5;    // 물 최대 보유량 추가
    public int currentWaterAmount;    // 현재 물 보유량

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine을 참조
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine 컴포넌트를 찾을 수 없습니다.");
        }

        currentWaterAmount = maxWaterAmount;  // 시작 시 최대 보유량으로 초기화
    }

    public bool MoveToPosition(Vector2 target)
    {
        // 목표 위치로 이동
        transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);

        // 목표 위치에 거의 도착했는지 확인
        return Vector2.Distance(transform.position, target) < 0.1f;
    }

    public void CheckSeed()
    {
        farmField.CheckSeedPlanted(new Vector2(0, 0));  // 씨앗을 확인 하는 메서드
        Debug.Log("씨앗 유무를 확인 합니다");
    }

    public void WaterCrop()
    {
        farmField.WaterCrop(new Vector2(0, 0));  // 밭의 특정 위치에 물을 줌
        currentWaterAmount--;  // 물 사용
        Debug.Log($"물을 줬습니다. 남은 물: {currentWaterAmount}");
    }


    public void HarvestCrop()
    {
        // 작물을 수확하는 코드
        farmField.Harvest(new Vector2(0, 0)); // 수확 메서드 호출
        Debug.Log("작물을 수확했습니다");
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // 물 보충
        Debug.Log("물을 다시 채웠습니다.");
    }

    public void AddField()
    {
        // 땅을 구매 시 그 땅을 몇 초 가량 공사를 하는 상태
    }
}
