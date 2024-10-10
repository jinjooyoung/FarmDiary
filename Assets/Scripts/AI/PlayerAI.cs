using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private AIStateManager aiStateManager;  // AI의 상태 매니저
    private AIStateMachine aiStateMachine;  // AI의 상태 머신

    private void Start()
    {
        // AIStateManager와 AIStateMachine 초기화
        aiStateManager = GetComponent<AIStateManager>();
        aiStateMachine = GetComponent<AIStateMachine>();

        if (aiStateManager == null || aiStateMachine == null) return;

        // 초기 상태 설정 (Idle 상태로 시작)
        aiStateMachine.TransitionToState(new IdleState(aiStateMachine));
    }

    private void Update()
    {
        // 현재 상태의 Update 메서드 호출
        aiStateMachine.currentState?.Update();
    }
}
