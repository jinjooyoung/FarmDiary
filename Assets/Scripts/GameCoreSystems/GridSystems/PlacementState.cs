using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IBuildingState를 활용하여 오브젝트의 고유 ID만 받아오면 다른 데이터 값도 얻을 수 있게 하는 듯?
// 처음 써보는 기능이라 확실하지 X 잘 모르겠음
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData placedOBJData;
    OBJPlacer objectPlacer;

    public PlacementState(int id,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData placedOBJData,
                          OBJPlacer objectPlacer)
    {
        ID = id;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.placedOBJData = placedOBJData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);    // 설치할 오브젝트 선택

        if (selectedObjectIndex > -1)       // 설치할 오브젝트가 할당되어 찾았다면
        {
            // 프리뷰 시작
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {id}");
        }
    }

    public void EndState()      // 프리뷰 종료 상태
    {
        previewSystem.StopShowingPreview();
    }

    // PlaceObject 메서드를 통해 설치
    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);   // 설치 가능/불가능 bool값 받아옴
        if (placementValidity == false || GameManager.currentCoin < database.objectsData[selectedObjectIndex].BuyPrice)     // 설치가 불가능할 때
        {
            return;
        }

        if (ID == 4 && objectPlacer.potCount >= 5)      // 솥을 설치할 때 이미 솥이 5개 이상 있다면 설치 불가능
        {
            return;
        }

        // 마지막으로 설치된 오브젝트의 인덱스(데이터 인덱스 말고)
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        GridData selectedData = placedOBJData;      // 선택한 오브젝트의 데이터 받아옴
        // GridData 스크립트에 선언된 placedObjects 딕셔너리에 받아온 데이터 저장
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID, index);

        GameManager.SubtractCoins(database.objectsData[selectedObjectIndex].BuyPrice);

        if (ID == 4 && objectPlacer.potCount < 5)       // 솥을 설치할 때 솥이 5개 이하라면 설치 로직 이후 솥 개수 증가, 가격 증가
        {
            objectPlacer.potCount++;
            ObjectsDatabase.PriceIncrease(4);
            UIManager.instance.Pot.text = ObjectsDatabase.CurrentPrice(4).ToString();
        }

        /*// 오브젝트의 ID를 보고 밭 오브젝트라면 해당 밭 가격을 증가시킴
        switch (ID)
        {
            case 0:
                ObjectsDatabase.PriceIncrease(0);
                UIManager.instance.one.text = ObjectsDatabase.CurrentPrice(0).ToString();
                break;
            case 1:
                ObjectsDatabase.PriceIncrease(1);
                UIManager.instance.two.text = ObjectsDatabase.CurrentPrice(1).ToString();
                break;
            case 2:
                ObjectsDatabase.PriceIncrease(2);
                UIManager.instance.three.text = ObjectsDatabase.CurrentPrice(2).ToString();
                break;
            case 3:
                ObjectsDatabase.PriceIncrease(3);
                UIManager.instance.four.text = ObjectsDatabase.CurrentPrice(3).ToString();
                break;
        }*/

        // 프리뷰 업데이트 (위 PlaceObject 메서드 호출과정에서 오브젝트를 설치했으므로 이제 설치 불가능.
        // 따라서 false로 커서 UI를 빨간색으로 업데이트)
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    // 선택된 (설치할 예정의) 오브젝트의 사이즈를 받아와서 CanPlaceObjectAt로 넘겨, 설치 가능한지 bool값 리턴
    private bool GetPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return placedOBJData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    // 스테이트 업데이트
    public void UpdateState(Vector3Int gridPosition)
    {
        // 선택된 오브젝트를 받아와서 크기를 따져 설치 가능/불가능 판별
        bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);

        // 프리뷰 업데이트
        previewSystem.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), placementValidity);
    }
}
