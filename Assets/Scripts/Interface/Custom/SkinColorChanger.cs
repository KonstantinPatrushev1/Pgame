using UnityEngine;
using UnityEngine.UI;
using Save;

public class SkinColorChanger : MonoBehaviour
{
    public Image skinhead;
    public Image headmap;
    public Slider skinToneSlider;  // Ползунок для изменения оттенка кожи
    public Button hatLeftButton;    // Кнопка влево для шапки
    public Button hatRightButton;   // Кнопка вправо для шапки
    public Button pantsLeftButton;  // Кнопка влево для штанов
    public Button pantsRightButton; // Кнопка вправо для штанов
    public Button shirtLeftButton;  // Кнопка влево для рубашки
    public Button shirtRightButton; // Кнопка вправо для рубашки
    
    public Image characterImage;  // Изображение персонажа (основной)
    public SpriteRenderer characterRenderer;
    public SpriteRenderer hatRenderer;  // SpriteRenderer для шапки
    public SpriteRenderer pantsRenderer;  // SpriteRenderer для штанов
    public SpriteRenderer shirtRenderer;  // SpriteRenderer для рубашки
    public Image hatImage;  // Изображение шапки
    public Image pantsImage;  // Изображение штанов
    public Image shirtImage;  // Изображение рубашки

    private Color selectedSkinTone;
    
    public Color lightSkinColor = new Color(1f, 0.8f, 0.6f);  // Светлый оттенок кожи
    public Color mediumSkinColor = new Color(0.9f, 0.6f, 0.4f);  // Средний оттенок кожи
    public Color darkSkinColor = new Color(0.6f, 0.4f, 0.2f);  // Смуглый оттенок кожи

    public Sprite[] hatSprites;  // Массив спрайтов для шапки
    public Sprite[] pantsSprites;  // Массив спрайтов для штанов
    public Sprite[] shirtSprites;  // Массив спрайтов для рубашки

    // Переменные для аниматоров
    public Animator hatAnimator;   // Аниматор для шляпы
    public Animator pantsAnimator; // Аниматор для штанов
    public Animator shirtAnimator; // Аниматор для рубашки

    public AnimatorOverrideController[] hatOverrideControllers;   // Массив для override аниматоров шляпы
    public AnimatorOverrideController[] pantsOverrideControllers; // Массив для override аниматоров штанов
    public AnimatorOverrideController[] shirtOverrideControllers; // Массив для override аниматоров рубашки
    
    public Button hatColorButton;    // Кнопка для выбора цвета шляпы
    public Button pantsColorButton;  // Кнопка для выбора цвета штанов
    public Button shirtColorButton;  // Кнопка для выбора цвета рубашки

    public GameObject colorPanel;    // Панель выбора цвета, которая будет активироваться
    public Slider redSlider;         // Слайдер для красного цвета
    public Slider greenSlider;       // Слайдер для зеленого цвета
    public Slider blueSlider;        // Слайдер для синего цвета

    private Image selectedClothingImage; // Храним ссылку на изображение текущей одежды

    // Цвета одежды
    private Color hatColor;
    private Color pantsColor;
    private Color shirtColor;
    
    // Индексы текущей одежды
    private int hatIndex = 0;
    private int pantsIndex = 0;
    private int shirtIndex = 0;

    void Start()
    {
        // Инициализация UI элементов
        skinToneSlider.value = 0.5f;  

        // Подключаем слушатели событий для ползунка кожи
        skinToneSlider.onValueChanged.AddListener(UpdateSkinTone);

        // Подключаем слушатели для кнопок одежды
        hatLeftButton.onClick.AddListener(() => ChangeHat(-1));
        hatRightButton.onClick.AddListener(() => ChangeHat(1));
        pantsLeftButton.onClick.AddListener(() => ChangePants(-1));
        pantsRightButton.onClick.AddListener(() => ChangePants(1));
        shirtLeftButton.onClick.AddListener(() => ChangeShirt(-1));
        shirtRightButton.onClick.AddListener(() => ChangeShirt(1));

        // Слушатели для кнопок выбора цвета
        hatColorButton.onClick.AddListener(() => OpenColorPanel(hatImage));
        pantsColorButton.onClick.AddListener(() => OpenColorPanel(pantsImage));
        shirtColorButton.onClick.AddListener(() => OpenColorPanel(shirtImage));

        // Инициализация внешнего вида
        UpdateSkinTone(0.5f);
        UpdateHatVisuals();
        UpdatePantsVisuals();
        UpdateShirtVisuals();
    }

    void OpenColorPanel(Image clothingImage)
    {
        // Отображаем панель выбора цвета
        colorPanel.SetActive(true);
        
        // Запоминаем, какую одежду мы редактируем
        selectedClothingImage = clothingImage;
        
        // Устанавливаем текущий цвет одежды на слайдерах RGB
        Color currentColor = selectedClothingImage.color;
        redSlider.value = currentColor.r;
        greenSlider.value = currentColor.g;
        blueSlider.value = currentColor.b;

        // Настроим слушателей для изменения цвета
        redSlider.onValueChanged.AddListener(UpdateClothingColor);
        greenSlider.onValueChanged.AddListener(UpdateClothingColor);
        blueSlider.onValueChanged.AddListener(UpdateClothingColor);
    }

