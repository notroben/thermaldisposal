using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPanel : MonoBehaviour
{
    [Header("Slideshow")]
    public Image slideImage;
    public Sprite[] tutorialSlides;

    [Header("Navigation")]
    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI pageIndicator;

    private int currentSlide = 0;

    void OnEnable()
    {
        currentSlide = 0;
        UpdateSlide();
    }

    public void OnPrevious()
    {
        if (currentSlide > 0)
        {
            currentSlide--;
            UpdateSlide();
        }
    }

    public void OnNext()
    {
        if (currentSlide < tutorialSlides.Length - 1)
        {
            currentSlide++;
            UpdateSlide();
        }
    }

    void UpdateSlide()
    {
        if (tutorialSlides == null || tutorialSlides.Length == 0) return;
        if (slideImage != null) slideImage.sprite = tutorialSlides[currentSlide];
        if (prevButton != null) prevButton.interactable = currentSlide > 0;
        if (nextButton != null) nextButton.interactable = currentSlide < tutorialSlides.Length - 1;
        if (pageIndicator != null) pageIndicator.text = (currentSlide + 1) + " / " + tutorialSlides.Length;
    }
}
