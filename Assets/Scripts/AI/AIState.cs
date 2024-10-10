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
        if (!aiStateManager.farmField.IsSeedPlanted(new Vector2(0, 0)))
        {
            aiStateMachine.TransitionToState(new CheckSeedState(aiStateMachine));
        }
        else if (aiStateManager.farmField.NeedsWater(new Vector2(0, 0)))
        {
            aiStateMachine.TransitionToState(new GoToWaterState(aiStateMachine));
        }
        else if (aiStateManager.farmField.IsReadyToHarvest(new Vector2(0, 0)))
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
        // 씨앗 확인 로직 추가
    }

    public override void Update()
    {
        // 씨앗을 확인하는 행동
        // 상태 전환 조건 체크
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
        // 물 웅덩이로 이동 로직 시작
        // 예: aiStateManager.MoveToPosition(aiStateManager.waterPosition.position);
    }

    public override void Update()
    {
        // 물 웅덩이에 도착했는지 확인
        // 도착 시 물 보충 후 상태 전환
        if (aiStateManager.MoveToPosition(aiStateManager.waterPosition.position))
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
        // 물 주기 로직 시작
        // 예: aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition);
    }

    public override void Update()
    {
        // 밭에 도착했는지 확인
        if (aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition))
        {
            aiStateManager.WaterCrop();
            CheckTransitions();
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
        // 수확 로직 시작
        // 예: aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition);
    }

    public override void Update()
    {
        // 밭에 도착했는지 확인
        if (aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition))
        {
            aiStateManager.HarvestCrop();
            CheckTransitions();
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
        // 집으로 가는 로직 시작
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("Harvesting 상태 종료");
    }
}