    void UpdateClothingColor(float value)
    {
        if (selectedClothingImage == hatImage)
        {
            hatColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
            hatImage.color = hatColor;
            headmap.color = hatColor;
        }
        else if (selectedClothingImage == pantsImage)
        {
            pantsColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
            pantsImage.color = pantsColor;
        }
        else if (selectedClothingImage == shirtImage)
        {
            shirtColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
            shirtImage.color = shirtColor;
        }
    }

    void UpdateSkinTone(float value)
    {
        if (value < 0.5f)
        {
            selectedSkinTone = Color.Lerp(lightSkinColor, mediumSkinColor, value * 2);
        }
        else
        {
            selectedSkinTone = Color.Lerp(mediumSkinColor, darkSkinColor, (value - 0.5f) * 2);
        }

        characterImage.color = selectedSkinTone;
        skinhead.color = selectedSkinTone;
    }

    void ChangeHat(int direction)
    {
        hatIndex += direction;
        // Ограничиваем индекс в пределах массива спрайтов
        hatIndex = Mathf.Clamp(hatIndex, 0, hatSprites.Length - 1);
        UpdateHatVisuals();
    }

    void ChangePants(int direction)
    {
        pantsIndex += direction;
        // Ограничиваем индекс в пределах массива спрайтов
        pantsIndex = Mathf.Clamp(pantsIndex, 0, pantsSprites.Length - 1);
        UpdatePantsVisuals();
    }

    void ChangeShirt(int direction)
    {
        shirtIndex += direction;
        // Ограничиваем индекс в пределах массива спрайтов
        shirtIndex = Mathf.Clamp(shirtIndex, 0, shirtSprites.Length - 1);
        UpdateShirtVisuals();
    }

    void UpdateHatVisuals()
    {
        hatImage.sprite = hatSprites[hatIndex];
        headmap.sprite = hatSprites[hatIndex];
    }

    void UpdatePantsVisuals()
    {
        pantsImage.sprite = pantsSprites[pantsIndex];
    }

    void UpdateShirtVisuals()
    {
        shirtImage.sprite = shirtSprites[shirtIndex];
    }
    
    public void Apply()
    {
        hatAnimator.runtimeAnimatorController = hatOverrideControllers[hatIndex];
        pantsAnimator.runtimeAnimatorController = pantsOverrideControllers[pantsIndex];
        shirtAnimator.runtimeAnimatorController = shirtOverrideControllers[shirtIndex];
        characterRenderer.color = selectedSkinTone;
        shirtRenderer.color = shirtImage.color;
        pantsRenderer.color = pantsImage.color;
        hatRenderer.color = hatImage.color;
    }
    
    public void SaveCustomization(Storage storage, GameData gameData, string fileName)
    {
        gameData.skinColor = new SerializableColor(characterRenderer.color);
        gameData.hatColor = new SerializableColor(hatRenderer.color);
        gameData.pantsColor = new SerializableColor(pantsRenderer.color);
        gameData.shirtColor = new SerializableColor(shirtRenderer.color);

        gameData.hatAnimationIndex = hatIndex;
        gameData.pantsAnimationIndex = pantsIndex;
        gameData.shirtAnimationIndex = shirtIndex;

        storage.Save(fileName, gameData);
    }

    public void LoadCustomization(Storage storage, GameData gameData, string fileName)
    {
        gameData = (GameData)storage.Load(fileName, new GameData());
        if (gameData != null)
        {
            characterRenderer.color = gameData.skinColor.ToColor();
            skinhead.color = gameData.skinColor.ToColor();
            headmap.color = gameData.hatColor.ToColor();
            hatRenderer.color = gameData.hatColor.ToColor();
            pantsRenderer.color = gameData.pantsColor.ToColor();
            shirtRenderer.color = gameData.shirtColor.ToColor();

            hatIndex = gameData.hatAnimationIndex;
            pantsIndex = gameData.pantsAnimationIndex;
            shirtIndex = gameData.shirtAnimationIndex;

            hatAnimator.runtimeAnimatorController = hatOverrideControllers[hatIndex];
            pantsAnimator.runtimeAnimatorController = pantsOverrideControllers[pantsIndex];
            shirtAnimator.runtimeAnimatorController = shirtOverrideControllers[shirtIndex];
            
            // Обновляем UI
            UpdateHatVisuals();
            UpdatePantsVisuals();
            UpdateShirtVisuals();
            skinToneSlider.value = GetSkinToneValue(characterRenderer.color);
        }
        else
        {
            Debug.LogError($"Failed to load customization data from: {fileName}");
        }
    }
    
    // Вспомогательная функция для определения значения слайдера по цвету кожи
    private float GetSkinToneValue(Color skinColor)
    {
        // Простая реализация - можно улучшить при необходимости
        if (skinColor == lightSkinColor) return 0f;
        if (skinColor == mediumSkinColor) return 0.5f;
        if (skinColor == darkSkinColor) return 1f;
        return 0.5f; // Значение по умолчанию
    }
}