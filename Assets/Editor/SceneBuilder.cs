using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ChessWorld.Core;
using ChessWorld.Game;
using ChessWorld.Game.UI;

namespace ChessWorld.Editor
{
    /// <summary>
    /// Editor-only one-shot builder that produces placeholder art, the SpriteCatalog
    /// asset, and the Main.unity scene with full hierarchy + wired components.
    /// Invoke from CLI with -executeMethod ChessWorld.Editor.SceneBuilder.BuildAll.
    /// </summary>
    public static class SceneBuilder
    {
        private const string PlaceholderDir = "Assets/Art/Placeholders";
        private const string ArtDir = "Assets/Art";
        private const string ScenesDir = "Assets/Scenes";
        private const string CatalogPath = "Assets/Art/ProjectSpriteCatalog.asset";
        private const string ScenePath = "Assets/Scenes/Main.unity";

        public static void BuildAll()
        {
            try
            {
                EnsureDirectory(ArtDir);
                EnsureDirectory(PlaceholderDir);
                EnsureDirectory(ScenesDir);

                CreatePlaceholderSprites();
                AssetDatabase.Refresh();

                var catalog = CreateCatalog();
                BuildScene(catalog);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("SceneBuilder.BuildAll: complete.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SceneBuilder.BuildAll FAILED: {e}");
                EditorApplication.Exit(1);
            }
        }

        // -----------------------------------------------------------------
        // Sprites
        // -----------------------------------------------------------------

        private static void CreatePlaceholderSprites()
        {
            var whitePiece = new Color32(230, 230, 230, 255);
            var blackPiece = new Color32(30, 30, 30, 255);

            string[] whiteNames = { "W_King", "W_Queen", "W_Rook", "W_Bishop", "W_Knight", "W_Pawn" };
            foreach (var n in whiteNames)
                WriteSolidSquarePng($"{PlaceholderDir}/{n}.png", whitePiece);

            string[] blackNames = { "B_King", "B_Queen", "B_Rook", "B_Bishop", "B_Knight", "B_Pawn" };
            foreach (var n in blackNames)
                WriteSolidSquarePng($"{PlaceholderDir}/{n}.png", blackPiece);

            WriteSolidSquarePng($"{PlaceholderDir}/LightSquare.png", new Color32(220, 220, 220, 255));
            WriteSolidSquarePng($"{PlaceholderDir}/DarkSquare.png", new Color32(60, 60, 60, 255));
            WriteSolidSquarePng($"{PlaceholderDir}/HighlightSquare.png", new Color32(255, 255, 255, 255));
            WriteCirclePng($"{PlaceholderDir}/LegalMoveDot.png", 24);
        }

        private static void WriteSolidSquarePng(string path, Color32 color)
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: false);
            var pixels = new Color32[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels32(pixels);
            tex.Apply();

            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            ConfigureSpriteImporter(path);
        }

