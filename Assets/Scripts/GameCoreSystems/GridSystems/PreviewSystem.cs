using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;               // 미리보기 오브젝트

    //[SerializeField]
    //private SpriteRenderer previewPrefab;

    private SpriteRenderer cellIndicatorRenderer;
    //private SpriteRenderer previewInstance;

    private void Start()
    {
        // cellIndicator와 previewObject의 SpriteRenderer 가져오기
        //cellIndicatorRenderer = cellIndicator.GetComponent<SpriteRenderer>();
        //previewObjectRenderer = previewObject.GetComponent<SpriteRenderer>();

        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<SpriteRenderer>();

        // 알파값 설정 (여기서는 50% 투명도 예시)
        //SetAlpha(cellIndicatorRenderer, 0.5f);
        //SetAlpha(previewObjectRenderer, 0.5f);
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        SpriteRenderer renderer = prefab.GetComponentInChildren<SpriteRenderer>();
        previewObject = Instantiate(prefab);
        PrepareCursor(size);
        SetAlpha(renderer, 0.2f);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)     // 커서 크기 자체는 오브젝트 크기에 맞춰 커지는데 커서의 위치가 어긋남 해결필요
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, -size.y, 1);
            //cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    public void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha; // 알파값 조정
        renderer.color = color;
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePreviewOBJPos(Vector3 position, bool validity)
    {
        MovePreview(position);
        MoveCursor(position);
        ApplyFeedback(validity);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y, 1);
    }

    private void MoveCursor(Vector3 position)
    {
        // 그리드 커서 위치 맞추려고 y 좌표 뒤에 0.5는 하드코딩함, 칸 크기 바뀌면 다시 설정해야함
        cellIndicator.transform.position = new Vector3(position.x, position.y + 0.5f, 1);
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        cellIndicatorRenderer.color = c;
    }
}
