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

        public const float selectedPathOpacity = 1f;

        public const float nonSelectedPathOpacity = 0.6f;

        private Color normalColor = Color.white;

        private Color bezierCurveColor = Color.green;

        private Color selectedBezierCurveColor = Color.yellow;

        private Color selectedBezierPointColor = Color.yellow;

        private Color bezierAnchorPointColor = Color.red;

        private Color selectedBezierPointInspectorColor = new Color(18f / 255f, 126f / 255f, 220f / 255f, 1f);

        private Color selectedPathInspectorColor = new Color(18f / 255f, 126f / 255f, 220f / 255f, 1f);

        public bool IsPathMoveHandleVisible
        {
            get; set;
        } = false;

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

        public Path SelectedPath
        {
            get; set;
        } = null;

        public Color NormalColor => normalColor;

        public Color BezierCurveColor => bezierCurveColor;

        public Color SelectedBezierCurveColor => selectedBezierCurveColor;

        public Color SelectedBezierPointColor => selectedBezierPointColor;

        public Color BezierAnchorPointColor => bezierAnchorPointColor;

        public Color SelectedBezierPointInspectorColor => selectedBezierPointInspectorColor;

        public Color SelectedPathInspectorColor => selectedPathInspectorColor;

        public void InsertPoint()
        {
            Undo.RecordObject(TargetCurcuit, "Insert Bezier Point");

            Vector2 midPoint = Bezier.CubicCurve(
                    SelectedPath[SelectedSegment.x].Anchor,
                    SelectedPath[SelectedSegment.x].ControlPoint2,
                    SelectedPath[SelectedSegment.y].ControlPoint1,
                    SelectedPath[SelectedSegment.y].Anchor,
                    0.5f
                    );

            Vector2 direction = SelectedPath[SelectedSegment.x].Anchor
                - SelectedPath[SelectedSegment.y].Anchor;

            BezierPoint newPoint = new BezierPoint(
                midPoint,
                direction
                );

            SelectedPath.InsertPoint(SelectedSegment.y, newPoint);

            SelectedBezierPoint = newPoint;
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void AddPoint(Vector2 mousePosition)
        {
            Undo.RecordObject(TargetCurcuit, "Add Bezier Point");

            Vector2 direction = SelectedPath.Count > 0
                ? SelectedPath[SelectedPath.Count - 1].Anchor - mousePosition : Vector2.right;

            BezierPoint newPoint = new BezierPoint(
                    HandleTransform.InverseTransformPoint(mousePosition),
                    direction
                    );

            SelectedPath.AddPoint(
                newPoint
                );

            SelectedBezierPoint = newPoint;
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void AddPath(Vector2 mousePosition)
        {
            Undo.RecordObject(TargetCurcuit, "Add Path");

            Path newPath = new Path(mousePosition);

            TargetCurcuit.AddPath(newPath);

            SelectedPath = newPath;
            SelectedBezierPoint = newPath[0];
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void AddPathToOrigin()
        {
            Undo.RecordObject(TargetCurcuit, "Add Path");

            Path newPath = new Path();

            TargetCurcuit.AddPath(newPath);

            SelectedPath = newPath;
            SelectedBezierPoint = newPath[0];
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void AddPointInOrigin(int pathIndex)
        {
            Undo.RecordObject(TargetCurcuit, "Add Bezier Point");

            BezierPoint newPoint = new BezierPoint();

            TargetCurcuit[pathIndex].AddPoint(newPoint);

            SelectedBezierPoint = newPoint;
            SelectedSegment = new Vector2Int(-1, -1);
        }

        public void RemovePoint()
        {
            if (SelectedPath.Count > 1)
            {
                Undo.RecordObject(TargetCurcuit, "Remove Bezier Point");

                SelectedPath.RemovePoint(SelectedBezierPoint);
            }
        }

        public void RemovePoint(Path path, int pointIndex)
        {
            if (path.Count > 1)
            {
                Undo.RecordObject(TargetCurcuit, "Remove Bezier Point");

                path.RemovePointAt(pointIndex);
            }
        }

        public void RemovePath()
        {
            if (TargetCurcuit.Count > 1)
            {
                Undo.RecordObject(TargetCurcuit, "Remove Path");

                TargetCurcuit.RemovePath(SelectedPath);
            }
        }

        public void RemovePath(int index)
        {
            if (TargetCurcuit.Count > 1)
            {
                Undo.RecordObject(TargetCurcuit, "Remove Path");

                TargetCurcuit.RemovePathAt(index);
            }
        }

        private void OnEnable()
        {
            TargetCurcuit = target as Curcuit;

            SelectedPath = TargetCurcuit[0];
            SelectedBezierPoint = SelectedPath[0];

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
