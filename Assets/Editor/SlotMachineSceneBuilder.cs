#if UNITY_EDITOR
using Underpin.Slots;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SlotMachineSceneBuilder
{
    private const string Art = "Assets/Art/";

    [MenuItem("Underpin/Create Slot Machine Demo Scene")]
    public static void CreateScene()
    {
        PrepareSprites();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var canvasObject = new GameObject("SlotMachineCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        Sprite background = Load("bg_gradient.png");
        Sprite cabinet = Load("slot-machine4.png");
        Sprite[] symbols = {
            Load("slot-symbol1.png"), Load("slot-symbol2.png"),
            Load("slot-symbol3.png"), Load("slot-symbol4.png")
        };

        Image bg = MakeImage("Background", canvasObject.transform, background, Vector2.zero, new Vector2(1920, 1080));
        bg.rectTransform.anchorMin = Vector2.zero;
        bg.rectTransform.anchorMax = Vector2.one;
        bg.rectTransform.offsetMin = Vector2.zero;
        bg.rectTransform.offsetMax = Vector2.zero;

        Text title = MakeText("GameTitle", canvasObject.transform, "LUCKY REELS", 64, new Vector2(0, 430), new Vector2(800, 90));
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.82f, 0.25f);

        Image machine = MakeImage("SlotMachineCabinet", canvasObject.transform, cabinet, new Vector2(0, 35), new Vector2(1040, 795));
        machine.preserveAspect = true;

        // Transparent symbol images are layered over the three cabinet windows.
        Image[] reelImages = new Image[3];
        float[] xPositions = { -145f, 0f, 145f };
        for (int i = 0; i < 3; i++)
        {
            reelImages[i] = MakeImage("ReelSymbol" + (i + 1), canvasObject.transform, symbols[i], new Vector2(xPositions[i], 155), new Vector2(132, 132));
            reelImages[i].preserveAspect = true;
        }

        Text balance = MakeText("BalanceText", canvasObject.transform, "CREDITS  500", 34, new Vector2(-280, -360), new Vector2(420, 70));
        balance.color = Color.white;
        Text bet = MakeText("BetText", canvasObject.transform, "BET  5", 34, new Vector2(280, -360), new Vector2(300, 70));
        bet.color = Color.white;

        Text result = MakeText("ResultText", canvasObject.transform, "MATCH ALL THREE SYMBOLS TO WIN!", 30, new Vector2(0, -270), new Vector2(1100, 70));
        result.color = new Color(1f, 0.88f, 0.35f);
        result.fontStyle = FontStyle.Bold;

        Button decrease = MakeButton("DecreaseBetButton", canvasObject.transform, "−", new Vector2(-420, -455), new Vector2(100, 76), new Color(0.35f, 0.19f, 0.65f));
        Button spin = MakeButton("SpinButton", canvasObject.transform, "SPIN", new Vector2(0, -455), new Vector2(300, 90), new Color(0.85f, 0.48f, 0.08f));
        Button increase = MakeButton("IncreaseBetButton", canvasObject.transform, "+", new Vector2(420, -455), new Vector2(100, 76), new Color(0.35f, 0.19f, 0.65f));

        GameObject managerObject = new GameObject("SlotGameManager");
        SlotGameManager manager = managerObject.AddComponent<SlotGameManager>();
        SerializedObject so = new SerializedObject(manager);
        so.FindProperty("reels").arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            GameObject reelObject = new GameObject("ReelController" + (i + 1));
            SlotReel reel = reelObject.AddComponent<SlotReel>();
            so.FindProperty("reels").GetArrayElementAtIndex(i).objectReferenceValue = reel;
        }
        so.FindProperty("symbols").arraySize = symbols.Length;
        for (int i = 0; i < symbols.Length; i++)
            so.FindProperty("symbols").GetArrayElementAtIndex(i).objectReferenceValue = symbols[i];
        so.FindProperty("balanceText").objectReferenceValue = balance;
        so.FindProperty("betText").objectReferenceValue = bet;
        so.FindProperty("resultText").objectReferenceValue = result;
        so.FindProperty("spinButton").objectReferenceValue = spin;
        so.FindProperty("increaseBetButton").objectReferenceValue = increase;
        so.FindProperty("decreaseBetButton").objectReferenceValue = decrease;
        so.ApplyModifiedPropertiesWithoutUndo();

        if (Object.FindObjectOfType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        string scenePath = "Assets/Scenes/SlotMachine.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        Selection.activeObject = managerObject;
        EditorUtility.DisplayDialog("Slot Machine Scene Created",
            "Demo scene created at Assets/Scenes/SlotMachine.unity. Open it and press Play to test.", "OK");
    }

    private static void PrepareSprites()
    {
        foreach (string file in System.IO.Directory.GetFiles(Art))
        {
            if (!file.EndsWith(".png")) continue;
            string assetPath = file.Replace("\\\\", "/");
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
    }

    private static Sprite Load(string file) => AssetDatabase.LoadAssetAtPath<Sprite>(Art + file);

    private static Image MakeImage(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        return image;
    }

    private static Text MakeText(string name, Transform parent, string value, int fontSize, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        Text text = go.GetComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static Button MakeButton(string name, Transform parent, string label, Vector2 position, Vector2 size, Color color)
    {
        Image image = MakeImage(name, parent, null, position, size);
        image.color = color;
        Button button = image.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        button.colors = colors;
        Text text = MakeText(name + "Label", image.transform, label, label == "SPIN" ? 38 : 46, Vector2.zero, size);
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;
        return button;
    }
}
#endif
