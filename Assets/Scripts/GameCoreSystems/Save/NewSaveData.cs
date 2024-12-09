using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSaveData : MonoBehaviour
{
    public Grid grid;
    public PlacementSystem placementSystem;
    public OBJPlacer objPlacer;
    public GridData gridData;

    // ������ ������ ����Ʈ
    private List<OBJData> objDataList = new List<OBJData>();

    private void Awake()
    {
        // ���� GridData �ν��Ͻ��� ����
        gridData = placementSystem.placedOBJData;
    }

    // ������ ��� ������Ʈ�� ID�� ��ġ(gridPos), ��ġ ���� (index)�� ����
    [System.Serializable]
    public class OBJData
    {
        public int ID;                  // ���� ID
        public Vector3Int objPosition; // ��ġ �׸��� ��ġ
        public int Index;               // ��ġ ����
    }

    // OBJData ����Ʈ�� ���� ���� Ŭ����
    [System.Serializable]
    public class OBJDataListWrapper
    {
        public List<OBJData> objDataList;
    }

    // ID�� �޾Ƽ� ������ ������Ʈ�� ���Ϲ޴� �Լ�    �ε忡�� ���
    public GameObject GetPrefabFromResourcesByID(int id)
    {
        return Resources.Load<GameObject>($"Prefabs/{id}");
    }

    //===================================

    public void SaveOBJs()
    {
        objDataList.Clear();  // ����Ʈ �ʱ�ȭ

        // placedGameObjects�� �ִ� ��� ���� ������Ʈ�� ���� ó��
        for (int i = 0; i < objPlacer.placedGameObjects.Count; i++)
        {
            GameObject placedObj = objPlacer.placedGameObjects[i];

            // ���� ������Ʈ �̸����� ID�� ���� (��: "123(Clone)"���� 123�� ����)
            int objectID = int.Parse(placedObj.name.Split('(')[0]);

            // ���� ������Ʈ�� transform.position�� Grid�� Cell�� ��ȯ
            Vector3Int objPosition = grid.WorldToCell(placedObj.transform.position);

            // OBJData ����
            OBJData objData = new OBJData
            {
                ID = objectID,
                objPosition = objPosition,
                Index = i
            };

            // ����Ʈ�� �߰�
            objDataList.Add(objData);
        }

        // OBJData ����Ʈ�� JSON���� ��ȯ�Ͽ� ����
        string json = JsonUtility.ToJson(new OBJDataListWrapper { objDataList = objDataList });

        // SaveSystem�� Save �޼��带 ����Ͽ� ���Ͽ� ����
        SaveSystem.Save(json, "PlacedObjects"); // "PlacedObjects"�� ���� �̸�
        Debug.Log("������Ʈ ���� �Ϸ�");
    }
}