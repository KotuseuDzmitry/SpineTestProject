using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

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

        public Curcuit TargetCurcuit
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

        public Color BezierCurveColor
        {
            get
            {
                return bezierCurveColor;
            }
        }

        public Color SelectedBezierCurveColor
        {
            get
            {
                return selectedBezierCurveColor;
            }
        }

        public Color SelectedBezierPointColor
        {
            get
            {
                return selectedBezierPointColor;
            }
        }

        private void OnSceneGUI()
        {
            TargetCurcuit = target as Curcuit;

            HandleTransform = TargetCurcuit.transform;

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

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            for (var i = 0; i < TargetCurcuit.Path.Count; i++)
            {

            }
        }
    }
}
