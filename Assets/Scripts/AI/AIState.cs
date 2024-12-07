using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    protected AIStateMachine aiStateMachine;
    protected AIStateManager aiStateManager;

    // 생성자
    public AIState(AIStateMachine stateMachine)
    {
        this.aiStateMachine = stateMachine;
        this.aiStateManager = stateMachine.aiStateManager;
    }

    // 가상 메서드들
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    // 상태 전환 조건을 체크하는 메서드
    protected void CheckTransitions()
    {
        // 유효하지 않은 currentCrop이 있다면 null로 설정하고 다음 상태로 이동
        if (aiStateManager.currentCrop == null || !CropGrowthManager.Instance.crops.Contains(aiStateManager.currentCrop))
        {
            aiStateManager.currentCrop = null;  // 현재 작물 초기화
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
            return;
        }

        foreach (Crop crops in CropGrowthManager.Instance.crops)
        {
            if (crops.IsSeedPlanted())
            {
                aiStateManager.currentCrop = crops; // 현재 작업 중인 씨앗 설정
                if (crops.NeedsWater())
                {
                    aiStateMachine.TransitionToState(new WateringState(aiStateMachine));
                    return;
                }
                else if (crops.IsReadyToHarvest())
                {
                    aiStateMachine.TransitionToState(new HarvestingState(aiStateMachine));
                    return;
                }
            }
        }

        aiStateMachine.TransitionToState(new IdleState(aiStateMachine)); // 모든 상태를 확인한 후 Idle 상태로 전환
    }
}

// IdleState : 플레이어가 정지해 있는 상태
public class IdleState : AIState
{
    public IdleState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Idle 상태로 진입");
    }

    public override void Update()
    {
        CheckTransitions();
    }

    public override void Exit()
    {
        Debug.Log("Idle 상태 종료");
    }
}

// CheckSeedState : 플레이어가 밭에 씨앗이 있는지 확인하는 상태
public class CheckSeedState : AIState
{
    public CheckSeedState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("CheckSeed 상태로 진입");
    }

    public override void Update()
    {
        aiStateManager.CheckSeed();
        CheckTransitions();
    }

    public override void Exit()
    {
        Debug.Log("CheckSeed 상태 종료");
    }
}

// GoToWaterState : 플레이어가 물을 채우러 가는 상태
public class GoToWaterState : AIState
{
    public GoToWaterState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("GoToWater 상태로 진입");
    }

    public override void Update()
    {
        // 물 웅덩이에 도착했는지 확인
        if (aiStateManager.MoveToPosition(aiStateManager.waterPosition))
        {
            aiStateManager.RefuelWater();
            CheckTransitions();
        }
    }

    public override void Exit()
    {
        Debug.Log("GoToWater 상태 종료");
    }
}

// WateringState : 플레이어가 밭에 물을 주는 상태
public class WateringState : AIState
{
    public WateringState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Watering 상태로 진입");
    }

    public override void Update()
    {
        if (aiStateManager.currentWaterAmount == 0)
        {
            aiStateMachine.TransitionToState(new GoToWaterState(aiStateMachine));
            return;
        }

        if (aiStateManager.currentCrop != null && aiStateManager.currentCrop.NeedsWater())
        {
            if (aiStateManager.MoveToPosition(aiStateManager.currentCrop.transform))
            {
                aiStateManager.WaterCrop(); // 물 주기
                Debug.Log("작물에 물을 주었습니다.");

                // 물을 다 준 후에 상태 전환 확인
                if (!aiStateManager.currentCrop.NeedsWater())
                {
                    CheckTransitions();
                }
            }
        }
        else
        {
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("Watering 상태 종료");
    }
}

// HarvestingState : 플레이어가 다 자란 작물을 재배하는 상태
public class HarvestingState : AIState
{
    public HarvestingState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Harvesting 상태로 진입");
    }

    public override void Update()
    {
        // 현재 수확할 준비가 된 작물이 있는지 확인
        if (aiStateManager.currentCrop != null && aiStateManager.currentCrop.IsReadyToHarvest())
        {
            if (aiStateManager.MoveToPosition(aiStateManager.currentCrop.transform))
            {
                Debug.Log("작물에 도착했습니다. 수확을 시작합니다.");
                aiStateManager.HarvestCrop();
                CheckTransitions();
            } 
        }
        else
        {
            aiStateMachine.TransitionToState(new GoingHomeState(aiStateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("Harvesting 상태 종료");
    }
}

// GoingHomeState : 플레이어가 집(창고)로 돌아가는 상태
public class GoingHomeState : AIState
{
    public GoingHomeState(AIStateMachine aiStateMachine) : base(aiStateMachine) { }

    public override void Enter()
    {
        Debug.Log("GoingHome 상태로 진입");
    }

    public override void Update()
    {
        // 집으로 이동
        if (aiStateManager.MoveToPosition(aiStateManager.homePosition))
        {
            Debug.Log("집에 도착했습니다.");
            CheckTransitions();
        }
    }

    public override void Exit()
    {
        Debug.Log("GoingHome 상태 종료");
    }
}