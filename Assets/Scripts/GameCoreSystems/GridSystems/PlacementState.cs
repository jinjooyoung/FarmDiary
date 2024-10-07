using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState
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
    }
}
