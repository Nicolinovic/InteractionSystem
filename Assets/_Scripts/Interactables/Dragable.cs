using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Interacting
{
    public enum MoveType
    {
        Transform,
        Velocity,
    }

    public enum ConstrainTypes
    {
        Position,
        Rotation
    }

    [System.Serializable]
    public struct Constrains
    {
        public ConstrainTypes ConstrainType;
        public bool X, Y;

        public Constrains(ConstrainTypes constrainType, bool x, bool y)
        {
            this.ConstrainType = constrainType;
            this.X = x;
            this.Y = y;
        }
    }

    public enum ClampAxis
    {
        X,
        Y,
    }

    [System.Serializable]
    public struct Clamps
    {
        public ClampAxis ClampAxis;
        public MinMax MinMax;

        public Clamps(ClampAxis clampAxis, MinMax minMax)
        {
            this.ClampAxis = clampAxis;
            this.MinMax = minMax;
        }
    }

    public class Dragable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerDragHandler
    {
        [SerializeField]
        private MoveType moveType;

        [SerializeField]
        private float followSpeed;

        [Header("Constraining")]

        [SerializeField]
        private Constrains[] constrains;

        [Header("Clamping")]

        [SerializeField]
        private Clamps[] clamps;

        private new Rigidbody2D rigidbody;
        private float startGravity = 8f;

        private Coroutine coroutine;

        private Transform pivotObject;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            pivotObject = new GameObject("Pivot Object").transform;
            pivotObject.parent = transform;
        }

        public void OnPointerDrag(Vector2 position, Vector2 lastPosition)
        {
            switch (moveType)
            {
                case MoveType.Transform:
                    var constrains = ReturnConstrains(ConstrainTypes.Position);

                    MinMax minMax;

                    if (transform.parent != null)
                        position = transform.parent.worldToLocalMatrix.MultiplyPoint(position);

                    if (Clamp(ClampAxis.X, out minMax))
                        position.x = Mathf.Clamp(position.x, minMax.Min, minMax.Max);
                    if (Clamp(ClampAxis.Y, out minMax))
                        position.y = Mathf.Clamp(position.y, minMax.Min, minMax.Max);

                    if (transform.parent != null)
                        position = transform.parent.localToWorldMatrix.MultiplyPoint(position);

                    transform.position = new Vector2(
                        constrains.X
                        ? transform.position.x
                        : position.x,
                        constrains.Y
                        ? transform.position.y
                        : position.y);
                    break;

                case MoveType.Velocity:
                    if (coroutine != null)
                        StopCoroutine(coroutine);
                    coroutine = StartCoroutine(MoveToPositionCo(position));

                    IEnumerator MoveToPositionCo(Vector2 position)
                    {
                        while (true)
                        {
                            rigidbody.velocity = Vector2.zero;
                            rigidbody.angularVelocity = 0;

                            var direction = position - (Vector2)pivotObject.position;
                            var velocity = direction * followSpeed * Time.fixedDeltaTime;

                            rigidbody.velocity = velocity;
                            yield return null;
                        }
                    }

                    break;
            }
        }

        public void OnPointerDown(Systems.PointerType pointerType, Vector3 mousePosition)
        {
            if (pointerType != Systems.PointerType.left)
                return;
            if (rigidbody == null)
                return;

            rigidbody.gravityScale = 0f;
            pivotObject.transform.position = mousePosition;
        }

        public void OnPointerUp(Systems.PointerType pointerType)
        {
            if (pointerType != Systems.PointerType.left)
                return;
            if (rigidbody == null)
                return;

            if (coroutine != null)
                StopCoroutine(coroutine);

            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0;

            rigidbody.gravityScale = startGravity;
        }

        private Constrains ReturnConstrains(ConstrainTypes constrainTypes)
        {
            foreach (var c in constrains)
            {
                if (c.ConstrainType == constrainTypes)
                {
                    return c;
                }
            }

            return default;
        }
        private bool Clamp(ClampAxis clampAxis, out MinMax minMax)
        {
            foreach (var c in clamps)
            {
                if (c.ClampAxis == clampAxis)
                {
                    minMax = c.MinMax;
                    return true;
                }
            }

            minMax = default;
            return false;
        }
    }
}
