using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace BezierSurcuitUtitlity
{
    public static class CurcuitEditorInspectorDrawer
    {
        private static CurcuitEditor curcuitEditor;

#if UNITY_EDITOR
        public static void DrawInspector(CurcuitEditor curcuitEditor)
        {
            CurcuitEditorInspectorDrawer.curcuitEditor = curcuitEditor;
            DrawInspectorUI();
        }

        private static void DrawInspectorUI()
        {
            ShowScriptReference();

            EditorGUILayout.Space();

            ShowInspectorTitle();

            EditorGUILayout.Space();

            ShowPathMoveHandleActivationToggle();

            EditorGUILayout.Space();

            ShowCurcuit();

            EditorGUILayout.Space();

            CheckChanges();
        }

        private static void ShowPathMoveHandleActivationToggle()
        {
            curcuitEditor.IsPathMoveHandleVisible = EditorGUILayout.Toggle(
                            "Is selected path move handle visible: ",
                            curcuitEditor.IsPathMoveHandleVisible);
        }

        private static void ShowScriptReference()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", curcuitEditor.Script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }

        private static void ShowInspectorTitle()
        {
            EditorGUILayout.LabelField(
                            "<b>Curcuit</b>",
                            new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, richText = true },
                            GUILayout.ExpandWidth(true));
        }

        private static void ShowCurcuit()
        {
            curcuitEditor.TargetCurcuit.isExpanded = EditorGUILayout.Foldout(curcuitEditor.TargetCurcuit.isExpanded, "Curcuit", true);
            if (curcuitEditor.TargetCurcuit.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                ShowPaths();

                EditorGUI.indentLevel -= 1;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add path",
                GUILayout.Height(25f), GUILayout.Width(200f)))
            {
                curcuitEditor.AddPathToOrigin();
                return;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void ShowPaths()
        {
            EditorGUI.indentLevel += 1;
            for (var i = 0; i < curcuitEditor.TargetCurcuit.Count; i++)
            {
                ShowPath(i);
            }
            EditorGUI.indentLevel -= 1;
        }

        private static void ShowPath(int index)
        {
            Path pathToShow = curcuitEditor.TargetCurcuit[index];

            bool isSelected = curcuitEditor.SelectedPath == pathToShow;

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

            if (isSelected)
            {
                foldoutStyle.fontStyle = FontStyle.Bold;
                foldoutStyle.normal.textColor = curcuitEditor.SelectedPathInspectorColor;
                foldoutStyle.onNormal.textColor = curcuitEditor.SelectedPathInspectorColor;
            }

            GUILayout.BeginHorizontal();

            pathToShow.isExpanded = EditorGUILayout.Foldout(pathToShow.isExpanded, $"Path {index}", true, foldoutStyle);

            if (GUILayout.Button("X", new GUIStyle(EditorStyles.miniButton) { }, GUILayout.Height(20f), GUILayout.Width(20f)))
            {
                curcuitEditor.RemovePath(index);
                return;
            }

            GUILayout.EndHorizontal();

            if (pathToShow.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                pathToShow.IsCyclic = EditorGUILayout.Toggle(
                "Is path cyclic: ",
                pathToShow.IsCyclic);

                EditorGUILayout.Space();

                ShowPoints(pathToShow);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add new point",
                    GUILayout.Height(25f), GUILayout.Width(200f)))
                {
                    curcuitEditor.AddPointInOrigin(index);
                    return;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                EditorGUI.BeginDisabledGroup(curcuitEditor.SelectedSegment == new Vector2Int(-1, -1));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Insert point into selected curve",
                    GUILayout.Height(25f), GUILayout.Width(200f)))
                {
                    curcuitEditor.InsertPoint();
                    return;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel -= 1;
            }
        }

        private static void ShowPoints(Path path)
        {
            EditorGUI.indentLevel += 1;
            for (var i = 0; i < path.Count; i++)
            {
                ShowPoint(path, i);
            }
            EditorGUI.indentLevel -= 1;
        }

        private static void ShowPoint(Path path, int index)
        {
            bool isSelected = curcuitEditor.SelectedBezierPoint == path[index];

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

            if (isSelected)
            {
                foldoutStyle.fontStyle = FontStyle.Bold;
                foldoutStyle.normal.textColor = curcuitEditor.SelectedBezierPointInspectorColor;
                foldoutStyle.onNormal.textColor = curcuitEditor.SelectedBezierPointInspectorColor;
            }

            EditorGUILayout.BeginHorizontal();

            path[index].isExpanded
                = EditorGUILayout.Foldout(path[index].isExpanded, $"Bezier point {index}", true, foldoutStyle);

            if (GUILayout.Button("X", new GUIStyle(EditorStyles.miniButton) { }, GUILayout.Height(20f), GUILayout.Width(20f)))
            {
                curcuitEditor.RemovePoint(path, index);
                return;
            }

            EditorGUILayout.EndHorizontal();

            if (path[index].isExpanded)
            {
                EditorGUI.indentLevel += 1;

                path[index].ControlPointMode
                    = (BezierControlPointMode)EditorGUILayout.EnumPopup("Control point mode", path[index].ControlPointMode);

                path[index].Anchor
                    = EditorGUILayout.Vector2Field("Anchor point", path[index].Anchor);
                path[index].ControlPoint1
                    = EditorGUILayout.Vector2Field("Control point 1", path[index].ControlPoint1);
                path[index].ControlPoint2
                    = EditorGUILayout.Vector2Field("Control point 2", path[index].ControlPoint2);

                EditorGUI.indentLevel -= 1;
            }
        }

        private static void CheckChanges()
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(curcuitEditor.TargetCurcuit);
                EditorSceneManager.MarkSceneDirty(curcuitEditor.TargetCurcuit.gameObject.scene);
            }
        }
#endif
    }
}
