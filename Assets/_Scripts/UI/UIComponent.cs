using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

namespace UI
{
    public enum ScaleableUIComponentType
    {
        normal,
        hover,
    }

    [Serializable]
    public struct ScaleableUIComponentConfig
    {
        public ScaleableUIComponentType ScaleableUIComponentType;
        public Vector3 Scale;
    }

    [RequireComponent(typeof(RectTransform))]
    public class UIComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("General")]
        [SerializeField]
        private Image[] images;

        [SerializeField]
        private TextMeshProUGUI[] text;

        [Header("Coloring")]

        [SerializeField]
        private Color hoverColor;
        public Color HoverColor => hoverColor;

        [SerializeField]
        private Color normalColor;
        public Color NormalColor => normalColor;

        [SerializeField]
        private Color disabledColor;
        public Color DisabledColor => disabledColor;

        [Header("Scaling")]

        [SerializeField]
        private ScaleableUIComponentConfig[] scaleableUIComponentConfig;

        private bool isActivated = false;
        private bool isInteractable = true;

        private RectTransform rectTransform;

        public event Action Clicked;

        private ScaleableUIComponentConfig ReturnScaleableUIComponentConfig(ScaleableUIComponentType scaleableUIComponentType)
        {
            foreach (var s in scaleableUIComponentConfig)
            {
                if (s.ScaleableUIComponentType == scaleableUIComponentType)
                    return s;
            }

            return new ScaleableUIComponentConfig();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (isActivated)
                return;
            if (!isInteractable)
                return;

            if (scaleableUIComponentConfig.Length > 0)
            {
                var scale = ReturnScaleableUIComponentConfig(ScaleableUIComponentType.hover).Scale;
                transform.localScale = scale;
            }

            SetImageColor(hoverColor);
            SetTextColor(hoverColor);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (isActivated)
                return;
            if (!isInteractable)
                return;

            if (scaleableUIComponentConfig.Length > 0)
            {
                var scale = ReturnScaleableUIComponentConfig(ScaleableUIComponentType.normal).Scale;
                transform.localScale = scale;
            }

            SetImageColor(normalColor);
            SetTextColor(normalColor);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable)
                return;

            Clicked?.Invoke();
        }

        public void Interactable(bool state)
        {
            if (state)
            {
                SetImageColor(normalColor);
                SetTextColor(normalColor);
            }
            else
            {
                SetImageColor(disabledColor);
                SetTextColor(disabledColor);
            }

            isInteractable = state;
        }

        public void Activate()
        {
            isActivated = true;

            SetImageColor(hoverColor);
            SetTextColor(hoverColor);
        }

        public void Deactivate()
        {
            isActivated = false;

            SetImageColor(normalColor);
            SetTextColor(normalColor);
        }

        private void SetImageColor(Color color)
        {
            foreach (var i in images)
                i.color = color;
        }

        private void SetTextColor(Color color)
        {
            foreach (var t in text)
                t.color = color;
        }

        private void OnDisable()
        {
            OnPointerExit(null);
        }
    }
}
