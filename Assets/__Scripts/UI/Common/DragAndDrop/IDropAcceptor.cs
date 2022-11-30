using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropAcceptor<T>
{
    public void AcceptDrop(Draggable<T> draggable, T draggedData);
}
