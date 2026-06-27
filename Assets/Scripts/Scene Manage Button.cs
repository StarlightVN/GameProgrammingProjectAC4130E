using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // BẮT BUỘC: Thêm thư viện này để sử dụng Coroutine delay âm thanh

public class SceneManageButton : MonoBehaviour {
    
    [Header("--- CẤU HÌNH ÂM THANH CLICK COI ---")]
    [Tooltip("Kéo linh kiện Audio Source chuyên phát tiếng động ngắn vào đây")]
    public AudioSource sfxSource;
    [Tooltip("Kéo file tệp âm thanh click chuột (.mp3/.wav) vào đây")]
    public AudioClip clickSFX;

    public void GoToScene(string sceneName) {
        // Kiểm tra nếu có đầy đủ bộ phát và tệp âm thanh click
        if (sfxSource != null && clickSFX != null) {
            sfxSource.PlayOneShot(clickSFX);
            // Kích hoạt luồng chờ tiếng click kêu xong một khoảng ngắn rồi mới đổi Scene
            StartCoroutine(LoadSceneAfterSound(sceneName, clickSFX.length));
        } else {
            // Bảo hiểm: Nếu quên gán âm thanh thì vẫn chuyển cảnh bình thường không bị lỗi kẹt
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator LoadSceneAfterSound(string sceneName, float soundLength) {
        // Chờ một khoảng bằng độ dài file âm thanh (nhưng tối đa 0.25 giây để người chơi không thấy bị hoãn quá lâu)
        float practicalDelay = Mathf.Min(soundLength, 0.25f);
        yield return new WaitForSecondsRealtime(practicalDelay);
        
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp() {
        if (sfxSource != null && clickSFX != null) {
            sfxSource.PlayOneShot(clickSFX);
            StartCoroutine(QuitAfterSound(clickSFX.length));
        } else {
            ExecuteQuit();
        }
    }

    private IEnumerator QuitAfterSound(float soundLength) {
        yield return new WaitForSecondsRealtime(Mathf.Min(soundLength, 0.25f));
        ExecuteQuit();
    }

    private void ExecuteQuit() {
        Application.Quit();
        Debug.Log("Application has quit.");
    }

    // 🔥 HÀM BỔ TRỢ: Để các thành phần UI khác (Slider, Dropdown) ké tiếng click khi tương tác
    public void PlayClickSoundOnly() {
        if (sfxSource != null && clickSFX != null) {
            sfxSource.PlayOneShot(clickSFX);
        }
    }
}