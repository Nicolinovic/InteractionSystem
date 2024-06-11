using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interacting
{
    public enum PushType
    {
        single,
        inTurns,
        mousePosition,
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Pushable : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private PushType pushType;

        [SerializeField]
        private Vector2[] forces;

        private int index;

        private new Rigidbody2D rigidbody;

        private Systems.InteractionManager interactionManager;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            interactionManager = Systems.InteractionManager.Current;
        }

        public void OnPointerDown(Systems.PointerType pointerType, Vector3 mousePosition)
        {
            if (pointerType != Systems.PointerType.right)
                return;

            rigidbody.velocity = Vector2.zero;

            switch (pushType)
            {
                case PushType.single:
                    rigidbody.AddForce(forces[0], ForceMode2D.Impulse);
                    break;
                case PushType.inTurns:
                    rigidbody.AddForce(forces[index], ForceMode2D.Impulse);
                    index = index < forces.Length - 1 ? index + 1 : 0;
                    break;
                case PushType.mousePosition:
                    var mousePoint = interactionManager.ReturnMousePosition();
                    var direction = (transform.position - mousePoint).normalized;

                    rigidbody.AddForce(direction * forces[0].x, ForceMode2D.Impulse);
                    break;
            }
        }
    }
}
