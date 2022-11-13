using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Битовая матрица, хранящая данные о заполненности секции инвентаря и предоставляющая методы,
/// связанные с заполненностью
/// </summary>
[Serializable]
public class FillingMatrix
{
    private BitArray[] _data;

    [SerializeField]
    private int _cols;
    [SerializeField]
    private int _rows;

    public int Width => _cols;
    public int Height => _rows;

    public FillingMatrix(int rows, int cols) {
        _data = new BitArray[rows];
        _cols = cols;
        _rows = rows;
        for (int i = 0; i < rows; i++) {
            _data[i] = new BitArray(cols);
        }
    }

    public bool this[int row, int col] {
        get => _data[row][col];
        set => _data[row][col] = value;
    }

    /// <summary>
    /// Устанавливает данное значение в позициях матрицы заполнения, соответствующих размеру 
    /// и позиции прямоугольника
    /// </summary>
    public void SetRect(int width, int height, int x, int y, bool val) {
         for (int row = y; row < y + height; row++) {
            for (int col = x; col < x + width; col++) {
                this[row, col] = val;
            }
        }
    }

    /// <summary>
    /// Проверяет, не занято ли место в матрице заполненности для данного прямоугольника.
    /// true, если место не занято
    /// </summary>
    public bool HasPlaceForRect(int width, int height, int x, int y) {
        // Если проверяемая на занятость область выходит за пределы, прямоугольник
        // гарантированно не поместится в нее, т.к. она урезана границами матрицы
        if (x >= _cols || y >= _rows
            || x + width > _cols || y + height > _rows)
            return false;
        for (int row = y; row < y + height; row++) {
            for (int col = x; col < x + width; col++) {
                // Если позиция заполнена, invItem не поместить в заданную область
                if (this[row, col]) {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Ищет свободное место для прямоугольника заданного размера.
    /// true, если в секции нашлось такое место
    /// </summary>
    public bool FindFreeRectPos(int width, int height, out int x, out int y) {
        for (int row = 0; row < _data.Length; row++) {
            for (int col = 0; col < this._cols; col++) {
                if (HasPlaceForRect(width, height, col, row)) {
                    x = col;
                    y = row;
                    return true;
                }
            }
        }
        x = y = 0;
        return false;
    }
}
