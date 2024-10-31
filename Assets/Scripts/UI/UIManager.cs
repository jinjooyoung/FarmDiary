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
        // 버튼에 클릭 이벤트 추가
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        storageButton.onClick.AddListener(() => TogglePanel(storagePanel));
        ShopButton.onClick.AddListener(() => TogglePanel(ShopPanel));
        questButton.onClick.AddListener(() => TogglePanel(questPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));
    }

    // 패널의 활성화 상태를 토글하는 메서드
    void TogglePanel(GameObject panel)
    {
        // 모든 패널을 먼저 닫기
        CloseAllPanels();

        // 클릭된 패널만 현재 상태의 반대로 설정
        panel.SetActive(!panel.activeSelf);
    }

    // 모든 패널을 닫는 메서드
    public void CloseAllPanels()
    {
        seedPanel.SetActive(false);
        storagePanel.SetActive(false);
        ShopPanel.SetActive(false);
        questPanel.SetActive(false);
        settingPanel.SetActive(false);
    }
}
