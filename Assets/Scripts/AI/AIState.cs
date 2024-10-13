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
        foreach (FarmField field in aiStateManager.farmFields)
        {
            if (field.IsSeedPlanted())
            {
                aiStateManager.currentFarmField = field; // ���� �۾� ���� �� ����
                if (field.NeedsWater())
                {
                    aiStateMachine.TransitionToState(new WateringState(aiStateMachine));
                    return;
                }
                else if (field.IsReadyToHarvest())
                {
                    aiStateMachine.TransitionToState(new HarvestingState(aiStateMachine));
                    return;
                }
            }
        }

        aiStateMachine.TransitionToState(new IdleState(aiStateMachine)); // ��� ���¸� Ȯ���� �� Idle ���·� ��ȯ
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
    }

    public override void Update()
    {
        aiStateManager.CheckSeed();
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
    }

    public override void Update()
    {
        // �� �����̿� �����ߴ��� Ȯ��
        if (aiStateManager.MoveToPosition(aiStateManager.waterPosition))
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
    }

    public override void Update()
    {
        if (aiStateManager.currentWaterAmount <= 0)
        {
            aiStateMachine.TransitionToState(new GoToWaterState(aiStateMachine));
            return;
        }

        if (aiStateManager.currentFarmField != null && aiStateManager.currentFarmField.NeedsWater())
        {
            if (aiStateManager.MoveToPosition(aiStateManager.currentFarmField.transform))
            {
                aiStateManager.WaterCrop(); // �� �ֱ�
                CheckTransitions(); // ���� �� �� ���� ��ȯ üũ
            }
        }
        else
        {
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
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
    }

    public override void Update()
    {
        if (aiStateManager.currentFarmField != null && aiStateManager.MoveToPosition(aiStateManager.currentFarmField.transform))
        {
            aiStateManager.HarvestCrop(); // ��Ȯ�ϱ�
            aiStateMachine.TransitionToState(new GoingHomeState(aiStateMachine)); // ������ ���� ���·� ��ȯ
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
    }

    public override void Update()
    {
        // ������ �̵�
        if (aiStateManager.MoveToPosition(aiStateManager.homePosition))
        {
            Debug.Log("���� �����߽��ϴ�.");
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine)); // Idle ���·� ��ȯ
        }
    }

    public override void Exit()
    {
        Debug.Log("GoingHome ���� ����");
    }
}