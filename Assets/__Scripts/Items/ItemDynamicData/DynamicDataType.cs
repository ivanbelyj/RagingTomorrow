using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Для десериализации динамической информации о предмете (при передачи Mirror по сети)
/// требуется знать, объект какого типа пришел в виде байтов
/// </summary>
public enum DynamicDataType : byte
{
    None,  // Бывает, что динамической информации для предмета не предусмотрено
    Test
}
