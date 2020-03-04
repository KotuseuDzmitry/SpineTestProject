using UnityEngine;
using UnityEditor;

namespace BezierSurcuitUtitlity
{
    public static class CurcuitEditorDrawer
    {
        private static CurcuitEditor curcuitEditor; 

        public static void HandleDrawing(CurcuitEditor curcuitEditor)
        {
            CurcuitEditorDrawer.curcuitEditor = curcuitEditor;

            if (curcuitEditor == null)
            {
                return;
            }

            Handles.color = curcuitEditor.NormalColor;

            if (curcuitEditor.TargetCurcuit.Path.Count > 0)
            {
                ShowBezierPoint(curcuitEditor.TargetCurcuit.Path[0]);
            }

            for (var i = 1; i < curcuitEditor.TargetCurcuit.Path.Count; i++)
            {
                ShowBezierPoint(curcuitEditor.TargetCurcuit.Path[i]);

                ShowBezierCurve(curcuitEditor.TargetCurcuit.Path[i - 1], curcuitEditor.TargetCurcuit.Path[i], i - 1, i);
            }
        }

        private static void ShowBezierCurve(BezierPoint firstBezierPoint, BezierPoint secondBezierPoint, int index1, int index2)
        {
            Color paintColor = curcuitEditor.BezierCurveColor;
            if ((curcuitEditor.SelectedSegment - new Vector2(index1, index2)).sqrMagnitude < Mathf.Epsilon)
            {
                paintColor = curcuitEditor.SelectedBezierCurveColor;
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
                Handles.color = curcuitEditor.SelectedBezierPointColor;
            }

            ShowPointControlPoint(bezierPoint, 1);
            ShowPointAnchorPoint(bezierPoint);
            ShowPointControlPoint(bezierPoint, 2);

            ShowLines(bezierPoint);

            Handles.color = curcuitEditor.NormalColor;
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
