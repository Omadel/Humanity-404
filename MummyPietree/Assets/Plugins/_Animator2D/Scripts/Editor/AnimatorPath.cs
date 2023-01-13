using UnityEditor;

namespace EtienneEditor.Animator2D
{
    internal static class AnimatorPath
    {
        private const string EDITORDEFAULTRESOURCES = "Editor Default Resources/";
        private const string FILTER = "Animator2DEditorWindow t:VisualTreeAsset";

        public static string EditorPath => editorPath ??= FindEditorPath();
        public static string editorPath;
        public static string EditorDefaultResourcesPath => editorDefaultResourcesPath ??= EditorPath + EDITORDEFAULTRESOURCES;
        public static string editorDefaultResourcesPath;

        private static string FindEditorPath()
        {
            string windowAssetGUID = AssetDatabase.FindAssets(FILTER, new string[] { "Packages", "Assets" })[0];
            string assetPath = AssetDatabase.GUIDToAssetPath(windowAssetGUID);
            int subFolderCount = 3;
            string directoryPath = assetPath;
            for (int i = 0; i < subFolderCount; i++)
            {
                directoryPath = System.IO.Path.GetDirectoryName(directoryPath);
            }
            UnityEngine.Debug.Log("Find Editor Path" + System.Environment.NewLine + directoryPath);
            return $"{directoryPath}/";
        }

        internal static T LoadFromEditorResources<T>(string fileName) where T : UnityEngine.Object
        {
            string assetPath = EditorDefaultResourcesPath + fileName;
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}
