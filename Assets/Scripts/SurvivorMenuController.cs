using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class manages the Toggleable Shop Menu
public class SurvivorMenuController : MonoBehaviour
{
    public GameObject panel; // The survivor menu panel
    public TMP_Dropdown laneDropdown;// Dropdown for lane selection

    public Button buyTypeAButton;
    public Button buyTypeBButton;
    public Button buyTypeCButton;
    public Button sellButton;
    public Button closeButton;

    private GameManager gameManager;
    private int selectedLane = 1;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (panel != null)
            panel.SetActive(false);  // Start hidden

        if (laneDropdown != null)
        {
            laneDropdown.onValueChanged.AddListener(OnLaneChanged);
            laneDropdown.value = 0;  // Default to Lane 1
            selectedLane = 1;
        }

        // Assign button listeners
        if (buyTypeAButton != null)
            buyTypeAButton.onClick.AddListener(() => BuySurvivor(1));
        if (buyTypeBButton != null)
            buyTypeBButton.onClick.AddListener(() => BuySurvivor(2));
        if (buyTypeCButton != null)
            buyTypeCButton.onClick.AddListener(() => BuySurvivor(3));
        if (sellButton != null)
            sellButton.onClick.AddListener(SellSurvivor);
        // Close button
        if (closeButton != null)
            closeButton.onClick.AddListener(() => panel.SetActive(false));

    }

    public void TogglePanel()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }

    private void OnLaneChanged(int index)
    {
        selectedLane = index + 1;  // Dropdown index 0 maps to lane 1
    }

    private void BuySurvivor(int survivorType)
    {
        if (gameManager != null)
        {
            int cost = 0;
            switch (survivorType)
            {
                case 1: cost = gameManager.costTypeA; break;
                case 2: cost = gameManager.costTypeB; break;
                case 3: cost = gameManager.costTypeC; break;
            }
            gameManager.BuySurvivor(selectedLane, survivorType, cost);
        }
    }

    private void SellSurvivor()
    {
        if (gameManager != null)
            gameManager.SellSurvivor(selectedLane);
    }
}
