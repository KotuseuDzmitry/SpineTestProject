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

        public void InsertPoint()
        {
            Undo.RecordObject(TargetCurcuit, "Insert Bezier Point");

            Vector2 midPoint = Bezier.CubicCurve(
                    TargetCurcuit.Path[SelectedSegment.x].Anchor,
                    TargetCurcuit.Path[SelectedSegment.x].ControlPoint2,
                    TargetCurcuit.Path[SelectedSegment.y].ControlPoint1,
                    TargetCurcuit.Path[SelectedSegment.y].Anchor,
                    0.5f
                    );

            Vector2 direction = TargetCurcuit.Path[SelectedSegment.x].Anchor
                - TargetCurcuit.Path[SelectedSegment.y].Anchor;

            BezierPoint newPoint = new BezierPoint(
                midPoint,
                direction
                );

            TargetCurcuit.Path.InsertPoint(SelectedSegment.y, newPoint);

            SelectedBezierPoint = newPoint;
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void AddPoint(Vector2 mousePosition)
        {
            Undo.RecordObject(TargetCurcuit, "Add Bezier Point");

            Vector2 direction = TargetCurcuit.Path.Count > 0
                ? TargetCurcuit.Path[TargetCurcuit.Path.Count - 1].Anchor - mousePosition : Vector2.right;

            BezierPoint newPoint = new BezierPoint(
                    HandleTransform.InverseTransformPoint(mousePosition),
                    direction
                    );

            TargetCurcuit.Path.AddPoint(
                newPoint
                );

            SelectedBezierPoint = newPoint;
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void AddPointInOrigin()
        {
            Undo.RecordObject(TargetCurcuit, "Add Bezier Point");

            BezierPoint newPoint = new BezierPoint();

            TargetCurcuit.Path.AddPoint(newPoint);

            SelectedBezierPoint = newPoint;
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void RemovePoint()
        {
            if (TargetCurcuit.Path.Count > 1)
            {
                Undo.RecordObject(TargetCurcuit, "Remove Bezier Point");

                TargetCurcuit.Path.RemovePoint(SelectedBezierPoint);
            }
        }

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
