using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    public static PotionManager instance;

    public List<GameObject> potList = new List<GameObject>(new GameObject[15]);
    [SerializeField] private PotionDatabaseSO potionsDatabase; // ���� �����ͺ��̽�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PotionDatabase.Initialize(potionsDatabase);
    }

    // ���� �߰��ϴ� �޼���
    public bool AddPot(GameObject pot)
    {
        // ����ִ� ID�� ã��
        int idToAssign = GetAvailableID();
        if (idToAssign == -1)
        {
            Debug.LogError("���� ������ �ִ� 5�������Դϴ�.");
            return false; // ���� �߰��� �� ����
        }

        // ���� ID ����
        Pot potComponent = pot.GetComponent<Pot>();  // Pot ������Ʈ ��������
        if (potComponent != null)
        {
            potComponent.SetID(idToAssign);  // Pot ������Ʈ�� ID ����
            potComponent.Initialize();      // pot �ʱ�ȭ
        }

        potList[idToAssign] = pot;  // �ش� ID ��ġ�� ���� ������Ʈ �Ҵ�
        return true; // �߰� ����
    }

    // ���� �����ϴ� �޼���
    public void RemovePot(GameObject pot)
    {
        int index = potList.IndexOf(pot);
        if (index != -1)
        {
            // ���� ������Ʈ ����
            potList[index] = null;  // �ش� ID ��ġ�� ���� ������Ʈ ����
            Pot potComponent = pot.GetComponent<Pot>();
            if (potComponent != null)
            {
                // ������ ID�� �ٽ� ��� �����ϵ��� ó��
                potComponent.ClearID();
            }
        }
    }

    // ����ִ� ID�� ã�� ��ȯ�ϴ� �޼���
    private int GetAvailableID()
    {
        // potList���� null�� �ε����� ã�� ��ȯ
        for (int i = 0; i < 15; i++)
        {
            if (potList[i] == null)
            {
                return i;  // ����ִ� ID ��ȯ
            }
        }

        return -1;  // ��� ������ ID�� ������ -1 ��ȯ
    }
}
