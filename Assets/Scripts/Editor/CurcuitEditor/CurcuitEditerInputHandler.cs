using UnityEngine;
using UnityEditor;

namespace BezierSurcuitUtitlity
{
    public static class CurcuitEditerInputHandler
    {
        private static CurcuitEditor curcuitEditor;

        public static void HandleInput(CurcuitEditor curcuitEditor)
        {
            CurcuitEditerInputHandler.curcuitEditor = curcuitEditor;

            if (curcuitEditor == null)
            {
                return;
            }

            Event guiEvent = Event.current;

            Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
            HandleAddPointToTheEnd(guiEvent, mousePosition);
            HandleSelection(guiEvent, mousePosition);
            HandleRemovePoint(guiEvent);
            HandleInsertPoint(guiEvent);
        }

        private static void HandleRemovePoint(Event guiEvent)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.X && guiEvent.shift
                && curcuitEditor.SelectedBezierPoint != null)
            {
                curcuitEditor.RemovePoint();
            }
        }

        private static void HandleInsertPoint(Event guiEvent)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.I && guiEvent.shift
                && curcuitEditor.SelectedSegment != new Vector2Int(-1, -1))
            {
                curcuitEditor.InsertPoint();
            }
        }

        private static void HandleSelection(Event guiEvent, Vector2 mousePosition)
        {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                bool isBezierPointSelected = HandleBezierPointSelection(mousePosition);

                curcuitEditor.SelectedSegment = new Vector2Int(-1, -1);

                if (!isBezierPointSelected)
                {
                    if (HandleSegmentSelection(mousePosition))
                    {
                        curcuitEditor.SelectedBezierPoint = null;
                    }
                }

                curcuitEditor.Repaint();
            }
        }

        private static void HandleAddPointToTheEnd(Event guiEvent, Vector2 mousePosition)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.A && guiEvent.shift)
            {
                curcuitEditor.AddPoint(mousePosition);
            }
        }

        private static bool HandleBezierPointSelection(Vector2 mousePosition)
        {
            BezierPoint currentSelectionCandidate = null;
            float currentMinDistance = CurcuitEditor.maxDistanceToPoint;

            foreach (BezierPoint bezierPoint in curcuitEditor.TargetCurcuit.Path)
            {
                Vector2 anchorGlobal = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.Anchor);
                Vector2 controlPoint1Global = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint1);
                Vector2 controlPoint2Global = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint2);

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

            curcuitEditor.SelectedBezierPoint = currentSelectionCandidate;

            return currentSelectionCandidate != null;
        }

        private static bool HandleSegmentSelection(Vector2 mousePosition)
        {
            Vector2Int currentSelectionCandidate = new Vector2Int(-1, -1);
            float currentMinDistance = CurcuitEditor.maxDistanceToCurve;

            int pathCount = curcuitEditor.TargetCurcuit.Path.Count;

            if (curcuitEditor.TargetCurcuit.Path.IsCyclic && pathCount >= 2)
            {
                Vector2 anchorGlobal
                    = curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[pathCount - 1].Anchor);
                float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                float distance = HandleUtility.DistancePointBezier(
                    mousePosition,
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[pathCount - 1].Anchor),
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[0].Anchor),
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[pathCount - 1].ControlPoint2),
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[0].ControlPoint1)
                    ) / anchorSize;

                if (distance < currentMinDistance)
                {
                    currentMinDistance = distance;
                    currentSelectionCandidate = new Vector2Int(pathCount - 1, 0);
                }
            }

            for (var i = 1; i < pathCount; i++)
            {
                Vector2 anchorGlobal = curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[i - 1].Anchor);
                float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                float distance = HandleUtility.DistancePointBezier(
                    mousePosition,
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[i - 1].Anchor),
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[i].Anchor),
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[i - 1].ControlPoint2),
                    curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.TargetCurcuit.Path[i].ControlPoint1)
                    ) / anchorSize;

                if (distance < currentMinDistance)
                {
                    currentMinDistance = distance;
                    currentSelectionCandidate = new Vector2Int(i - 1, i);
                }
            }

            curcuitEditor.SelectedSegment = currentSelectionCandidate;

            return currentSelectionCandidate != new Vector2Int(-1, -1);
        }
    }
}
