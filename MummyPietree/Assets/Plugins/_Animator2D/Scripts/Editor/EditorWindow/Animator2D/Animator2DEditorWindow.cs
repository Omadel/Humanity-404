using Etienne.Animator2D;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class Animator2DEditorWindow : EditorWindow
    {
        private const string KEYNAMESPACE = "EtienneEditor.Animator2D.";
        private const string KEYANIMATION = KEYNAMESPACE + "targetAnimationGUID";
        private const string KEYTIMELINEVALUE = KEYNAMESPACE + "timelineValue";


        [MenuItem("Window/2D/Animation 2D")] public static void ShowWindow() => GetWindow<Animator2DEditorWindow>("Animator2DEditorWindow");

        #region DEBUG
        //todo Debug, to reload the window faster 
        [Shortcut("Animation 2D/Refresh Window", typeof(Animator2DEditorWindow), KeyCode.U)]
        private static async void Refresh()
        {
            Animator2DEditorWindow wnd = GetWindow<Animator2DEditorWindow>();
            wnd.rootVisualElement.Clear();
            wnd.isEnabled = false;
            wnd.isPlaying = false;
            await Task.Delay(100);
            wnd.isEnabled = true;
            wnd.CreateGUI();
        }
        [Shortcut("Animation 2D/Delete Keys Window", typeof(Animator2DEditorWindow), KeyCode.D)]
        private static void DeleteKeys()
        {
            Debug.Log("Delete keys");
            EditorPrefs.DeleteKey(KEYANIMATION);
            EditorPrefs.DeleteKey(KEYTIMELINEVALUE);
        }
        [Shortcut("Animation 2D/Save Window", typeof(Animator2DEditorWindow), KeyCode.S)]
        private static void Save()
        {
            GetWindow<Animator2DEditorWindow>().SaveWindow();
        }
        #endregion


        private Animation2D targetAnimation;
        private Timeline timeline;
        private ToolbarMenu menu;
        private VisualElement inspector;
        private VisualElement preview;
        private Editor editor = null;
        private Dictionary<string, (string name, string guid)> animations;
        private bool wasProGui;
        private bool isPlaying;
        private bool isEnabled;

        public void CreateGUI()
        {
            editor = null;
            animations = new Dictionary<string, (string name, string guid)>();
            targetAnimation = null;

            //find and populate the window
            VisualElement root = rootVisualElement;
            string treePath = $"{AnimatorPath.EditorPath}EditorWindow/Animator2D/Animator2DEditorWindow.uxml";
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treePath);
            TemplateContainer child = visualTree.Instantiate();
            child.StretchToParentSize();
            root.Add(child);

            //fetch the child elements
            menu = child.Q<ToolbarMenu>();
            inspector = child.Q<VisualElement>("Inspector");
            preview = child.Q<VisualElement>("Preview");
            timeline = child.Q<Timeline>();
            timeline.OnSelectedSpriteChanged += ChangeSprite;

            TimelineButtonHolder holder = child.Q<TimelineButtonHolder>();
            holder.FirstButton.clicked += timeline.FirstFrame;
            holder.PreviousButton.clicked += timeline.PreviousFrame;
            holder.PlayButton.clicked += TogglePlay;
            holder.NextButton.clicked += timeline.NextFrame;
            holder.LastButton.clicked += timeline.LastFrame;

            //setup the animation Menu
            SetupMenu<Animation2D>(menu.menu);
            LoadWindow();
            StartAsyncUpdate();
        }

        private void TogglePlay()
        {
            if (targetAnimation == null) return;
            isPlaying = !isPlaying;
            //if (isPlaying) playButton.AddToClassList("pause");
            //else playButton.RemoveFromClassList("pause");
        }

        //todo no magic numbers
        private async void StartAsyncUpdate()
        {
            Debug.Log($"Start play plaing{isPlaying} enabled {isEnabled}");
            while (isEnabled)
            {
                while (!isPlaying)
                {
                    if (!isEnabled) return;
                    await Task.Delay(100);
                }
                await Task.Delay(100);
                timeline.NextFrame();
            }
            Debug.Log("Stopped");
        }

        /// <summary>
        /// Fetch all assets of type T and separate the uppercases with a '/' for sub-menus with <seealso cref="AddCharBeforeUppercase(string, char)"/>
        /// </summary>
        private void SetupMenu<T>(DropdownMenu menu)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (int i = 0; i < guids.Length; i++)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guids[i]));
                name = AddCharBeforeUppercase(name, '/');
                (string name, string guid) animation = (name, guids[i]);
                animations.Add(animation.guid, animation);
                menu.AppendAction(animation.name, MenuSelect, MenuStatusChange, animation.guid);
            }
        }
        /// <summary>Changes the preview background image as a Sprite</summary>
        private void ChangeSprite(Sprite sprite) => preview.style.backgroundImage = new StyleBackground(sprite);
        /// <summary>Set arg status to <see cref="DropdownMenuAction.Status.Normal"/></summary>
        /// <returns><see cref="DropdownMenuAction.Status.Normal"/></returns>
        private DropdownMenuAction.Status MenuStatusChange(DropdownMenuAction arg) => DropdownMenuAction.Status.Normal;
        /// <summary>
        /// Load asset from the <see cref="AssetDatabase"/> depending of <paramref name="obj"/>.userdata as a <see cref="string"/>, (<see cref="GUID"/>)
        /// </summary>
        /// <param name="obj"/>
        private void MenuSelect(DropdownMenuAction obj) => SetTargetAnimation(AssetDatabase.LoadAssetAtPath<Animation2D>(AssetDatabase.GUIDToAssetPath((string)obj.userData)));

        private void SetTargetAnimation(Animation2D animation) => SetTargetAnimation(animation, null);
        private void SetTargetAnimation(Animation2D animation, float? percentValue = null)
        {
            targetAnimation = animation;
            menu.text = animation.name;
            timeline.SetAnimation(animation, percentValue);
            ChangeSprite(timeline.SelectedSprite);
            Editor.CreateCachedEditor(animation, null, ref editor);
        }

        private void SelectSelectedAnimation()
        {
            if (!(Selection.activeObject is Animation2D animation)) return;
            SetTargetAnimation(animation);
        }

        //todo change
        #region GUI TO CHANGE
        private Texture2D checker;
        private float pixelPerUnit;
        //todo redraw animation editor
        [Obsolete]
        /// <summary>Temporary, draws the background of the window</summary>
        private void OnGUI()
        {
            if (checker == null || wasProGui != EditorGUIUtility.isProSkin)
            {
                wasProGui = EditorGUIUtility.isProSkin;
                checker = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/_Animator2D/Scripts/Editor/Editor Default Resources/{(EditorGUIUtility.isProSkin ? "d_" : "")}Checker.png");
            }
            if (preview == null) return;

            Rect rect = preview.worldBound;
            rect.y = 0;

            if (targetAnimation == null || inspector == null || editor == null)
            {
                GUI.Label(rect, new GUIContent("No Animation 2D Selected"), EditorStyles.centeredGreyMiniLabel);
                return;
            }

            Sprite sprite = preview.style.backgroundImage.value.sprite;
            if (sprite != null) pixelPerUnit = sprite.pixelsPerUnit;
            GUI.DrawTextureWithTexCoords(rect, checker, new Rect(0, 0, rect.width / pixelPerUnit, rect.height / pixelPerUnit));

            BeginWindows();
            GUI.Window(0, inspector.layout, DoWindow, "");
            EndWindows();
        }
        [Obsolete]
        /// <summary>Temporary, draws the window editor of the animation</summary>
        private void DoWindow(int id)
        {
            GUI.FocusWindow(id);
            editor.OnInspectorGUI();
        }
        #endregion

        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += SaveWindow;
            EditorApplication.quitting += SaveWindow;
            isEnabled = true;
            Selection.selectionChanged += SelectSelectedAnimation;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= SaveWindow;
            EditorApplication.quitting -= SaveWindow;
            isEnabled = false;
            Selection.selectionChanged -= SelectSelectedAnimation;
        }

        private void SaveWindow()
        {
            isEnabled = false;
            if (targetAnimation == null)
            {
                EditorPrefs.DeleteKey(KEYANIMATION);
                EditorPrefs.DeleteKey(KEYTIMELINEVALUE);
                return;
            }
            EditorPrefs.SetString(KEYANIMATION, AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(targetAnimation)).ToString());
            EditorPrefs.SetFloat(KEYTIMELINEVALUE, timeline.Value);
        }

        private void LoadWindow()
        {
            string animPath = EditorPrefs.GetString(KEYANIMATION, string.Empty);
            float timelineValue = EditorPrefs.GetFloat(KEYTIMELINEVALUE, 0f);
            Animation2D animation = null;
            if (animPath != string.Empty)
            {
                animation = AssetDatabase.LoadAssetAtPath<Animation2D>(AssetDatabase.GUIDToAssetPath(animPath));
                if (animation != null)
                {
                    SetTargetAnimation(animation, timelineValue);
                }
            }
        }

        private string AddCharBeforeUppercase(string text, char c = ' ')
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ') newText.Append(c);
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }

    public class SplitView : TwoPaneSplitView { public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { } }
}
