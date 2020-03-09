using UnityEngine;
using UnityEditor;
using System;

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

            HandlePathMoveHandleActivation(guiEvent);
            HandleAdd(guiEvent, mousePosition);
            HandleSelection(guiEvent, mousePosition); // TODO
            HandleRemove(guiEvent);
            HandleInsertPoint(guiEvent); // TODO
        }

        private static void HandlePathMoveHandleActivation(Event guiEvent)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.G && guiEvent.shift)
            {
                curcuitEditor.IsPathMoveHandleVisible = !curcuitEditor.IsPathMoveHandleVisible;
                curcuitEditor.Repaint();
            }
        }

        private static void HandleAdd(Event guiEvent, Vector2 mousePosition)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.A && guiEvent.shift)
            {
                ShowAddGenericMenu(mousePosition);
            }
        }

        private static void HandleRemove(Event guiEvent)
        {
            if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.X && guiEvent.shift
                && (curcuitEditor.SelectedBezierPoint != null || curcuitEditor.SelectedSegment != new Vector2Int(-1, -1)
                || curcuitEditor.SelectedPath != null))
            {
                ShowRemoveGenericMenu();
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

        private static bool HandleBezierPointSelection(Vector2 mousePosition)
        {
            Path currentSelectionPathCandidate = null;
            BezierPoint currentSelectionPointCandidate = null;
            float currentMinDistance = CurcuitEditor.maxDistanceToPoint;

            foreach (Path path in curcuitEditor.TargetCurcuit)
            {
                foreach (BezierPoint bezierPoint in path)
                {
                    Vector2 anchorGlobal = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.Anchor);
                    Vector2 controlPoint1Global = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint1);
                    Vector2 controlPoint2Global = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint2);

                    float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                    float anchorDistance = Vector2.Distance(anchorGlobal, mousePosition) / anchorSize;

                    if (anchorDistance < currentMinDistance)
                    {
                        currentMinDistance = anchorDistance;
                        currentSelectionPointCandidate = bezierPoint;
                        currentSelectionPathCandidate = path;
                    }

                    float controlPoint1Size = HandleUtility.GetHandleSize(controlPoint1Global);

                    float controlPoint1Distance = Vector2.Distance(controlPoint1Global, mousePosition) / controlPoint1Size;
                    if (controlPoint1Distance < currentMinDistance)
                    {
                        currentMinDistance = controlPoint1Distance;
                        currentSelectionPointCandidate = bezierPoint;
                        currentSelectionPathCandidate = path;
                    }

                    float controlPoint2Size = HandleUtility.GetHandleSize(controlPoint2Global);

                    float controlPoint2Distance = Vector2.Distance(controlPoint2Global, mousePosition) / controlPoint2Size;
                    if (controlPoint2Distance < currentMinDistance)
                    {
                        currentMinDistance = controlPoint2Distance;
                        currentSelectionPointCandidate = bezierPoint;
                        currentSelectionPathCandidate = path;
                    }
                }
            }

            if (currentSelectionPathCandidate != null)
            {
                curcuitEditor.SelectedPath = currentSelectionPathCandidate;
            }

            curcuitEditor.SelectedBezierPoint = currentSelectionPointCandidate;

            return currentSelectionPointCandidate != null;
        }

        private static bool HandleSegmentSelection(Vector2 mousePosition)
        {
            Path currentSelectionPathCandidate = null;
            Vector2Int currentSelectionCandidate = new Vector2Int(-1, -1);
            float currentMinDistance = CurcuitEditor.maxDistanceToCurve;

            foreach (Path path in curcuitEditor.TargetCurcuit)
            {
                int pathCount = path.Count;

                if (path.IsCyclic && pathCount >= 2)
                {
                    Vector2 anchorGlobal
                        = curcuitEditor.HandleTransform.TransformPoint(path[pathCount - 1].Anchor);
                    float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                    float distance = HandleUtility.DistancePointBezier(
                        mousePosition,
                        curcuitEditor.HandleTransform.TransformPoint(path[pathCount - 1].Anchor),
                        curcuitEditor.HandleTransform.TransformPoint(path[0].Anchor),
                        curcuitEditor.HandleTransform.TransformPoint(path[pathCount - 1].ControlPoint2),
                        curcuitEditor.HandleTransform.TransformPoint(path[0].ControlPoint1)
                        ) / anchorSize;

                    if (distance < currentMinDistance)
                    {
                        currentMinDistance = distance;
                        currentSelectionCandidate = new Vector2Int(pathCount - 1, 0);
                        currentSelectionPathCandidate = path;
                    }
                }

                for (var i = 1; i < pathCount; i++)
                {
                    Vector2 anchorGlobal = curcuitEditor.HandleTransform.TransformPoint(path[i - 1].Anchor);
                    float anchorSize = HandleUtility.GetHandleSize(anchorGlobal);

                    float distance = HandleUtility.DistancePointBezier(
                        mousePosition,
                        curcuitEditor.HandleTransform.TransformPoint(path[i - 1].Anchor),
                        curcuitEditor.HandleTransform.TransformPoint(path[i].Anchor),
                        curcuitEditor.HandleTransform.TransformPoint(path[i - 1].ControlPoint2),
                        curcuitEditor.HandleTransform.TransformPoint(path[i].ControlPoint1)
                        ) / anchorSize;

                    if (distance < currentMinDistance)
                    {
                        currentMinDistance = distance;
                        currentSelectionCandidate = new Vector2Int(i - 1, i);
                        currentSelectionPathCandidate = path;
                    }
                }
            }

            if (currentSelectionPathCandidate != null)
            {
                curcuitEditor.SelectedPath = currentSelectionPathCandidate;
            }

            curcuitEditor.SelectedSegment = currentSelectionCandidate;

            return currentSelectionCandidate != new Vector2Int(-1, -1);
        }

        private static void ShowAddGenericMenu(Vector2 mousePosition)
        {
            GenericMenu addMenu = new GenericMenu();

            addMenu.AddItem(new GUIContent("Add new path"), false,
                () =>
                {
                    curcuitEditor.AddPath(mousePosition);
                });

            if (curcuitEditor.SelectedPath != null)
            {
                addMenu.AddItem(new GUIContent("Add new point to currently selected path"), false,
                () =>
                {
                    curcuitEditor.AddPoint(mousePosition);
                });
            }
            else
            {
                addMenu.AddDisabledItem(new GUIContent("Add new point to currently selected path"));
            }

            addMenu.ShowAsContext();
        }

        private static void ShowRemoveGenericMenu()
        {
            GenericMenu removeMenu = new GenericMenu();

            if (curcuitEditor.SelectedPath != null && curcuitEditor.TargetCurcuit.Count > 1)
            {
                removeMenu.AddItem(new GUIContent("Remove selected path"), false,
                () =>
                {
                    curcuitEditor.RemovePath();
                });
            }
            else
            {
                removeMenu.AddDisabledItem(new GUIContent("Remove selected path"));
            }

            if (curcuitEditor.SelectedBezierPoint != null && curcuitEditor.SelectedPath.Count > 1)
            {
                removeMenu.AddItem(new GUIContent("Remove selected point"), false,
                () =>
                {
                    curcuitEditor.RemovePoint();
                });
            }
            else
            {
                removeMenu.AddDisabledItem(new GUIContent("Remove selected point"));
            }

            removeMenu.ShowAsContext();
        }
    }
}
