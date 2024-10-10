using UnityEngine;

public abstract class AIState
{
    protected AIStateMachine aiStateMachine;
    protected AIStateManager aiStateManager;

    // ������
    public AIState(AIStateMachine stateMachine)
    {
        this.aiStateMachine = stateMachine;
        this.aiStateManager = stateMachine.aiStateManager;
    }

    // ���� �޼����
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    // ���� ��ȯ ������ üũ�ϴ� �޼���
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

// IdleState : �÷��̾ ������ �ִ� ����
public class IdleState : AIState
{
    public IdleState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Idle ���·� ����");
    }

    public override void Update()
    {
        // Idle ���¿����� �ൿ (��: ���)
        // ���� ��ȯ ���� üũ
        CheckTransitions();
    }

    public override void Exit()
    {
        Debug.Log("Idle ���� ����");
    }
}

// CheckSeedState : �÷��̾ �翡 ������ �ִ��� Ȯ���ϴ� ����
public class CheckSeedState : AIState
{
    public CheckSeedState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("CheckSeed ���·� ����");
        // ���� Ȯ�� ���� �߰�
    }

    public override void Update()
    {
        // ������ Ȯ���ϴ� �ൿ
        // ���� ��ȯ ���� üũ
        CheckTransitions();
    }

    public override void Exit()
    {
        Debug.Log("CheckSeed ���� ����");
    }
}

// GoToWaterState : �÷��̾ ���� ä�췯 ���� ����
public class GoToWaterState : AIState
{
    public GoToWaterState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("GoToWater ���·� ����");
        // �� �����̷� �̵� ���� ����
        // ��: aiStateManager.MoveToPosition(aiStateManager.waterPosition.position);
    }

    public override void Update()
    {
        // �� �����̿� �����ߴ��� Ȯ��
        // ���� �� �� ���� �� ���� ��ȯ
        if (aiStateManager.MoveToPosition(aiStateManager.waterPosition.position))
        {
            aiStateManager.RefuelWater();
            CheckTransitions();
        }
    }

    public override void Exit()
    {
        Debug.Log("GoToWater ���� ����");
    }
}

// WateringState : �÷��̾ �翡 ���� �ִ� ����
public class WateringState : AIState
{
    public WateringState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Watering ���·� ����");
        // �� �ֱ� ���� ����
        // ��: aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition);
    }

    public override void Update()
    {
        // �翡 �����ߴ��� Ȯ��
        if (aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition))
        {
            aiStateManager.WaterCrop();
            CheckTransitions();
        }
    }

    public override void Exit()
    {
        Debug.Log("Watering ���� ����");
    }
}

// HarvestingState : �÷��̾ �� �ڶ� �۹��� ����ϴ� ����
public class HarvestingState : AIState
{
    public HarvestingState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Harvesting ���·� ����");
        // ��Ȯ ���� ����
        // ��: aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition);
    }

    public override void Update()
    {
        // �翡 �����ߴ��� Ȯ��
        if (aiStateManager.MoveToPosition(aiStateManager.farmField.fieldPosition))
        {
            aiStateManager.HarvestCrop();
            CheckTransitions();
        }
    }

    public override void Exit()
    {
        Debug.Log("Harvesting ���� ����");
    }
}

// GoingHomeState : �÷��̾ ��(â��)�� ���ư��� ����
public class GoingHomeState : AIState
{
    public GoingHomeState(AIStateMachine aiStateMachine) : base(aiStateMachine) { }

    public override void Enter()
    {
        Debug.Log("GoingHome ���·� ����");
        // ������ ���� ���� ����
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("Harvesting ���� ����");
    }
}