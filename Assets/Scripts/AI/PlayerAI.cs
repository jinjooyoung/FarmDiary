using System.Collections;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        CheckSeed,
        Watering,
        Harvesting,
        GoToWater,
        GoingHome
    }
    
    public State currentState;

    public FarmField farmField;  // 2x2 밭 필드

    [SerializeField] private Transform waterPosition;  // 물 웅덩이 위치
    [SerializeField] private Transform homePosition;   // 집의 위치 추가
    [SerializeField] private float movementSpeed = 2f;  // AI 이동 속도
    [SerializeField] private int maxWaterAmount = 5;    // 물 최대 보유량 추가
    [SerializeField] private int currentWaterAmount;    // 현재 물 보유량

    private Vector2 currentTarget;  // 현재 이동 목표 지점

    private void Start()
    {
        currentWaterAmount = maxWaterAmount;  // 시작 시 최대 보유량으로 초기화
        currentState = State.Idle;  // 초기 상태를 Idle로 설정
    }

    private void Update()
    {
        // 상태에 따른 행동을 처리
        HandleState();
    }

    private void HandleState()
    {
        // 물이 없을 경우 자동으로 물을 채우러 가도록 추가
        if (currentWaterAmount <= 0 && currentState != State.GoToWater)
        {
            currentState = State.GoToWater;
            currentTarget = waterPosition.position; // 물을 가지러 이동
        }

        switch (currentState)
        {
            case State.Idle:
                if (farmField.IsSeedPlanted(new Vector2(0, 0)))
                {
                    Debug.Log("작물이 심어졌습니다.");
                    currentState = State.CheckSeed;
                }
                else
                {
                    Debug.Log("작물이 심어져있지 않습니다.");
                }
                break;

            case State.CheckSeed:
                if (farmField.NeedsWater(new Vector2(0, 0)))
                {
                    if (currentWaterAmount > 0)
                    {
                        currentState = State.Watering;
                        currentTarget = farmField.fieldPosition; // 밭으로 이동
                    }
                    else
                    {
                        currentState = State.GoToWater;
                        currentTarget = waterPosition.position; // 물을 가지러 이동
                    }
                }
                else if (farmField.IsReadyToHarvest(new Vector2(0, 0)))
                {
                    currentState = State.Harvesting;
                    currentTarget = farmField.fieldPosition; // 수확할 밭으로 이동
                }
                break;

            case State.Watering:
                // 물을 주고 집으로 돌아감
                if (MoveToPosition(currentTarget))  // 현재 타겟(밭)에 도착했는지 확인
                {
                    WaterCrop();  // 물을 주는 동작
                    currentState = State.GoingHome;  // 집으로 가는 상태로 변경
                }
                break;

            case State.Harvesting:
                // 수확할 밭으로 이동
                if (MoveToPosition(currentTarget))
                {
                    HarvestCrop();
                    currentState = State.GoingHome; // 수확 후 집으로 가기
                }
                break;

            case State.GoToWater:
                // 물 웅덩이로 이동
                if (MoveToPosition(waterPosition.position))
                {
                    // 물을 채우는 로직
                    currentWaterAmount = maxWaterAmount; // 물 최대치로 채움
                    Debug.Log("물을 채웠습니다.");

                    // 밭 상태 확인
                    if (farmField.IsSeedPlanted(new Vector2(0, 0)) && farmField.cropState != FarmField.CropState.Empty)
                    {
                        // 씨앗이 심어져 있고 물이 필요하면 밭으로 이동
                        currentState = State.Watering;
                        currentTarget = farmField.fieldPosition;
                    }
                    else
                    {
                        // 씨앗이 없거나 밭이 비었으면 대기 상태로 전환
                        Debug.Log("밭에 심어져 있는 씨앗이 없거나 밭이 비었습니다.");
                        currentState = State.GoingHome; // 대기 상태로 전환
                    }
                }
                break;

            case State.GoingHome:
                // 집으로 이동
                if (MoveToPosition(homePosition.position))
                {
                    Debug.Log("집에 도착했습니다. 대기 중...");
                    currentState = State.Idle; // 대기 상태로 돌아가기
                }
                break;
        }
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
                currentState = State.Idle;
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
