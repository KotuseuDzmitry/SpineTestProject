using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace SurcuitUtitlity
{
    [CustomEditor(typeof(Curcuit))]
    public class CurcuitEditor : Editor
    {
        private const float handleSize = 0.06f;

        private const float maxDistanceToPoint = 0.125f;

        private const float maxDistanceToCurve = 0.15f;

        private Color normalColor = Color.white;

        private Color bezierCurveColor = Color.green;

        private Color selectedBezierCurveColor = Color.yellow;

        private Color selectedBezierPointColor = Color.yellow;

        private Curcuit curcuit;

        private Transform handleTransform;

        private Quaternion handleRotation;

        private Vector2Int selectedSegment = new Vector2Int(-1, -1);

        private BezierPoint selectedBezierPoint = null;

        private void OnSceneGUI()
        {
            curcuit = target as Curcuit;

            HandleInput();

            HandleDrawing();

            PreventDeselectionIfCurveSelected();
        }

        private void PreventDeselectionIfCurveSelected()
        {
            if (selectedSegment != new Vector2Int(-1, -1))
            {
                Debug.Log(Selection.activeGameObject);
                Selection.activeGameObject = curcuit.gameObject;
            }
        }

        private void HandleInput()
        {
            Event guiEvent = Event.current;

            Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
            HandleAddPointToTheEnd(guiEvent, mousePosition);
            HandleSelection(guiEvent, mousePosition);
            // HandleRemovePoint(guiEvent);
            HandleInsertPoint(guiEvent);
        }

        private void HandleInsertPoint(Event guiEvent)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.I && guiEvent.shift
                && selectedSegment != new Vector2Int(-1, -1))
            {
                curcuit.Path.InsertPoint(selectedSegment.y, new BezierPoint(
                    Vector2.Lerp(curcuit.Path[selectedSegment.x].Anchor, curcuit.Path[selectedSegment.y].Anchor, 0.5f)
                    ));
            }
        }

        private void HandleSelection(Event guiEvent, Vector2 mousePosition)
        {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                bool isBezierPointSelected = HandleBezierPointSelection(mousePosition);

                selectedSegment = new Vector2Int(-1, -1);

                if (!isBezierPointSelected)
                {
                    if (HandleSegmentSelection(mousePosition))
                    {
                        selectedBezierPoint = null;
                    }
                }
            }
        }

        private void HandleAddPointToTheEnd(Event guiEvent, Vector2 mousePosition)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.A && guiEvent.shift)
            {
                Undo.RecordObject(curcuit, "Add Bezier Point");
                curcuit.Path.AddPoint(
                    new BezierPoint(handleTransform.InverseTransformPoint(mousePosition))
                    );
            }
        }

        private bool HandleBezierPointSelection(Vector2 mousePosition)
        {
            BezierPoint currentSelectionCandidate = null;
            float currentMinDistance = maxDistanceToPoint;

            foreach (BezierPoint bezierPoint in curcuit.Path)
            {
                Vector2 anchorGlobal = handleTransform.TransformPoint(bezierPoint.Anchor);
                Vector2 controlPoint1Global = handleTransform.TransformPoint(bezierPoint.ControlPoint1);
                Vector2 controlPoint2Global = handleTransform.TransformPoint(bezierPoint.ControlPoint2);

                float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                float anchorDistance = Vector2.Distance(anchorGlobal, mousePosition) / anchorSize;

                if (anchorDistance < currentMinDistance)
                {
                    currentMinDistance = anchorDistance;
                    currentSelectionCandidate = bezierPoint;
                }

                float controlPoint1Size = HandleUtility.GetHandleSize(controlPoint1Global);

                float controlPoint1Distance = Vector2.Distance(controlPoint1Global, mousePosition) / controlPoint1Size;
                if (controlPoint1Distance < currentMinDistance)
                {
                    currentMinDistance = controlPoint1Distance;
                    currentSelectionCandidate = bezierPoint;
                }

                float controlPoint2Size = HandleUtility.GetHandleSize(controlPoint2Global);

                float controlPoint2Distance = Vector2.Distance(controlPoint2Global, mousePosition) / controlPoint2Size;
                if (controlPoint2Distance < currentMinDistance)
                {
                    currentMinDistance = controlPoint2Distance;
                    currentSelectionCandidate = bezierPoint;
                }
            }

            selectedBezierPoint = currentSelectionCandidate;

            return currentSelectionCandidate != null;
        }

        private bool HandleSegmentSelection(Vector2 mousePosition)
        {
            Vector2Int currentSelectionCandidate = new Vector2Int(-1, -1);
            float currentMinDistance = maxDistanceToCurve;
            for (var i = 1; i < curcuit.Path.Count; i++)
            {
                Vector2 anchorGlobal = handleTransform.TransformPoint(curcuit.Path[i - 1].Anchor);
                float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                float distance = HandleUtility.DistancePointBezier(
                    mousePosition,
                    handleTransform.TransformPoint(curcuit.Path[i - 1].Anchor),
                    handleTransform.TransformPoint(curcuit.Path[i].Anchor),
                    handleTransform.TransformPoint(curcuit.Path[i - 1].ControlPoint2),
                    handleTransform.TransformPoint(curcuit.Path[i].ControlPoint1)
                    ) / anchorSize;

                if (distance < currentMinDistance)
                {
                    currentMinDistance = distance;
                    currentSelectionCandidate = new Vector2Int(i - 1, i);
                }
            }

            selectedSegment = currentSelectionCandidate;

            return currentSelectionCandidate != new Vector2Int(-1, -1);
        }

        private void HandleDrawing()
        {
            handleTransform = curcuit.transform;

            handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

            Handles.color = normalColor;

            if (curcuit.Path.Count > 0)
            {
                ShowBezierPoint(curcuit.Path[0]);
            }

            for (var i = 1; i < curcuit.Path.Count; i++)
            {
                ShowBezierPoint(curcuit.Path[i]);

                ShowBezierCurve(curcuit.Path[i - 1], curcuit.Path[i], i - 1, i);
            }
        }

        private void ShowBezierCurve(BezierPoint firstBezierPoint, BezierPoint secondBezierPoint, int index1, int index2)
        {
            Color paintColor = bezierCurveColor;
            if ((selectedSegment - new Vector2(index1, index2)).sqrMagnitude < Mathf.Epsilon)
            {
                paintColor = selectedBezierCurveColor;
            }

            Handles.DrawBezier(
                handleTransform.TransformPoint(firstBezierPoint.Anchor),
                handleTransform.TransformPoint(secondBezierPoint.Anchor),
                handleTransform.TransformPoint(firstBezierPoint.ControlPoint2),
                handleTransform.TransformPoint(secondBezierPoint.ControlPoint1),
                paintColor,
                null,
                2f
                );
        }

        private void ShowBezierPoint(BezierPoint bezierPoint)
        {
            if (selectedBezierPoint == bezierPoint)
            {
                Handles.color = selectedBezierPointColor;
            }

            ShowPointControlPoint(bezierPoint, 1);
            ShowPointAnchorPoint(bezierPoint);
            ShowPointControlPoint(bezierPoint, 2);

            ShowLines(bezierPoint);

            Handles.color = normalColor;
        }

        private void ShowLines(BezierPoint bezierPoint)
        {
            Handles.DrawLine(handleTransform.TransformPoint(bezierPoint.Anchor),
                handleTransform.TransformPoint(bezierPoint.ControlPoint1));

            Handles.DrawLine(handleTransform.TransformPoint(bezierPoint.Anchor),
                handleTransform.TransformPoint(bezierPoint.ControlPoint2));
        }

        private void ShowPointAnchorPoint(BezierPoint bezierPoint)
        {
            Vector2 anchorGlobal = handleTransform.TransformPoint(bezierPoint.Anchor);

            float size = HandleUtility.GetHandleSize(anchorGlobal);

            EditorGUI.BeginChangeCheck();

            anchorGlobal = Handles.FreeMoveHandle(anchorGlobal, handleRotation, handleSize * size, Vector2.zero,
                Handles.CylinderHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curcuit, "Move Anchor Point");
                bezierPoint.Anchor = handleTransform.InverseTransformPoint(anchorGlobal);
            }
        }

        private void ShowPointControlPoint(BezierPoint bezierPoint, int pointIndex)
        {
            if (pointIndex == 1)
            {
                Vector2 pointGlobal = handleTransform.TransformPoint(bezierPoint.ControlPoint1);

                float size = HandleUtility.GetHandleSize(pointGlobal);

                EditorGUI.BeginChangeCheck();
                //if (selectedBezierPoint != bezierPoint)
                //{
                //    Handles.DrawSolidArc(pointGlobal, Vector3.forward, Vector2.one * 10, 360, handleSize * size / 2);
                //}

                pointGlobal = Handles.FreeMoveHandle(pointGlobal, handleRotation, handleSize * size, Vector2.zero, Handles.CylinderHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curcuit, "Move Control Point 1");
                    bezierPoint.ControlPoint1 = handleTransform.InverseTransformPoint(pointGlobal);
                }
            }
            else
            {
                Vector2 pointGlobal = handleTransform.TransformPoint(bezierPoint.ControlPoint2);

                float size = HandleUtility.GetHandleSize(pointGlobal);

                EditorGUI.BeginChangeCheck();
                //if (selectedBezierPoint != bezierPoint)
                //{
                //    Handles.DrawSolidArc(pointGlobal, Vector3.forward, Vector2.one * 10, 360, handleSize * size / 2);
                //}

                pointGlobal = Handles.FreeMoveHandle(pointGlobal, handleRotation, handleSize * size, Vector2.zero, Handles.CylinderHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curcuit, "Move Control Point 2");
                    bezierPoint.ControlPoint2 = handleTransform.InverseTransformPoint(pointGlobal);
                }
            }
            
        }
    }
}
