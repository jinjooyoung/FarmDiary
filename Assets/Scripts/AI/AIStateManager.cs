using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine�� ���� ����

    public FarmField farmField;  // 2x2 �� �ʵ�
    public Transform waterPosition;  // �� ������ ��ġ
    public Transform homePosition;   // ���� ��ġ �߰�

    public float movementSpeed = 2f;  // AI �̵� �ӵ�

    public int maxWaterAmount = 5;    // �� �ִ� ������ �߰�
    public int currentWaterAmount;    // ���� �� ������

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine�� ����
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine ������Ʈ�� ã�� �� �����ϴ�.");
        }

        currentWaterAmount = maxWaterAmount;  // ���� �� �ִ� ���������� �ʱ�ȭ
    }

    public bool MoveToPosition(Vector2 target)
    {
        // ��ǥ ��ġ�� �̵�
        transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);

        // ��ǥ ��ġ�� ���� �����ߴ��� Ȯ��
        return Vector2.Distance(transform.position, target) < 0.1f;
    }

    public void CheckSeed()
    {
        farmField.CheckSeedPlanted(new Vector2(0, 0));  // ������ Ȯ�� �ϴ� �޼���
        Debug.Log("���� ������ Ȯ�� �մϴ�");
    }

    public void WaterCrop()
    {
        farmField.WaterCrop(new Vector2(0, 0));  // ���� Ư�� ��ġ�� ���� ��
        currentWaterAmount--;  // �� ���
        Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");
    }


    public void HarvestCrop()
    {
        // �۹��� ��Ȯ�ϴ� �ڵ�
        farmField.Harvest(new Vector2(0, 0)); // ��Ȯ �޼��� ȣ��
        Debug.Log("�۹��� ��Ȯ�߽��ϴ�");
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // �� ����
        Debug.Log("���� �ٽ� ä�����ϴ�.");
    }

    public void AddField()
    {
        // ���� ���� �� �� ���� �� �� ���� ���縦 �ϴ� ����
    }
}
