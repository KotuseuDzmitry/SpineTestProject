using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            CurcuitEditorInspectorDrawer.DrawInspector(this);
        }
#endif
    }
}
