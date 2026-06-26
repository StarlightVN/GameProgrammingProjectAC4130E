using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // BẮT BUỘC: Sử dụng thư viện Input mới để đồng bộ phím Space và Chuột trái
using TMPro;

public enum BGMAction { Continue, Change, Stop }

[System.Serializable]
public struct CutsceneSlide
{
    [Header("--- HÌNH ẢNH PHÂN CẢNH ---")]
    [Tooltip("Tấm ảnh minh họa cho slide này")]
    public Sprite slideSprite;

    [Header("--- THỜI GIAN KHÓA PHÂN CẢNH ---")]
    [Tooltip("Khoảng thời gian (tính bằng GIÂY) bắt buộc người chơi phải xem, không được phép bấm qua phân cảnh này")]
    public float lockDuration;

    [Header("--- CHỈ THỊ NHẠC NỀN (BGM) ---")]
    [Tooltip("Continue: Giữ nguyên nhạc cũ\nChange: Đổi sang nhạc mới\nStop: Tắt nhạc nền")]
    public BGMAction bgmAction;
    public AudioClip newBgmClip;

    [Header("--- HIỆU ỨNG ÂM THANH (SFX) ---")]
    public AudioClip sfxClip;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("--- DANH SÁCH CHUỖI ẢNH PHÂN CẢNH ---")]
    public List<CutsceneSlide> cutsceneSlides = new List<CutsceneSlide>();

    [Header("--- UI OUTPUT COMPONENTS ---")]
    public Image displayImage;

    [Header("--- TEXT BÁO TIẾP TỤC NHẤP NHÁY ---")]
    [Tooltip("Kéo linh kiện TextMeshPro hiển thị dòng chữ 'Ấn Chuột Trái hoặc Space để tiếp tục...' vào đây")]
    public TextMeshProUGUI continuePromptText;

    [Header("--- AUDIO CHANNELS ---")]
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    [Header("--- CHUYỂN SCENE KHI KẾT THÚC ---")]
    [Tooltip("Tên của Scene tiếp theo để chuyển về sau khi người chơi xem xong (Ví dụ: MainMenu hoặc Gameplay)")]
    public string nextSceneName = "MainMenu";

    private int currentSlideIndex = 0;
    
    // Bộ khóa kiểm soát chu kỳ click của người chơi
    private bool isSlideLocked = true;
    private Coroutine blinkCoroutine;

    void Start()
    {
        if (bgmAudioSource != null) bgmAudioSource.loop = true;
        if (sfxAudioSource != null) sfxAudioSource.loop = false;

        currentSlideIndex = 0;

        // Ban đầu ẩn dòng chữ nhắc nhở đi
        if (continuePromptText != null) continuePromptText.gameObject.SetActive(false);

        if (cutsceneSlides != null && cutsceneSlides.Count > 0)
        {
            PlaySlide(currentSlideIndex);
        }
        else
        {
            Debug.LogError("HỆ THỐNG CUTSCENE: Bạn chưa nạp bất kỳ slide ảnh nào vào danh sách!");
        }
    }

    void Update()
    {
        // Nếu slide đang bị khóa thời gian -> Tuyệt đối không nhận lệnh bấm qua
        if (isSlideLocked) return;

        // Kiểm tra nếu người chơi nhấn Chuột trái HOẶC bấm phím Spacebar trên bàn phím
        bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (mouseClicked || spacePressed)
        {
            AdvanceToNextSlide();
        }
    }

    public void PlaySlide(int index)
    {
        if (index >= cutsceneSlides.Count)
        {
            FinishCutscene();
            return;
        }

        CutsceneSlide currentSlide = cutsceneSlides[index];

        // 1. Cập nhật đổi ảnh phân cảnh
        if (displayImage != null && currentSlide.slideSprite != null)
        {
            displayImage.sprite = currentSlide.slideSprite;
        }

        // 2. Điều phối nhạc nền BGM
        if (bgmAudioSource != null)
        {
            switch (currentSlide.bgmAction)
            {
                case BGMAction.Continue: break;
                case BGMAction.Stop: bgmAudioSource.Stop(); break;
                case BGMAction.Change:
                    if (currentSlide.newBgmClip != null && bgmAudioSource.clip != currentSlide.newBgmClip)
                    {
                        bgmAudioSource.Stop();
                        bgmAudioSource.clip = currentSlide.newBgmClip;
                        bgmAudioSource.Play();
                    }
                    break;
            }
        }

        // 3. Kích hoạt âm thanh SFX (Chạy 1 lần duy nhất)
        if (sfxAudioSource != null && currentSlide.sfxClip != null)
        {
            sfxAudioSource.PlayOneShot(currentSlide.sfxClip);
        }

        // 4. Kích hoạt mạch khóa đếm ngược thời gian theo từng thoại
        StartCoroutine(LockDurationRoutine(currentSlide.lockDuration));
    }

    // Coroutine đếm ngược thời gian khóa cứng phân cảnh
    private IEnumerator LockDurationRoutine(float duration)
    {
        isSlideLocked = true;

        // Tắt ẩn dòng chữ nhắc và dừng Coroutine nhấp nháy cũ nếu có
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (continuePromptText != null) continuePromptText.gameObject.SetActive(false);

        // Chờ đúng số giây quy định thiết kế riêng cho slide này
        yield return new WaitForSeconds(duration);

        // Hết thời gian khóa -> Giải phóng phong ấn input!
        isSlideLocked = false;

        // Bật sáng dòng chữ nhắc lên và kích hoạt luồng nhấp nháy chu kỳ
        if (continuePromptText != null)
        {
            continuePromptText.gameObject.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkTextPromptRoutine());
        }
    }

    // Luồng tự trị nhấp nháy chữ (Fade Lerp chu kỳ 0.5 giây)
    private IEnumerator BlinkTextPromptRoutine()
    {
        if (continuePromptText == null) yield break;

        while (!isSlideLocked)
        {
            // Hiệu ứng mờ dần (Fade Out)
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                continuePromptText.alpha = Mathf.Lerp(1f, 0.1f, elapsed / 0.5f);
                yield return null;
            }

            // Hiệu ứng rõ dần (Fade In)
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                continuePromptText.alpha = Mathf.Lerp(0.1f, 1f, elapsed / 0.5f);
                yield return null;
            }
        }
    }

    private void AdvanceToNextSlide()
    {
        currentSlideIndex++;
        PlaySlide(currentSlideIndex);
    }

    private void FinishCutscene()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (bgmAudioSource != null) bgmAudioSource.Stop();
        
        Debug.Log("HỆ THỐNG CUTSCENE: Đã hiển thị xong toàn bộ slide. Tiến hành tải Scene tiếp theo.");
        SceneManager.LoadScene(nextSceneName);
    }
}