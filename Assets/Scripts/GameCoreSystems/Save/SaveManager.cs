using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // �̱��� ���� ���

    [Header("����ȭ Ŭ����, ���̺� ����")]
    [SerializeField] private NewSaveData newSaveData;

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
        //saveDatas.SaveCrops(objPlacer.placedGameObjects);

        // ��ġ�Ǵ� ��� ������Ʈ ����
        newSaveData.SaveOBJs();
        // AI ������ ����
        newSaveData.SaveAIData();
        // â�� ����
        newSaveData.SaveStorage();
        // ���� ������ ����
        newSaveData.SaveAchievements();
        // ���� ���� ����
        newSaveData.SaveBuyPrice();
    }

    // ��� �����͸� �ѹ��� �ε��ϴ� �޼���
    public void LoadGameData()
    {
        // ������ �����͸� �ε��ϴ� �޼��带 ȣ��
        //saveDatas.LoadCrops();

        // ������Ʈ �ٽ� ������ؼ� �ε���, ��ġ�ǰ� ���� ����� ������Ʈ ������ ��Ȳ�� �ε� �� ��, ������Ʈ ���� �ε��ؼ� �����صΰ� ���� ������ ���� ����� �ؾ���
        newSaveData.LoadOBJs();
        // AI ������ �ε�
        newSaveData.LoadAIData();
        // â�� ������ �ε�
        newSaveData.LoadStorage();
        // ���� ������ �ε�
        newSaveData.LoadAchievements();
        // ���� ���� �ε�
        newSaveData.LoadBuyPrice();
    }
}
