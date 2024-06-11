using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interacting
{
    public interface IPointerDragHandler
    {
        public void OnPointerDrag(Vector2 position, Vector2 lastPosition);
    }
}
