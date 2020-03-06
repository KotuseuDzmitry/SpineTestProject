using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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

            ShowPathInfo();

            EditorGUILayout.Space();

            CheckChanges();
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

        private static void ShowPathInfo()
        {
            curcuitEditor.TargetCurcuit.Path.isExpanded = EditorGUILayout.Foldout(curcuitEditor.TargetCurcuit.Path.isExpanded, "Path", true);
            if (curcuitEditor.TargetCurcuit.Path.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                curcuitEditor.TargetCurcuit.Path.IsCyclic = EditorGUILayout.Toggle(
                "Is path cyclic: ",
                curcuitEditor.TargetCurcuit.Path.IsCyclic);

                EditorGUILayout.Space();

                ShowPoints();

                EditorGUI.indentLevel -= 1;
            }
        }

        private static void ShowPoints()
        {
            EditorGUI.indentLevel += 1;
            for (var i = 0; i < curcuitEditor.TargetCurcuit.Path.Count; i++)
            {
                ShowPoint(i);
            }
            EditorGUI.indentLevel -= 1;
        }

        private static void ShowPoint(int i)
        {
            bool isSelected = curcuitEditor.SelectedBezierPoint == curcuitEditor.TargetCurcuit.Path[i];

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

            if (isSelected)
            {
                foldoutStyle.fontStyle = FontStyle.Bold;
                foldoutStyle.normal.textColor = curcuitEditor.SelectedBezierPointInspectorColor;
                foldoutStyle.onNormal.textColor = curcuitEditor.SelectedBezierPointInspectorColor;
            }

            curcuitEditor.TargetCurcuit.Path[i].isExpanded
                = EditorGUILayout.Foldout(curcuitEditor.TargetCurcuit.Path[i].isExpanded, $"Bezier point {i}", true, foldoutStyle);

            if (curcuitEditor.TargetCurcuit.Path[i].isExpanded)
            {
                EditorGUI.indentLevel += 1;

                curcuitEditor.TargetCurcuit.Path[i].ControlPointMode
                    = (BezierControlPointMode)EditorGUILayout.EnumPopup("Control point mode", curcuitEditor.TargetCurcuit.Path[i].ControlPointMode);

                curcuitEditor.TargetCurcuit.Path[i].Anchor
                    = EditorGUILayout.Vector2Field("Anchor point", curcuitEditor.TargetCurcuit.Path[i].Anchor);
                curcuitEditor.TargetCurcuit.Path[i].ControlPoint1
                    = EditorGUILayout.Vector2Field("Control point 1", curcuitEditor.TargetCurcuit.Path[i].ControlPoint1);
                curcuitEditor.TargetCurcuit.Path[i].ControlPoint2
                    = EditorGUILayout.Vector2Field("Control point 2", curcuitEditor.TargetCurcuit.Path[i].ControlPoint2);

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
