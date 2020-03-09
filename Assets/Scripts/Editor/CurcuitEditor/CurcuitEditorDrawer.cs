using UnityEngine;
using UnityEditor;
using System;

namespace BezierSurcuitUtitlity
{
    public static class CurcuitEditorDrawer
    {
        private static CurcuitEditor curcuitEditor;

        private static float currentColorOpacity = 1f;

        public static void HandleDrawing(CurcuitEditor curcuitEditor)
        {
            CurcuitEditorDrawer.curcuitEditor = curcuitEditor;

            if (curcuitEditor == null)
            {
                return;
            }

            foreach (Path path in curcuitEditor.TargetCurcuit)
            {
                if (path == curcuitEditor.SelectedPath)
                {
                    currentColorOpacity = CurcuitEditor.selectedPathOpacity;

                    DrawSelectedPathMoveHandle();
                }
                else
                {
                    currentColorOpacity = CurcuitEditor.nonSelectedPathOpacity;
                }

                int pathCount = path.Count;

                Handles.color =
                    new Color(
                        curcuitEditor.NormalColor.r,
                        curcuitEditor.NormalColor.g,
                        curcuitEditor.NormalColor.b,
                        currentColorOpacity
                    );

                if (pathCount > 0)
                {
                    ShowBezierPoint(path[0]);
                }

                if (path.IsCyclic && pathCount >= 2)
                {
                    ShowBezierCurve(path[pathCount - 1], path[0], pathCount - 1, 0);
                }

                for (var i = 1; i < pathCount; i++)
                {
                    ShowBezierPoint(path[i]);

                    ShowBezierCurve(path[i - 1], path[i], i - 1, i);
                }
            }
        }

        private static void DrawSelectedPathMoveHandle()
        {
            if (!curcuitEditor.IsPathMoveHandleVisible)
            {
                return;
            }

            if (curcuitEditor.SelectedPath == null)
            {
                return;
            }

            Vector2 globalPathPosition = curcuitEditor.HandleTransform.TransformPoint(curcuitEditor.SelectedPath.PathPosition);
            EditorGUI.BeginChangeCheck();
            globalPathPosition = Handles.DoPositionHandle(globalPathPosition, curcuitEditor.HandleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curcuitEditor.TargetCurcuit, "Move Path");
                EditorUtility.SetDirty(curcuitEditor.TargetCurcuit);
                curcuitEditor.SelectedPath.PathPosition = curcuitEditor.HandleTransform.InverseTransformPoint(globalPathPosition);
            }
        }

        private static void ShowBezierCurve(BezierPoint firstBezierPoint, BezierPoint secondBezierPoint, int index1, int index2)
        {
            Color paintColor =
                    new Color(
                        curcuitEditor.BezierCurveColor.r,
                        curcuitEditor.BezierCurveColor.g,
                        curcuitEditor.BezierCurveColor.b,
                        currentColorOpacity
                    );

            if (curcuitEditor.SelectedSegment == new Vector2Int(index1, index2))
            {
                paintColor =
                    new Color(
                        curcuitEditor.SelectedBezierCurveColor.r,
                        curcuitEditor.SelectedBezierCurveColor.g,
                        curcuitEditor.SelectedBezierCurveColor.b,
                        currentColorOpacity
                    );
            }

            Handles.DrawBezier(
                curcuitEditor.HandleTransform.TransformPoint(firstBezierPoint.Anchor),
                curcuitEditor.HandleTransform.TransformPoint(secondBezierPoint.Anchor),
                curcuitEditor.HandleTransform.TransformPoint(firstBezierPoint.ControlPoint2),
                curcuitEditor.HandleTransform.TransformPoint(secondBezierPoint.ControlPoint1),
                paintColor,
                null,
                2f
                );
        }

        private static void ShowBezierPoint(BezierPoint bezierPoint)
        {
            if (curcuitEditor.SelectedBezierPoint == bezierPoint)
            {
                Handles.color =
                    new Color(
                        curcuitEditor.SelectedBezierPointColor.r,
                        curcuitEditor.SelectedBezierPointColor.g,
                        curcuitEditor.SelectedBezierPointColor.b,
                        currentColorOpacity
                    );
            }

            ShowPointControlPoint(bezierPoint, 1);
            ShowPointAnchorPoint(bezierPoint);
            ShowPointControlPoint(bezierPoint, 2);

            ShowLines(bezierPoint);

            Handles.color =
                    new Color(
                        curcuitEditor.NormalColor.r,
                        curcuitEditor.NormalColor.g,
                        curcuitEditor.NormalColor.b,
                        currentColorOpacity
                    );
        }

        private static void ShowLines(BezierPoint bezierPoint)
        {
            Handles.DrawLine(curcuitEditor.HandleTransform.TransformPoint(bezierPoint.Anchor),
                curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint1));

            Handles.DrawLine(curcuitEditor.HandleTransform.TransformPoint(bezierPoint.Anchor),
                curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint2));
        }

        private static void ShowPointAnchorPoint(BezierPoint bezierPoint)
        {
            Color oldColor = Handles.color;

            Handles.color =
                    new Color(
                        curcuitEditor.BezierAnchorPointColor.r,
                        curcuitEditor.BezierAnchorPointColor.g,
                        curcuitEditor.BezierAnchorPointColor.b,
                        currentColorOpacity
                    );

            Vector2 anchorGlobal = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.Anchor);

            float size = HandleUtility.GetHandleSize(anchorGlobal);

            EditorGUI.BeginChangeCheck();

            anchorGlobal = Handles.FreeMoveHandle(anchorGlobal, curcuitEditor.HandleRotation, CurcuitEditor.handleSize * size, Vector2.zero,
                Handles.CylinderHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curcuitEditor.TargetCurcuit, "Move Anchor Point");
                bezierPoint.Anchor = curcuitEditor.HandleTransform.InverseTransformPoint(anchorGlobal);
            }

            Handles.color = oldColor;
        }

        private static void ShowPointControlPoint(BezierPoint bezierPoint, int pointIndex)
        {
            if (pointIndex == 1)
            {
                Vector2 pointGlobal = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint1);

                float size = HandleUtility.GetHandleSize(pointGlobal);

                EditorGUI.BeginChangeCheck();

                pointGlobal = Handles.FreeMoveHandle(pointGlobal, curcuitEditor.HandleRotation, CurcuitEditor.handleSize * size, Vector2.zero, Handles.CylinderHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curcuitEditor.TargetCurcuit, "Move Control Point 1");
                    bezierPoint.ControlPoint1 = curcuitEditor.HandleTransform.InverseTransformPoint(pointGlobal);
                }
            }
            else
            {
                Vector2 pointGlobal = curcuitEditor.HandleTransform.TransformPoint(bezierPoint.ControlPoint2);

                float size = HandleUtility.GetHandleSize(pointGlobal);

                EditorGUI.BeginChangeCheck();

                pointGlobal = Handles.FreeMoveHandle(pointGlobal, curcuitEditor.HandleRotation, CurcuitEditor.handleSize * size, Vector2.zero, Handles.CylinderHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curcuitEditor.TargetCurcuit, "Move Control Point 2");
                    bezierPoint.ControlPoint2 = curcuitEditor.HandleTransform.InverseTransformPoint(pointGlobal);
                }
            }
        }
    }
}
