using System.Collections.Generic;
using UnityEngine;

public class WeaponTemplatePivots : MonoBehaviour
{
    [SerializeField]
    private Transform body;

    [SerializeField]
    private Transform magazine;

    [SerializeField]
    private Transform trigger;

    [SerializeField]
    private Transform centerPivot;

    public void Configurate(List<Transform> bodies, List<Transform> magazines, List<Transform> triggers)
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].SetParent(body);
        }
        for (int i = 0; i < magazines.Count; i++)
        {
            magazines[i].SetParent(magazine);
        }
        for (int i = 0; i < triggers.Count; i++)
        {
            triggers[i].SetParent(trigger);
        }
    }

    #region [Getter / Setter]
    public Transform GetCenterPivot()
    {
        return centerPivot;
    }
    #endregion
}
