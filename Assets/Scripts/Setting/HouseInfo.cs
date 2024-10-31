using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInfo : MonoBehaviour
{
    public List<Crop> storedCrops = new List<Crop>(); // ���ȿ� ������ �۹� ����Ʈ

    public AIStateManager aiStateManager; // AIStateManager ����

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("����");

            // ���� ���� ��Ȯ�� �۹��� ������ ��������.
            foreach (var harvestedCrop in aiStateManager.harvestedCrops)
            {
                AddCropToHouse(harvestedCrop); // �۹��� ���� �߰�
            }

            // ��Ȯ�� �۹� ����� ����.
            aiStateManager.harvestedCrops.Clear();
            Debug.Log("��� ��Ȯ�� �۹��� ������ �̵��Ǿ����ϴ�.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("����");
    }

    private void AddCropToHouse(Crop crop)
    {
        storedCrops.Add(crop); // �۹��� �� ����Ʈ�� �߰�
        Debug.Log(crop.ID);
    }
}
