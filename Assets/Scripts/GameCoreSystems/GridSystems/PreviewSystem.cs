using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;               // �̸����� ������Ʈ

    //[SerializeField]
    //private SpriteRenderer previewPrefab;

    private SpriteRenderer cellIndicatorRenderer;
    //private SpriteRenderer previewInstance;

    private void Start()
    {
        // cellIndicator�� previewObject�� SpriteRenderer ��������
        //cellIndicatorRenderer = cellIndicator.GetComponent<SpriteRenderer>();
        //previewObjectRenderer = previewObject.GetComponent<SpriteRenderer>();

        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<SpriteRenderer>();

        // ���İ� ���� (���⼭�� 50% ���� ����)
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

    private void PrepareCursor(Vector2Int size)     // Ŀ�� ũ�� ��ü�� ������Ʈ ũ�⿡ ���� Ŀ���µ� Ŀ���� ��ġ�� ��߳� �ذ��ʿ�
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
        color.a = alpha; // ���İ� ����
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
        // �׸��� Ŀ�� ��ġ ���߷��� y ��ǥ �ڿ� 0.5�� �ϵ��ڵ���, ĭ ũ�� �ٲ�� �ٽ� �����ؾ���
        cellIndicator.transform.position = new Vector3(position.x, position.y + 0.5f, 1);
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        cellIndicatorRenderer.color = c;
    }
}
