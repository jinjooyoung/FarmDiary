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

    public void WaterCrop()
    {
        // 씨앗이 심어져 있고 물이 필요한 상태일 때만 물을 줌
        if (farmField.IsSeedPlanted(new Vector2(0, 0)) && farmField.NeedsWater(new Vector2(0, 0)))
        {
            if (currentWaterAmount > 0)
            {
                farmField.WaterCrop(new Vector2(0, 0));  // 밭의 특정 위치에 물을 줌
                currentWaterAmount--;  // 물 사용
                Debug.Log($"물을 줬습니다. 남은 물: {currentWaterAmount}");
            }
            else if (farmField.IsSeedPlanted(new Vector2(0, 0)) && farmField.cropState == FarmField.CropState.Watered)
            {
                //currentState = State.Idle;
            }
            else
            {
                Debug.Log("물을 주기엔 물이 부족합니다.");
            }
        }
        else
        {
            Debug.Log("물을 줄 수 없습니다: 씨앗이 심어져 있지 않거나 물이 필요하지 않습니다.");
        }
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
}
