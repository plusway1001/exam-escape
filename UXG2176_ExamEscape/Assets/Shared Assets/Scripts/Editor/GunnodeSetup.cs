using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gunnode
{
    // one-click setup for the shared screens: Tools > Gunnode in Unity's menu bar.
    // it only links things that already exist (the prefabs and scripts), so it's safe to run again.
    // editor-only: it sits in an Editor folder, so it never goes into the build.
    public static class GunnodeSetup
    {
        private const string PrefabFolder = "Assets/Shared Assets/Prefab";
        private const string GameScreensPath = PrefabFolder + "/GameScreens.prefab";
        private const string MenuScenePath = "Assets/Scenes/Main.unity";

        // ---------- 1. the shared GameScreens prefab ----------

        [MenuItem("Tools/Gunnode/1. Build GameScreens prefab")]
        public static void BuildGameScreens()
        {
            GameObject pausePrefab = LoadPrefab("pauseMenu", true);
            GameObject confirmPrefab = LoadPrefab("confirmMenu", true);
            GameObject gameOverPrefab = LoadPrefab("gameOverMenu", true);
            GameObject winPrefab = LoadPrefab("winMenu", false);
            if (pausePrefab == null || confirmPrefab == null || gameOverPrefab == null) return;

            // give each panel prefab the components the scripts need
            PreparePanel(pausePrefab, "resumeBtn");
            PreparePanel(gameOverPrefab, "restartBtn");
            if (winPrefab != null)
                PreparePanel(winPrefab, "quitBtn");
            PrepareConfirm(confirmPrefab);

            // the canvas everything sits on. sort order 10 so it draws over any level's HUD
            GameObject root = new GameObject("GameScreens", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // level progress: always visible, so it's the one child that stays switched on
            GameObject progressPrefab = LoadPrefab("LevelProgress", false);
            if (progressPrefab != null)
            {
                PrepareLevelProgress(progressPrefab);
                GameObject progress = (GameObject)PrefabUtility.InstantiatePrefab(progressPrefab, root.transform);
                progress.SetActive(true);
            }

            // pause: the PauseMenu object stays on, the pause panel and confirm box under it start hidden
            PauseMenu pause = MakeHolder<PauseMenu>(root.transform, "PauseMenu");
            GameObject pausePanel = Spawn(pausePrefab, pause.transform);
            GameObject confirm = Spawn(confirmPrefab, pause.transform);
            SetObject(pause, "panel", pausePanel.GetComponent<UIPanel>());
            SetObject(pause, "confirm", confirm.GetComponent<ConfirmDialog>());
            Wire(pausePanel, "resumeBtn", pause.OnResume);
            Wire(pausePanel, "quitBtn", pause.OnMainMenu);

            // game over
            EndScreen lose = MakeHolder<EndScreen>(root.transform, "GameOver");
            GameObject losePanel = Spawn(gameOverPrefab, lose.transform);
            SetObject(lose, "panel", losePanel.GetComponent<UIPanel>());
            SetBool(lose, "isWinScreen", false);
            Wire(losePanel, "restartBtn", lose.OnRestart);
            Wire(losePanel, "quitBtn", lose.OnMainMenu);

            // win
            EndScreen win = MakeHolder<EndScreen>(root.transform, "Win");
            SetBool(win, "isWinScreen", true);
            if (winPrefab != null)
            {
                GameObject winPanel = Spawn(winPrefab, win.transform);
                SetObject(win, "panel", winPanel.GetComponent<UIPanel>());
                Wire(winPanel, "quitBtn", win.OnMainMenu);
            }
            else
            {
                Debug.LogWarning("Gunnode setup: there's no winMenu prefab in " + PrefabFolder +
                    " yet, so the Win screen is empty. Make one (its button named quitBtn) and run this again.");
            }

            PrefabUtility.SaveAsPrefabAsset(root, GameScreensPath);
            Object.DestroyImmediate(root);
            Debug.Log("Gunnode setup: saved " + GameScreensPath + ". Drop it into any level.");
        }

        // ---------- 2. put it in the open level ----------

        [MenuItem("Tools/Gunnode/2. Add GameScreens to the open scene")]
        public static void AddToScene()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameScreensPath);
            if (prefab == null)
            {
                Debug.LogWarning("Gunnode setup: run '1. Build GameScreens prefab' first.");
                return;
            }

            // the menu isn't a level, it has no pause or end screens
            if (SceneManager.GetActiveScene().path == MenuScenePath)
            {
                Debug.LogWarning("Gunnode setup: this is the main menu. Open a level scene (like Caijie) and run step 2 there.");
                return;
            }

            if (Object.FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include) == null)
            {
                GameObject screens = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(screens, "Add GameScreens");
            }
            else
            {
                Debug.Log("Gunnode setup: this scene already has GameScreens.");
            }

            // buttons need exactly one EventSystem in the scene
            if (Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include) == null)
            {
                GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                Undo.RegisterCreatedObjectUndo(es, "Add EventSystem");
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Gunnode setup: GameScreens is in this scene. Save the scene (Ctrl+S).");
        }

        // ---------- 3. the main menu, GameFlow and the scene list ----------

        [MenuItem("Tools/Gunnode/3. Link main menu and scene list")]
        public static void LinkMainMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            if (!File.Exists(MenuScenePath))
            {
                Debug.LogWarning("Gunnode setup: can't find " + MenuScenePath);
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(MenuScenePath, OpenSceneMode.Single);

            // a GameScreens that ended up in the menu by mistake: take it out (with anything linked to it)
            // only the GameScreens prefab copy itself, never the menu's own canvas around it
            foreach (PauseMenu stray in Object.FindObjectsByType<PauseMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                GameObject strayRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(stray);
                if (strayRoot == null || strayRoot.GetComponent<Canvas>() == null || FindDeep(strayRoot.transform, "startBtn") != null)
                {
                    Debug.LogWarning("Gunnode setup: found a PauseMenu in the menu scene but couldn't safely remove it. Delete GameScreens by hand.");
                    continue;
                }
                Debug.Log("Gunnode setup: removed " + strayRoot.name + " from the menu scene. It only belongs in levels.");
                Object.DestroyImmediate(strayRoot);
            }

            // the menu's own canvas is the one that holds startBtn
            Canvas canvas = null;
            foreach (Canvas c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (FindDeep(c.transform, "startBtn") != null)
                {
                    canvas = c.rootCanvas;
                    break;
                }
            }
            if (canvas == null)
            {
                Debug.LogWarning("Gunnode setup: no Canvas in the menu scene has a startBtn.");
                return;
            }

            MainMenu menu = canvas.GetComponent<MainMenu>();
            if (menu == null)
                menu = canvas.gameObject.AddComponent<MainMenu>();

            // a button shrunk to scale 0 is still there but invisible, so put it back to normal size
            foreach (string name in new[] { "startBtn", "quitBtn" })
            {
                Transform t = FindDeep(canvas.transform, name);
                if (t != null && t.localScale == Vector3.zero)
                {
                    t.localScale = Vector3.one;
                    Debug.Log("Gunnode setup: " + name + " had scale 0, set it back to 1.");
                }
            }

            Wire(canvas.gameObject, "startBtn", menu.OnStart);
            Wire(canvas.gameObject, "quitBtn", menu.OnQuit);

            // the "exit game?" box: the exitGameMenu prefab inside the menu canvas, last so it draws on top
            ConfirmDialog exitBox = null;
            GameObject exitPrefab = LoadPrefab("exitGameMenu", true);
            if (exitPrefab != null)
            {
                PrepareConfirm(exitPrefab);

                // already in the menu? use that one
                foreach (ConfirmDialog d in canvas.GetComponentsInChildren<ConfirmDialog>(true))
                {
                    if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(d.gameObject) == exitPrefab)
                        exitBox = d;
                }

                if (exitBox == null)
                {
                    GameObject box = Spawn(exitPrefab, canvas.transform);
                    box.transform.SetAsLastSibling();
                    exitBox = box.GetComponent<ConfirmDialog>();
                    Debug.Log("Gunnode setup: added exitGameMenu to the menu.");
                }
            }
            SetObject(menu, "confirm", exitBox);

            // Start and Exit can't be clicked while the box is open
            SerializedObject menuSo = new SerializedObject(menu);
            SerializedProperty buttons = menuSo.FindProperty("menuButtons");
            string[] buttonNames = { "startBtn", "quitBtn" };
            buttons.arraySize = buttonNames.Length;
            for (int i = 0; i < buttonNames.Length; i++)
            {
                Transform t = FindDeep(canvas.transform, buttonNames[i]);
                Button b = null;
                if (t != null)
                    b = t.GetComponent<Button>();
                buttons.GetArrayElementAtIndex(i).objectReferenceValue = b;
            }
            menuSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            // GameFlow: the menu scene's real name, then the scene list in the same order
            GameFlow flow = Resources.Load<GameFlow>("GameFlow");
            if (flow == null)
            {
                Debug.LogWarning("Gunnode setup: no GameFlow asset in a Resources folder.");
                return;
            }

            SerializedObject so = new SerializedObject(flow);
            so.FindProperty("menuScene").stringValue = Path.GetFileNameWithoutExtension(MenuScenePath);
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();

            List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>();
            list.Add(new EditorBuildSettingsScene(MenuScenePath, true));
            foreach (string level in flow.levelScenes)
            {
                string path = FindScenePath(level);
                if (path == null)
                    Debug.LogWarning("Gunnode setup: GameFlow lists '" + level + "' but no scene has that name.");
                else
                    list.Add(new EditorBuildSettingsScene(path, true));
            }
            EditorBuildSettings.scenes = list.ToArray();

            Debug.Log("Gunnode setup: main menu linked, GameFlow.menuScene = Main, scene list has " +
                list.Count + " scenes (menu first, then GameFlow's order).");
        }

        // ---------- helpers ----------

        private static GameObject LoadPrefab(string name, bool required)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabFolder + "/" + name + ".prefab");
            if (prefab == null && required)
                Debug.LogWarning("Gunnode setup: can't find " + PrefabFolder + "/" + name + ".prefab");
            return prefab;
        }

        // edits the prefab asset itself: CanvasGroup + UIPanel on the root, a Button on every *Btn
        private static void PreparePanel(GameObject prefab, string firstSelectedName)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);

            UIPanel panel = contents.GetComponent<UIPanel>();
            if (panel == null)
                panel = contents.AddComponent<UIPanel>();   // RequireComponent adds the CanvasGroup too

            AddMissingButtons(contents.transform);

            Transform first = FindDeep(contents.transform, firstSelectedName);
            if (first != null)
                SetObject(panel, "firstSelected", first.GetComponent<Selectable>());

            PrefabUtility.SaveAsPrefabAsset(contents, path);
            PrefabUtility.UnloadPrefabContents(contents);
        }

        // the LevelProgress prefab: LevelProgress on the root, pointing at its text
        private static void PrepareLevelProgress(GameObject prefab)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);

            LevelProgress progress = contents.GetComponent<LevelProgress>();
            if (progress == null)
                progress = contents.AddComponent<LevelProgress>();

            TMPro.TMP_Text text = contents.GetComponentInChildren<TMPro.TMP_Text>(true);
            SetObject(progress, "progressText", text);

            // it only shows information, so it should never catch clicks meant for the menus
            foreach (Graphic g in contents.GetComponentsInChildren<Graphic>(true))
                g.raycastTarget = false;

            PrefabUtility.SaveAsPrefabAsset(contents, path);
            PrefabUtility.UnloadPrefabContents(contents);
        }

        // the confirm box: ConfirmDialog with its Yes/No buttons, and No selected first
        private static void PrepareConfirm(GameObject prefab)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);

            ConfirmDialog dialog = contents.GetComponent<ConfirmDialog>();
            if (dialog == null)
                dialog = contents.AddComponent<ConfirmDialog>();

            AddMissingButtons(contents.transform);

            Transform yes = FindDeep(contents.transform, "yesButton");
            Transform no = FindDeep(contents.transform, "noButton");
            if (yes != null)
                SetObject(dialog, "yesButton", yes.GetComponent<Button>());
            if (no != null)
            {
                SetObject(dialog, "noButton", no.GetComponent<Button>());
                SetObject(dialog, "firstSelected", no.GetComponent<Selectable>());
            }

            PrefabUtility.SaveAsPrefabAsset(contents, path);
            PrefabUtility.UnloadPrefabContents(contents);
        }

        // image "buttons" (RawImage or Image) get a real Button so OnClick works
        private static void AddMissingButtons(Transform root)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                bool looksLikeButton = t.name.EndsWith("Btn") || t.name.EndsWith("Button");
                if (!looksLikeButton || t.GetComponent<Button>() != null) continue;

                Button button = t.gameObject.AddComponent<Button>();
                button.targetGraphic = t.GetComponent<Graphic>();
            }
        }

        // an empty full-screen object that stays switched on, holding a screen script
        private static T MakeHolder<T>(Transform parent, string name) where T : Component
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return go.AddComponent<T>();
        }

        // a linked copy of a prefab, switched off so it starts hidden
        private static GameObject Spawn(GameObject prefab, Transform parent)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.SetActive(false);
            return go;
        }

        // replaces a button's OnClick with one persistent call, the same as dragging it in the Inspector
        private static void Wire(GameObject root, string buttonName, UnityAction action)
        {
            Transform t = FindDeep(root.transform, buttonName);
            Button button = null;
            if (t != null)
                button = t.GetComponent<Button>();

            if (button == null)
            {
                Debug.LogWarning("Gunnode setup: no button called '" + buttonName + "' under " + root.name);
                return;
            }

            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            UnityEventTools.AddPersistentListener(button.onClick, action);

            if (PrefabUtility.IsPartOfPrefabInstance(button))
                PrefabUtility.RecordPrefabInstancePropertyModifications(button);
            EditorUtility.SetDirty(button);
        }

        private static void SetObject(Object target, string field, Object value)
        {
            SerializedObject so = new SerializedObject(target);
            SerializedProperty prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogWarning("Gunnode setup: " + target.GetType().Name + " has no field '" + field + "'");
                return;
            }
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string field, bool value)
        {
            SerializedObject so = new SerializedObject(target);
            so.FindProperty(field).boolValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Transform FindDeep(Transform root, string name)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == name)
                    return t;
            }
            return null;
        }

        private static string FindScenePath(string sceneName)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Scene " + sceneName))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == sceneName)
                    return path;
            }
            return null;
        }
    }
}
