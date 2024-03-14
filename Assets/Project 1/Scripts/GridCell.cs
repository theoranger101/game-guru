using System;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public SpriteRenderer BackgroundSprite;
    public SpriteRenderer XSprite;

    private int m_NeighborCount;
    
    private void OnMouseDown()
    {
        XSprite.enabled = true;
    }
}