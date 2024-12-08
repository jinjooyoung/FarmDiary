using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // �̱��� ���� ���


    // ������ �ʿ��� �������� �����ϴ� ��ũ��Ʈ�� ���� �Ǵ� ����
    [Header("�÷��̾� ��ġ")]
    [SerializeField] private Transform playerPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ��� �����͸� �ѹ��� �����ϴ� �޼���
    public void SaveGameData()
    {
        // ������ �����ؾ� �� �����͵��� SaveDatas���� ó��
        //SaveDatas.SaveGridData();
        //SaveDatas.SavePotData();
        //SaveDatas.SaveFieldData();
        // �ʿ信 ���� �� ���� ���� �޼��带 �߰�
    }

    // ��� �����͸� �ѹ��� �ε��ϴ� �޼���
    public void LoadGameData()
    {
        // ������ �����͸� �ε��ϴ� �޼��带 ȣ��
        //SaveDatas.LoadGridData();
        //SaveDatas.LoadPotData();
        //SaveDatas.LoadFieldData();
        // �ʿ信 ���� �� ���� �ε� �޼��带 �߰�
    }
}