        private static void WriteCirclePng(string path, int radius)
        {
            const int size = 64;
            float cx = size / 2f - 0.5f;
            float cy = size / 2f - 0.5f;
            float r2 = radius * radius;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: false);
            var pixels = new Color32[size * size];
            var white = new Color32(255, 255, 255, 255);
            var clear = new Color32(0, 0, 0, 0);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    pixels[y * size + x] = (dx * dx + dy * dy) <= r2 ? white : clear;
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();

            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            ConfigureSpriteImporter(path);
        }

        private static void ConfigureSpriteImporter(string path)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning($"SceneBuilder: TextureImporter unavailable for {path}");
                return;
            }
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        // -----------------------------------------------------------------
        // Catalog
        // -----------------------------------------------------------------

        private static SpriteCatalog CreateCatalog()
        {
            var existing = AssetDatabase.LoadAssetAtPath<SpriteCatalog>(CatalogPath);
            if (existing != null) AssetDatabase.DeleteAsset(CatalogPath);

            var catalog = ScriptableObject.CreateInstance<SpriteCatalog>();

            Sprite Load(string name) =>
                AssetDatabase.LoadAssetAtPath<Sprite>($"{PlaceholderDir}/{name}.png");

            catalog.PieceSprites = new[]
            {
                new SpriteCatalog.Entry { Side = Side.White, Type = PieceType.King,   Sprite = Load("W_King") },
                new SpriteCatalog.Entry { Side = Side.White, Type = PieceType.Queen,  Sprite = Load("W_Queen") },
                new SpriteCatalog.Entry { Side = Side.White, Type = PieceType.Rook,   Sprite = Load("W_Rook") },
                new SpriteCatalog.Entry { Side = Side.White, Type = PieceType.Bishop, Sprite = Load("W_Bishop") },
                new SpriteCatalog.Entry { Side = Side.White, Type = PieceType.Knight, Sprite = Load("W_Knight") },
                new SpriteCatalog.Entry { Side = Side.White, Type = PieceType.Pawn,   Sprite = Load("W_Pawn") },
                new SpriteCatalog.Entry { Side = Side.Black, Type = PieceType.King,   Sprite = Load("B_King") },
                new SpriteCatalog.Entry { Side = Side.Black, Type = PieceType.Queen,  Sprite = Load("B_Queen") },
                new SpriteCatalog.Entry { Side = Side.Black, Type = PieceType.Rook,   Sprite = Load("B_Rook") },
                new SpriteCatalog.Entry { Side = Side.Black, Type = PieceType.Bishop, Sprite = Load("B_Bishop") },
                new SpriteCatalog.Entry { Side = Side.Black, Type = PieceType.Knight, Sprite = Load("B_Knight") },
                new SpriteCatalog.Entry { Side = Side.Black, Type = PieceType.Pawn,   Sprite = Load("B_Pawn") },
            };

            catalog.LightSquare = Load("LightSquare");
            catalog.DarkSquare = Load("DarkSquare");
            catalog.HighlightSquare = Load("HighlightSquare");
            catalog.LegalMoveDot = Load("LegalMoveDot");
            catalog.DecayedBackground = null;
            catalog.FallbackBackground = catalog.LightSquare;

            AssetDatabase.CreateAsset(catalog, CatalogPath);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<SpriteCatalog>(CatalogPath);
        }

        // -----------------------------------------------------------------
        // Scene
        // -----------------------------------------------------------------

        private static void BuildScene(SpriteCatalog catalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera ----------------------------------------------------------
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.backgroundColor = new Color(10f / 255f, 10f / 255f, 12f / 255f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            var camFit = camGo.AddComponent<CameraFit>();
            camFit.BoardSize = 8f;
            camFit.MarginSquares = 1.5f;

            // Background ------------------------------------------------------
            var bgGo = new GameObject("Background");
            var bgSr = bgGo.AddComponent<SpriteRenderer>();
            bgSr.sortingOrder = -10;
            var bgView = bgGo.AddComponent<BackgroundView>();
            bgView.Catalog = catalog;

            // Board -----------------------------------------------------------
            var boardGo = new GameObject("BoardRoot");
            var boardView = boardGo.AddComponent<BoardView>();
            boardView.Catalog = catalog;
            boardView.SquareSize = 1f;

            // Pieces root -----------------------------------------------------
            var piecesGo = new GameObject("PiecesRoot");

            // Canvas + UI -----------------------------------------------------
            var canvasGo = BuildCanvas(catalog,
                out var turnIndicator,
                out var whiteCaptures,
                out var blackCaptures,
                out var gameOverPanel);

            // Game root -------------------------------------------------------
            var gameGo = new GameObject("GameRoot");
            var input = gameGo.AddComponent<InputHandler>();
            input.Board = boardView;
            input.MainCamera = cam;
            input.Enabled = true;

            var selection = gameGo.AddComponent<SelectionController>();
            selection.HumanSide = Side.White;
            selection.Enabled = true;

            var controller = gameGo.AddComponent<GameController>();
            controller.Catalog = catalog;
            controller.Board = boardView;
            controller.Background = bgView;
            controller.Input = input;
            controller.Selection = selection;
            controller.TurnIndicator = turnIndicator;
            controller.WhiteCaptures = whiteCaptures;
            controller.BlackCaptures = blackCaptures;
            controller.GameOverPanel = gameOverPanel;
            controller.PiecesRoot = piecesGo.transform;
            controller.AiDepth = 3;

            // Main thread dispatcher (used by GameController for AI callback) -
            var dispatcherGo = new GameObject("MainThreadDispatcher");
            dispatcherGo.AddComponent<UnityMainThreadDispatcher>();

            // EventSystem -----------------------------------------------------
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();

            // Save ------------------------------------------------------------
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }

        // -----------------------------------------------------------------
        // Canvas + UI
        // -----------------------------------------------------------------

        private static GameObject BuildCanvas(SpriteCatalog catalog,
            out TurnIndicator turnIndicator,
            out CapturedTray whiteCaptures,
            out CapturedTray blackCaptures,
            out GameOverPanel gameOverPanel)
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var font = LoadFont();

            // Turn indicator --------------------------------------------------
            var turnRoot = new GameObject("TurnIndicatorRoot", typeof(RectTransform));
            turnRoot.transform.SetParent(canvasGo.transform, false);
            var turnRt = (RectTransform)turnRoot.transform;
            turnRt.anchorMin = new Vector2(0.5f, 1f);
            turnRt.anchorMax = new Vector2(0.5f, 1f);
            turnRt.pivot = new Vector2(0.5f, 1f);
            turnRt.anchoredPosition = new Vector2(0f, -80f);
            turnRt.sizeDelta = new Vector2(600f, 100f);

            var turnText = MakeUiText(turnRoot.transform, "TurnIndicator",
                "Your turn", 48, font, fillParent: true);
            turnIndicator = turnRoot.AddComponent<TurnIndicator>();
            turnIndicator.Label = turnText;

            // White captures (top-left, displays Black pieces) ---------------
            whiteCaptures = BuildCapturedTray(canvasGo.transform,
                "WhiteCapturesRoot",
                Side.Black,
                anchorMin: new Vector2(0f, 1f),
                anchorMax: new Vector2(0f, 1f),
                pivot: new Vector2(0f, 1f),
                anchoredPos: new Vector2(20f, -200f),
                catalog);

            // Black captures (top-right, displays White pieces) --------------
            blackCaptures = BuildCapturedTray(canvasGo.transform,
                "BlackCapturesRoot",
                Side.White,
                anchorMin: new Vector2(1f, 1f),
                anchorMax: new Vector2(1f, 1f),
                pivot: new Vector2(1f, 1f),
                anchoredPos: new Vector2(-20f, -200f),
                catalog);

            // Game over panel -------------------------------------------------
            gameOverPanel = BuildGameOverPanel(canvasGo.transform, font);

            return canvasGo;
        }

        private static CapturedTray BuildCapturedTray(Transform parent, string name,
            Side displaysSide,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos,
            SpriteCatalog catalog)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var rt = (RectTransform)root.transform;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(400f, 80f);

            var container = new GameObject("Container", typeof(RectTransform));
            container.transform.SetParent(root.transform, false);
            var crt = (RectTransform)container.transform;
            crt.anchorMin = Vector2.zero;
            crt.anchorMax = Vector2.one;
            crt.pivot = new Vector2(0.5f, 0.5f);
            crt.offsetMin = Vector2.zero;
            crt.offsetMax = Vector2.zero;
            var hlg = container.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.spacing = 4f;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            var tray = root.AddComponent<CapturedTray>();
            tray.DisplaysCapturesOfSide = displaysSide;
            tray.IconContainer = container.transform;
            tray.Catalog = catalog;
            return tray;
        }

        private static GameOverPanel BuildGameOverPanel(Transform parent, Font font)
        {
            var panel = new GameObject("GameOverPanelRoot", typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            var rt = (RectTransform)panel.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.6f);

            var headlineGo = new GameObject("Headline", typeof(RectTransform));
            headlineGo.transform.SetParent(panel.transform, false);
            var hrt = (RectTransform)headlineGo.transform;
            hrt.anchorMin = new Vector2(0.5f, 0.5f);
            hrt.anchorMax = new Vector2(0.5f, 0.5f);
            hrt.pivot = new Vector2(0.5f, 0.5f);
            hrt.anchoredPosition = new Vector2(0f, 200f);
            hrt.sizeDelta = new Vector2(900f, 200f);
            var headlineText = headlineGo.AddComponent<Text>();
            headlineText.text = "Headline";
            headlineText.fontSize = 64;
            headlineText.alignment = TextAnchor.MiddleCenter;
            headlineText.color = Color.white;
            headlineText.font = font;
            headlineText.horizontalOverflow = HorizontalWrapMode.Wrap;
            headlineText.verticalOverflow = VerticalWrapMode.Overflow;

            var creditsGo = new GameObject("Credits", typeof(RectTransform));
            creditsGo.transform.SetParent(panel.transform, false);
            var crt = (RectTransform)creditsGo.transform;
            crt.anchorMin = new Vector2(0.5f, 0.5f);
            crt.anchorMax = new Vector2(0.5f, 0.5f);
            crt.pivot = new Vector2(0.5f, 0.5f);
            crt.anchoredPosition = new Vector2(0f, 0f);
            crt.sizeDelta = new Vector2(900f, 100f);
            var creditsText = creditsGo.AddComponent<Text>();
            creditsText.text = "Background art by Rehaan Rashid";
            creditsText.fontSize = 24;
            creditsText.alignment = TextAnchor.MiddleCenter;
            creditsText.color = Color.white;
            creditsText.font = font;
            creditsText.horizontalOverflow = HorizontalWrapMode.Wrap;

            var buttonGo = new GameObject("Restart", typeof(RectTransform));
            buttonGo.transform.SetParent(panel.transform, false);
            var brt = (RectTransform)buttonGo.transform;
            brt.anchorMin = new Vector2(0.5f, 0.5f);
            brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.pivot = new Vector2(0.5f, 0.5f);
            brt.anchoredPosition = new Vector2(0f, -200f);
            brt.sizeDelta = new Vector2(360f, 120f);
            var buttonImg = buttonGo.AddComponent<Image>();
            buttonImg.color = new Color(0.2f, 0.2f, 0.25f, 1f);
            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = buttonImg;

            var buttonLabelGo = new GameObject("Text", typeof(RectTransform));
            buttonLabelGo.transform.SetParent(buttonGo.transform, false);
            var blrt = (RectTransform)buttonLabelGo.transform;
            blrt.anchorMin = Vector2.zero;
            blrt.anchorMax = Vector2.one;
            blrt.pivot = new Vector2(0.5f, 0.5f);
            blrt.offsetMin = Vector2.zero;
            blrt.offsetMax = Vector2.zero;
            var buttonLabel = buttonLabelGo.AddComponent<Text>();
            buttonLabel.text = "Restart";
            buttonLabel.fontSize = 36;
            buttonLabel.alignment = TextAnchor.MiddleCenter;
            buttonLabel.color = Color.white;
            buttonLabel.font = font;

            var script = panel.AddComponent<GameOverPanel>();
            script.Root = panel;
            script.Headline = headlineText;
            script.Credits = creditsText;
            script.RestartButton = button;

            panel.SetActive(false);
            return script;
        }

        private static Text MakeUiText(Transform parent, string name, string text,
            int fontSize, Font font, bool fillParent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            if (fillParent)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            var t = go.AddComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.font = font;
            return t;
        }

        private static Font LoadFont()
        {
            // Unity 6 ships LegacyRuntime.ttf (the renamed Arial).
            var f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f != null) return f;
            // Older Unity versions exposed Arial.ttf.
            f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return f;
        }

        // -----------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------

        private static void EnsureDirectory(string assetPath)
        {
            // assetPath is "Assets/..." — convert to absolute and Directory.CreateDirectory.
            var fullPath = Path.GetFullPath(assetPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }
    }
}
