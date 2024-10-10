using UnityEditor.SceneManagement;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    public AIState currentState;                // ���� �÷��̾��� ���¸� ��Ÿ���� ����
    public AIStateManager aiStateManager;       // AIStateManager�� ����

    private void Awake()
    {
        aiStateManager = GetComponent<AIStateManager>();    // AIStateManager�� ����
    }

    void Start()
    {
        // �ʱ� ���¸� IdleState �� ����
        TransitionToState(new IdleState(this));
    }

    void Update()
    {
        // ���� ���װ� �����Ѵٸ� ���� ������ Update �޼��� ȣ��
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    private void FixedUpdate()
    {
        // ���� ���װ� �����Ѵٸ� ���� ������ FixedUpdate �޼��� ȣ��
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }

    // ���ο� ���·� ��ȯ �ϴ� �޼���
    public void TransitionToState(AIState newstate)
    {
        // ���� ���°� ���� �Ѵٸ� Exit �޼��带 ȣ��
        currentState?.Exit();       // �˻��ؼ� ȣ�� ���� (?)�� IF ����

        // ���ο� ���·� ��ȯ
        currentState = newstate;

        // ���ο� ������ Enter �޼��带 ȣ��(���� ����)
        currentState.Enter();

        // �α׿� ���� ��ȯ ������ ���
        Debug.Log($"���� ��ȯ �Ǵ� ������Ʈ {newstate.GetType().Name}");
    }
}
