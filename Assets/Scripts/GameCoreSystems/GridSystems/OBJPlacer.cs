using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPlacer : MonoBehaviour
{
    [SerializeField]
    private PreviewSystem preview;

    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        placedGameObjects.Add(newObject);

        SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        preview.SetAlpha(newObjectRenderer, 1.0f);

        return placedGameObjects.Count - 1;
    }
}
