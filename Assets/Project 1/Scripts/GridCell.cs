using Events;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public SpriteRenderer BackgroundSprite;
    public SpriteRenderer XSprite;

    private int m_NeighborCount;

    public int Index1, Index2;

    private void OnMouseDown()
    {
        XSprite.enabled = true;

        using var gridEvt = GridEvent.Get(Index1, Index2);
        {
            gridEvt.SendGlobal();
        }
    }
}