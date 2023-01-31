using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Класс, реализующий данный интерфейс, может устанавливать новые смещения для камеры
    /// (например, чтобы обеспечить вид от 1-го лица для изначально неизвестного кастомизируемого объекта)
    /// </summary>
    public interface ISetCameraOffset
    {
        public void SetCameraOffset(Vector3 pos);
    }
}
