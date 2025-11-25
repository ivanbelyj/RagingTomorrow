/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public abstract class ApexLayout : ApexSerializedField, IEnumerable<ApexSerializedField>, IEnumerable, ICollection
    {
        protected List<ApexSerializedField> children;

        public ApexLayout(SerializedProperty serializedProperty, List<ApexSerializedField> children) : base(serializedProperty)
        {
            this.children = children;
        }

        public ApexLayout(SerializedProperty serializedProperty, params ApexSerializedField[] children) : base(serializedProperty)
        {
            this.children = new List<ApexSerializedField>(children.Length);
            for (int i = 0; i < children.Length; i++)
            {
                this.children.Add(children[i]);
            }
        }

        #region [ApexSerializedField Implemetation]
        public override void DrawFieldLayout()
        {
            Rect position = GUILayoutUtility.GetRect(0, GetFieldHeight());
            DrawField(position);
        }

        public override bool IsVisible()
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].IsVisible())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region [List<T> Implementation]
        public virtual void Add(ApexSerializedField field)
        {
            children.Add(field);
        }

        public virtual void RemoveAt(int index)
        {
            children.RemoveAt(index);
        }

        public virtual void Remove(ApexSerializedField field)
        {
            children.Remove(field);
        }
        #endregion

        #region [IEnumerable & IEnumerable<T>]
        public IEnumerator GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator<ApexSerializedField> IEnumerable<ApexSerializedField>.GetEnumerator()
        {
            return children.GetEnumerator();
        }
        #endregion

        #region [ICollection & IReadOnlyCollection<T> Implementation]
        public int Count
        {
            get
            {
                return ((ICollection)children).Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)children).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)children).SyncRoot;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            ((ICollection)children).CopyTo(array, index);
        }

        #endregion

        #region [Getter / Setter]
        public List<ApexSerializedField> GetChildren()
        {
            return children;
        }

        public void SetChildren(List<ApexSerializedField> value)
        {
            children = value;
        }

        public ApexSerializedField GetChild(int index)
        {
            return children[index];
        }

        public void SetChild(int index, ApexSerializedField value)
        {
            children[index] = value;
        }
        #endregion
    }
}