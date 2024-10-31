using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject seedPanel;
    public GameObject storagePanel;
    public GameObject ShopPanel;
    public GameObject questPanel;
    public GameObject settingPanel;

    public Button seedButton;
    public Button storageButton;
    public Button ShopButton;
    public Button questButton;
    public Button settingButton;

    void Start()
    {
        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        storageButton.onClick.AddListener(() => TogglePanel(storagePanel));
        ShopButton.onClick.AddListener(() => TogglePanel(ShopPanel));
        questButton.onClick.AddListener(() => TogglePanel(questPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));
    }

    // �г��� Ȱ��ȭ ���¸� ����ϴ� �޼���
    void TogglePanel(GameObject panel)
    {
        // ��� �г��� ���� �ݱ�
        CloseAllPanels();

        // Ŭ���� �гθ� ���� ������ �ݴ�� ����
        panel.SetActive(!panel.activeSelf);
    }

    // ��� �г��� �ݴ� �޼���
    public void CloseAllPanels()
    {
        seedPanel.SetActive(false);
        storagePanel.SetActive(false);
        ShopPanel.SetActive(false);
        questPanel.SetActive(false);
        settingPanel.SetActive(false);
    }
}
