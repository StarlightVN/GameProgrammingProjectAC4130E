using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DaySummaryController : MonoBehaviour
{
    [Header("--- UI Elements ---")]
    [SerializeField] private TextMeshProUGUI txtCorrectCount;
    [SerializeField] private TextMeshProUGUI txtIncorrectCount;
    [SerializeField] private TextMeshProUGUI txtAccuracy;
    [SerializeField] private Button btnNextDay;

    [Header("--- Scene Management ---")]
    [Tooltip("Tên chính xác của Scene Cutscene Ngày 2 trong Build Settings")]
    [SerializeField] private string nextSceneName = "Cutscene_Day2";

    private void Awake()
    {
        // Gắn sự kiện click cho nút bấm chuyển ngày
        if (btnNextDay != null)
        {
            btnNextDay.onClick.AddListener(LoadNextScene);
        }
    }

    /// <summary>
    /// Hàm nhận dữ liệu từ SchoolGateManager để tính toán và hiển thị lên màn hình
    /// </summary>
    public void DisplayStats(int correct, int incorrect)
    {
        // Hiển thị số lượng
        if (txtCorrectCount != null) txtCorrectCount.text = $"{correct}";
        if (txtIncorrectCount != null) txtIncorrectCount.text = $"{incorrect}";

        // Tính toán tỷ lệ chính xác (%)
        int total = correct + incorrect;
        float accuracyPercent = 0f;

        if (total > 0)
        {
            accuracyPercent = ((float)correct / total) * 100f;
        }

        if (txtAccuracy != null)
        {
            // Định dạng hiển thị 1 chữ số thập phân (Ví dụ: Tỉ lệ chính xác: 85.5%)
            txtAccuracy.text = $"{accuracyPercent:F1}%";
        }
    }

    /// <summary>
    /// Hàm chuyển Scene khi người chơi ấn nút "Tiếp tục"
    /// </summary>
    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Chưa điền tên Scene tiếp theo vào DaySummaryController!");
        }
    }
}