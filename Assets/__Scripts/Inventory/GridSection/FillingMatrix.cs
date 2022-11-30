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
    // Прямоугольник, занимающий место в матрице
    public struct FillingRect {
        public int width;
        public int height;

        // Координаты верхнего левого угла прямоугольника в матрице
        public int x;
        public int y;
    }

    private BitArray[] _data;

    // [SerializeField]
    private int _cols;
    // [SerializeField]
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
        private set => _data[row][col] = value;
    }

    public void SetRect(FillingRect rect, bool val) {
        SetRect(rect.width, rect.height, rect.x, rect.y, val);
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
    /// true, если прямоугольник находится в рамках матрицы
    /// </summary>
    public bool IsRectInBounds(int width, int height, int x, int y) {
        return x > 0 && y > 0
            && x + width <= _cols
            && y + height <= _rows;
    }

    public bool IsRectInBounds(FillingRect rect) {
        return IsRectInBounds(rect.width, rect.height, rect.x, rect.y);
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

    public bool HasPlaceForRect(FillingRect rect) {
        return HasPlaceForRect(rect.width, rect.height, rect.x, rect.y);
    }

    /// <summary>
    /// Ищет свободное место для прямоугольника заданного размера.
    /// true, если в секции нашлось такое место.
    /// Асимптотика: O(n^2)
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

    /// <summary>
    /// Создает матрицу заполненности на основе данных о ее заполнении.
    /// Асимптотика: O(n)
    /// </summary>
    public static FillingMatrix Create(int rows, int cols, FillingRect[] fillingRects) {
        FillingMatrix res = new FillingMatrix(rows, cols);
        foreach (var rect in fillingRects) {
            /// На практике можно считать O(const), т.к. средний размер
            // прямоугольника заполнения - const
            res.SetRect(rect, true);
        }
        return res;
    }

    /// <summary>
    /// Возвращается копия прямоугольника, выходящего за границы матрицы заполнения,
    /// обрезанная по рамкам матрицы.
    /// </summary>
    public FillingRect ToBounds(FillingRect rect) {
        int rowOverflow = (rect.height + rect.y) - _rows;
        int colOverflow = (rect.width + rect.x) - _cols;
        return new FillingRect() {
            x = rect.x,
            y = rect.y,
            width = rect.width - colOverflow,
            height = rect.height - rowOverflow
        };
    }
}
