using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Класс, реализующий данный интерфейс, может установить камеру в соответствии с переданными
    /// параметрами типа кастомизируемого объекта
    /// (например, чтобы обеспечить вид от 1-го лица для изначально неизвестного кастомизируемого объекта)
    /// </summary>
    public interface ISetCameraToAppearance
    {
        public void SetCamera(Transform cameraBone, Vector3 cameraOffset);
    }
}
