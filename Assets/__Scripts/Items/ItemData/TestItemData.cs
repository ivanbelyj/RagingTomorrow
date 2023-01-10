using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemData : ItemData, IEquatable<TestItemData>
{
    public string bookNotes = "";

    public override bool Equals(object other)
    {
        if (other is TestItemData testDynamicData) {
            return Equals(testDynamicData);
        }
        return false;
    }

    public bool Equals(TestItemData other)
    {
        return base.Equals((ItemData)other) && bookNotes == other.bookNotes;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.ItemStaticDataName, this.bookNotes);
    }
}
