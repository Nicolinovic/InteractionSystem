using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interacting
{
    public interface IPointerDownHandler
    {
        public void OnPointerDown(Systems.PointerType pointerType, Vector3 mousePosition);
    }

}
