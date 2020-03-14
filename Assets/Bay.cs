using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Bay
{
    public Product product;
    public int maxProductsDepth;
    public int maxProductsWidth;
    public Vector3 boundingBoxSize;
    public Vector3 boundingBoxOffset;
    public float gridScale;
}