using System.Collections;
using System.Collections.Generic;
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
        // ��ȿ���� ���� currentCrop�� �ִٸ� null�� �����ϰ� ���� ���·� �̵�
        if (aiStateManager.currentCrop == null || !CropGrowthManager.Instance.crops.Contains(aiStateManager.currentCrop))
        {
            aiStateManager.currentCrop = null;  // ���� �۹� �ʱ�ȭ
            aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
            return;
        }

        foreach (Crop crops in CropGrowthManager.Instance.crops)
        {
            if (crops.IsSeedPlanted())
            {
                aiStateManager.currentCrop = crops; // ���� �۾� ���� ���� ����
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
        if (aiStateManager.currentWaterAmount == 0)
        {
            aiStateMachine.TransitionToState(new GoToWaterState(aiStateMachine));
            return;
        }

        if (aiStateManager.currentCrop != null && aiStateManager.currentCrop.NeedsWater())
        {
            if (aiStateManager.MoveToPosition(aiStateManager.currentCrop.transform))
            {
                aiStateManager.WaterCrop(); // �� �ֱ�
                Debug.Log("�۹��� ���� �־����ϴ�.");

                // ���� �� �� �Ŀ� ���� ��ȯ Ȯ��
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
        // ���� ��Ȯ�� �غ� �� �۹��� �ִ��� Ȯ��
        if (aiStateManager.currentCrop != null && aiStateManager.currentCrop.IsReadyToHarvest())
        {
            if (aiStateManager.MoveToPosition(aiStateManager.currentCrop.transform))
            {
                Debug.Log("�۹��� �����߽��ϴ�. ��Ȯ�� �����մϴ�.");
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
            CheckTransitions();
        }
    }

    public override void Exit()
    {
        Debug.Log("GoingHome ���� ����");
    }
}