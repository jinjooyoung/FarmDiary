using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private AIStateManager aiStateManager;  // AI�� ���� �Ŵ���
    private AIStateMachine aiStateMachine;  // AI�� ���� �ӽ�

    private void Start()
    {
        // AIStateManager�� AIStateMachine �ʱ�ȭ
        aiStateManager = GetComponent<AIStateManager>();
        aiStateMachine = GetComponent<AIStateMachine>();

        if (aiStateManager == null || aiStateMachine == null) return;

        // �ʱ� ���� ���� (Idle ���·� ����)
        aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
    }

    private void Update()
    {
        // ���� ������ Update �޼��� ȣ��
        aiStateMachine.currentState?.Update();
    }
}
