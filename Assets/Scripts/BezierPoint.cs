using UnityEngine;
using System;

namespace BezierSurcuitUtitlity
{
    [Serializable]
    public class BezierPoint
    {
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        public bool isExpanded = false;
#endif

        [SerializeField]
        private Vector2 anchor;

        [SerializeField]
        private Vector2 controlPoint1;

        [SerializeField]
        private Vector2 controlPoint2;

        [SerializeField]
        private BezierControlPointMode controlPointMode = BezierControlPointMode.Mirrored;

        public BezierPoint()
        {
            anchor = Vector2.zero;
            controlPoint1 = anchor + Vector2.right;
            controlPoint2 = anchor - Vector2.right;
        }

        public BezierPoint(Vector2 anchor)
        {
            this.anchor = anchor;
            controlPoint1 = anchor + Vector2.right;
            controlPoint2 = anchor - Vector2.right;
        }

        public BezierPoint(Vector2 anchor, Vector2 direction)
        {
            this.anchor = anchor;
            controlPoint1 = anchor + direction.normalized;
            controlPoint2 = anchor - direction.normalized;
        }

        public BezierPoint(Vector2 anchor, Vector2 direction, float offset)
        {
            this.anchor = anchor;
            controlPoint1 = anchor + direction.normalized * offset;
            controlPoint2 = anchor - direction.normalized * offset;
        }

        public BezierControlPointMode ControlPointMode
        {
            get
            {
                return controlPointMode;
            }
            set
            {
                if (value == controlPointMode)
                {
                    return;
                }

                if (value == BezierControlPointMode.Mirrored)
                {
                    controlPoint2 = 2 * anchor - controlPoint1;
                }
                else if (value == BezierControlPointMode.Aligned && controlPointMode != BezierControlPointMode.Mirrored)
                {
                    float distance = (anchor - controlPoint2).magnitude;
                    controlPoint2 = (2 * anchor - controlPoint1).normalized * distance;
                }

                controlPointMode = value;
            }
        }

        public Vector2 Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                Vector2 anchorDelta = value - anchor;

                anchor = value;

                controlPoint1 += anchorDelta;
                controlPoint2 += anchorDelta;
            }
        }

        public Vector2 ControlPoint1
        {
            get
            {
                return controlPoint1;
            }
            set
            {
                controlPoint1 = value;

                if(controlPointMode == BezierControlPointMode.Mirrored)
                {
                    controlPoint2 = 2 * anchor - controlPoint1;
                }
                else if (controlPointMode == BezierControlPointMode.Aligned)
                {
                    float distance = (anchor - controlPoint2).magnitude;
                    controlPoint2 = anchor + (anchor - controlPoint1).normalized * distance;
                }
            }
        }

        public Vector2 ControlPoint2
        {
            get
            {
                return controlPoint2;
            }
            set
            {
                controlPoint2 = value;

                if (controlPointMode == BezierControlPointMode.Mirrored)
                {
                    controlPoint1 = 2 * anchor - controlPoint2;
                }
                else if (controlPointMode == BezierControlPointMode.Aligned)
                {
                    float distance = (anchor - controlPoint1).magnitude;
                    controlPoint1 = anchor + (anchor - controlPoint2).normalized * distance;
                }
            }
        }
    }

    public enum BezierControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }
}
