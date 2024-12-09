using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // �̱��� ���� ���


    // ������ �ʿ��� �������� �����ϴ� ��ũ��Ʈ�� ���� �Ǵ� ����
    [Header("�÷��̾� ��ġ")]
    [SerializeField] private Transform playerPos;
    [Header("OBJ Placer")]
    [SerializeField] private OBJPlacer objPlacer;
    [Header("����ȭ Ŭ����, ���̺� ����")]
    [SerializeField] private NewSaveData newSaveData;
    /*[SerializeField] private SaveDatas saveDatas;*/


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

        // �ϴ� �ܺ���
        //newSaveData.SaveOBJs();
    }

    // ��� �����͸� �ѹ��� �ε��ϴ� �޼���
    public void LoadGameData()
    {
        // ������ �����͸� �ε��ϴ� �޼��带 ȣ��
        //saveDatas.LoadCrops();
    }
}
