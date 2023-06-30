using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void RemoveAllChildren(Transform transform)
    {
        int totalChild = transform.childCount;
        for (int index = totalChild - 1; index >= 0; index--)
        {
            GameObject.Destroy(transform.GetChild(index).gameObject);
        }
    }
}
