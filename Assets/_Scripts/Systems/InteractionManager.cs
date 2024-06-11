using Interacting;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Systems
{
    public enum PointerType
    {
        left,
        right,
        none,
    }

    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Current;

        private new Camera camera;
        public Camera Camera => camera;

        #region Raycasting

        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private LayerMask whatIsInteractable;

        #endregion

        #region Hovered Objects

        private GameObject currentHoveredGameObject;
        public GameObject CurrentHavoredGameObject => currentHoveredGameObject;

        private GameObject lastHoveredGameObject;
        public GameObject LastHavoredGameObject => lastHoveredGameObject;

        #endregion

        #region Pointer Klick Handler

        private IPointerDownHandler[] pointerDownHandler;
        private IPointerUpHandler[] pointerUpHandler;

        #endregion

        #region Drag Handler
        private IPointerDragHandler[] pointerDragHandler;

        private bool updateDrag = false;

        private Vector3 mousePosition;
        private Vector3 lastMousePosition;
        #endregion

        [SerializeField]
        private PointerType currentPointerInteraction = PointerType.none;

        #region Visuals

        [SerializeField]
        private Transform cursorTexture;

        #endregion

        private void Awake()
        {
            camera = FindObjectOfType<Camera>();
            if (camera == null)
                Debug.LogError("Camera could not be found");

            if (Current != null && Current != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Current = this;
            }

            Cursor.visible = false;
        }

        private void Update()
        {
            if (cursorTexture != null)
            {
                var position = camera.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0f;
                cursorTexture.transform.position = position;

                cursorTexture.transform.localScale = Vector3.one * radius;
            }

            if (currentHoveredGameObject == null)
                return;

            HandlePointerKlick();
            HandlePointerDrag();
        }

        private void FixedUpdate()
        {
            if (currentPointerInteraction != PointerType.none)
                return;

            currentHoveredGameObject = ReturnHoveredGameObject();

            if (currentHoveredGameObject != null && currentHoveredGameObject != lastHoveredGameObject)
                GetReferences();

            lastHoveredGameObject = currentHoveredGameObject;
        }

        private void OnValidate()
        {
            if (cursorTexture != null)
                cursorTexture.localScale = Vector3.one * radius;
        }

        private void GetReferences()
        {
            pointerDownHandler = currentHoveredGameObject.GetComponents<IPointerDownHandler>();
            pointerUpHandler = currentHoveredGameObject.GetComponents<IPointerUpHandler>();
            pointerDragHandler = currentHoveredGameObject.GetComponents<IPointerDragHandler>();
        }

        private void HandlePointerKlick()
        {
            var leftMouse = KeyCode.Mouse0;
            var rightMouse = KeyCode.Mouse1;

            if (Input.GetKeyDown(leftMouse))
            {
                if (currentPointerInteraction == PointerType.right)
                {
                    foreach (var p in pointerUpHandler)
                        p.OnPointerUp(PointerType.right);
                }

                if (pointerDownHandler.Length > 0)
                {
                    foreach (var p in pointerDownHandler)
                        p.OnPointerDown(PointerType.left, ReturnMousePosition());
                }

                lastMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
                currentPointerInteraction = PointerType.left;
            }
            else if (Input.GetKeyUp(leftMouse))
            {
                if (pointerUpHandler != null)
                {
                    foreach (var p in pointerUpHandler)
                        p.OnPointerUp(PointerType.left);
                }

                if (currentPointerInteraction == PointerType.left)
                    currentPointerInteraction = PointerType.none;
            }

            if (Input.GetKeyDown(rightMouse))
            {
                if (currentPointerInteraction == PointerType.left)
                {
                    foreach (var p in pointerUpHandler)
                        p.OnPointerUp(PointerType.left);
                }

                if (pointerDownHandler.Length > 0)
                {
                    foreach (var p in pointerDownHandler)
                        p.OnPointerDown(PointerType.right, ReturnMousePosition());
                }

                currentPointerInteraction = PointerType.right;
            }
            else if (Input.GetKeyUp(rightMouse))
            {
                if (pointerUpHandler != null)
                {
                    foreach (var p in pointerUpHandler)
                        p.OnPointerUp(PointerType.right);
                }

                if (currentPointerInteraction == PointerType.right)
                    currentPointerInteraction = PointerType.none;
            }
        }

        private void HandlePointerDrag()
        {
            if (currentPointerInteraction != PointerType.left)
                return;

            mousePosition = ReturnMousePosition();

            if (lastMousePosition != mousePosition)
            {
                foreach (var p in pointerDragHandler)
                    p.OnPointerDrag(mousePosition, lastMousePosition);
            }

            lastMousePosition = mousePosition;
        }

        private GameObject ReturnHoveredGameObject()
        {
            var hitCollider = Physics2D.OverlapCircle(camera.ScreenToWorldPoint(Input.mousePosition), radius, whatIsInteractable);
            if (hitCollider != null)
            {
                var interactable = hitCollider.attachedRigidbody != null
                    ? hitCollider.attachedRigidbody.gameObject
                    : hitCollider.gameObject;

                return interactable;
            }

            return null;
        }

        public Vector3 ReturnMousePosition()
        {
            var position = camera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
            return position;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (camera == null)
                return;

            Handles.DrawWireArc(camera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, Vector3.right, 360, radius);
        }
#endif
    }
}
