using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interacting
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Throwable : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        private new Rigidbody2D rigidbody;

        private Vector3 lastMousePosition;

        private Vector3 delta;
        private Vector3 lastDelta;

        private bool update = false;

        private Systems.InteractionManager interactionManager;

        [SerializeField]
        private float maxThroughSpeed;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            interactionManager = Systems.InteractionManager.Current;
        }

        private void FixedUpdate()
        {
            if (!update)
                return;

            lastDelta = delta;

            var mousePosition = interactionManager.ReturnMousePosition();
            delta = (mousePosition - lastMousePosition) / Time.fixedDeltaTime;
            lastMousePosition = mousePosition;
        }

        public void OnPointerDown(Systems.PointerType pointerType, Vector3 mousePosition)
        {
            lastMousePosition = interactionManager.ReturnMousePosition();
            update = true;
        }

        public void OnPointerUp(Systems.PointerType pointerType)
        {
            if (pointerType != Systems.PointerType.left)
                return;

            var averageDelta = Vector3.Lerp(delta, lastDelta, 0.5f);
            var clampedDelta = Vector3.MoveTowards(rigidbody.velocity, averageDelta, maxThroughSpeed);
            rigidbody.AddForce(clampedDelta, ForceMode2D.Impulse);
        }
    }
}
