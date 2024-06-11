using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interacting
{
    public interface IPointerUpHandler
    {
        public void OnPointerUp(Systems.PointerType pointerType);
    }
}
