using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // �̱��� ���� ���
    public GameObject loadingIcon;

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
    public async Task SaveGameData()
    {
        loadingIcon.SetActive(true);

        // ���� �۾�
        // AI ������ ����
        newSaveData.SaveAIData();
        // â�� ����
        newSaveData.SaveStorage();
        // ���� ������ ����
        newSaveData.SaveAchievements();
        // ���� ���� ����
        newSaveData.SaveBuyPrice();


        // �񵿱� �۾�
        // ��ġ�Ǵ� ��� ������Ʈ ����
        await newSaveData.SaveOBJs();

        UpdateUIAfterSaveLoad();
    }

    // ��� �����͸� �ѹ��� �ε��ϴ� �޼���
    public async Task LoadGameData()
    {
        loadingIcon.SetActive(true);
        // ���� �۾�
        // AI ������ �ε�
        newSaveData.LoadAIData();
        // â�� ������ �ε�
        newSaveData.LoadStorage();
        // ���� ������ �ε�
        newSaveData.LoadAchievements();
        // ���� ���� �ε�
        newSaveData.LoadBuyPrice();

        // �񵿱� �۾�
        // ������Ʈ �ٽ� ������ؼ� �ε���, ��ġ�ǰ� ���� ����� ������Ʈ ������ ��Ȳ�� �ε� �� ��, ������Ʈ ���� �ε��ؼ� �����صΰ� ���� ������ ���� ����� �ؾ���
        await newSaveData.LoadOBJs();

        UpdateUIAfterSaveLoad();
    }

    // UI ������Ʈ �޼��� (���� �����忡�� ����)
    private void UpdateUIAfterSaveLoad()
    {
        loadingIcon.SetActive(false);
        Debug.LogWarning("�񵿱� ó�� �Ϸ� �Ǿ� UI ������Ʈ ȣ���!");
    }
}
