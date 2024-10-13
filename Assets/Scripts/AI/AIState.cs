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
        if (!aiStateManager.farmField.IsSeedPlanted())
        {
            aiStateMachine.TransitionToState(new CheckSeedState(aiStateMachine));
        }
        else if (aiStateManager.farmField.NeedsWater())
        {
            aiStateMachine.TransitionToState(new WateringState(aiStateMachine));
        }
        else if (aiStateManager.farmField.IsReadyToHarvest())
        {
            aiStateMachine.TransitionToState(new HarvestingState(aiStateMachine));
        }
        else
        {
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
        }
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
        // Idle 상태에서의 행동 (예: 대기)
        // 상태 전환 조건 체크
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
        // 물이 부족하면 물을 채우러 감
        if (aiStateManager.currentWaterAmount <= 0)
        {
            Debug.Log("물이 부족합니다. 물을 채우러 이동합니다.");
            aiStateMachine.TransitionToState(new GoToWaterState(aiStateMachine));
            return;
        }

        // 씨앗이 심어져 있고 물이 필요한지 체크
        if (aiStateManager.farmField.IsSeedPlanted() && aiStateManager.farmField.NeedsWater())
        {
            // 밭으로 이동 후 물을 줌
            if (aiStateManager.MoveToPosition(aiStateManager.farmField.transform))
            {
                aiStateManager.WaterCrop(); // 물 주기
                CheckTransitions(); // 물을 준 후 상태 전환 체크
            }
        }
        else
        {
            Debug.Log("씨앗이 없거나 물이 필요하지 않음. Idle 상태로 전환.");
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
        // 밭에 도착했는지 확인
        if (aiStateManager.MoveToPosition(aiStateManager.farmField.transform))
        {
            aiStateManager.HarvestCrop(); // 수확하기
            Debug.Log("작물을 수확했습니다.");
            aiStateMachine.TransitionToState(new GoingHomeState(aiStateMachine)); // 집으로 가는 상태로 전환
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
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine)); // Idle 상태로 전환
        }
    }

    public override void Exit()
    {
        Debug.Log("GoingHome 상태 종료");
    }
}