using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BezierSurcuitUtitlity
{
    [CustomEditor(typeof(Curcuit))]
    public class CurcuitEditor : Editor
    {
        public const float handleSize = 0.06f;

        public const float maxDistanceToPoint = 0.125f;

        public const float maxDistanceToCurve = 0.15f;

        private Color normalColor = Color.white;

        private Color bezierCurveColor = Color.green;

        private Color selectedBezierCurveColor = Color.yellow;

        private Color selectedBezierPointColor = Color.yellow;

        private Color bezierAnchorPointColor = Color.red;

        private Color selectedBezierPointInspectorColor = new Color(18f / 255f, 126f / 255f, 220f / 255f, 1f);

        public Curcuit TargetCurcuit
        {
            get; private set;
        }

        public MonoScript Script
        {
            get; private set;
        }

        public Transform HandleTransform
        {
            get; private set;
        }

        public Quaternion HandleRotation
        {
            get; private set;
        }

        public Vector2Int SelectedSegment
        {
            get; set;
        } = new Vector2Int(-1, -1);

        public BezierPoint SelectedBezierPoint
        {
            get; set;
        } = null;

        public Color NormalColor
        {
            get
            {
                return normalColor;
            }
        }

        public Color BezierCurveColor => bezierCurveColor;

        public Color SelectedBezierCurveColor => selectedBezierCurveColor;

        public Color SelectedBezierPointColor => selectedBezierPointColor;

        public Color BezierAnchorPointColor => bezierAnchorPointColor;

        public Color SelectedBezierPointInspectorColor => selectedBezierPointInspectorColor;

        private void OnEnable()
        {
            TargetCurcuit = target as Curcuit;
            HandleTransform = TargetCurcuit.transform;
            Script = MonoScript.FromMonoBehaviour(TargetCurcuit);
        }

        private void OnSceneGUI()
        {
            HandleRotation = Tools.pivotRotation == PivotRotation.Local ?
            HandleTransform.rotation : Quaternion.identity;

            CurcuitEditerInputHandler.HandleInput(this);

            CurcuitEditorDrawer.HandleDrawing(this);

            PreventDeselectionIfCurveSelected();
        }

        private void PreventDeselectionIfCurveSelected()
        {
            if (SelectedSegment != new Vector2Int(-1, -1))
            {
                Selection.activeGameObject = TargetCurcuit.gameObject;
            }
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", Script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "<b>Curcuit</b>",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, richText = true },
                GUILayout.ExpandWidth(true));

            EditorGUILayout.Space();

            TargetCurcuit.Path.isExpanded = EditorGUILayout.Foldout(TargetCurcuit.Path.isExpanded, "Path", true);
            if (TargetCurcuit.Path.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                TargetCurcuit.Path.IsCyclic = EditorGUILayout.Toggle(
                "Is path cyclic: ",
                TargetCurcuit.Path.IsCyclic);

                EditorGUILayout.Space();

                EditorGUI.indentLevel += 1;
                for (var i = 0; i < TargetCurcuit.Path.Count; i++)
                {
                    bool isSelected = SelectedBezierPoint == TargetCurcuit.Path[i];

                    GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

                    if (isSelected)
                    {
                        foldoutStyle.fontStyle = FontStyle.Bold;
                        foldoutStyle.normal.textColor = selectedBezierPointInspectorColor;
                        foldoutStyle.onNormal.textColor = selectedBezierPointInspectorColor;
                    }

                    TargetCurcuit.Path[i].isExpanded = EditorGUILayout.Foldout(TargetCurcuit.Path[i].isExpanded, $"Bezier point {i}", true, foldoutStyle);

                    if (TargetCurcuit.Path[i].isExpanded)
                    {
                        EditorGUI.indentLevel += 1;

                        TargetCurcuit.Path[i].ControlPointMode
                            = (BezierControlPointMode)EditorGUILayout.EnumPopup("Control point mode", TargetCurcuit.Path[i].ControlPointMode);

                        TargetCurcuit.Path[i].Anchor = EditorGUILayout.Vector2Field("Anchor point", TargetCurcuit.Path[i].Anchor);
                        TargetCurcuit.Path[i].ControlPoint1 = EditorGUILayout.Vector2Field("Control point 1", TargetCurcuit.Path[i].ControlPoint1);
                        TargetCurcuit.Path[i].ControlPoint2 = EditorGUILayout.Vector2Field("Control point 2", TargetCurcuit.Path[i].ControlPoint2);

                        EditorGUI.indentLevel -= 1;
                    }
                }
                EditorGUI.indentLevel -= 1;

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.Space();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(TargetCurcuit);
                EditorSceneManager.MarkSceneDirty(TargetCurcuit.gameObject.scene);
            }
        }
#endif
    }
}
