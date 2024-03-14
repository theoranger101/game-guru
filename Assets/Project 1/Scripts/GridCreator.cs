using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public GridCell CellPrefab;

    public int CellCount;

    public int PoolSize = 36;
    
    [SerializeField]
    private List<GridCell> m_Cells = new List<GridCell>();
    
    [Button]
    private void CreateGrid()
    {
        if (m_Cells.IsNullOrEmpty())
        {
            CreateCells(PoolSize);
        }

        var totalCellCount = CellCount * CellCount;
        if (PoolSize < totalCellCount)
        {
            var countToBeCreated = totalCellCount - PoolSize;
            CreateCells(countToBeCreated);
            PoolSize = totalCellCount;
        }

        var cam = Camera.main;
        
        var screenWidth = cam.aspect * cam.orthographicSize * 2; // calculates the width of the screen in world terms.
        var cellSize = screenWidth / CellCount;

        var topLeftCornerWorldPosition = cam.ViewportToWorldPoint(new Vector3(0f,1f, 0f));
        
        for (var i = 0; i < CellCount; i++)
        {
            for (var k = 0; k < CellCount; k++)
            {
                var cellObj = m_Cells[(CellCount * i) + k];
                var cellT = cellObj.transform;
                
                var screenPositionX = topLeftCornerWorldPosition.x + (cellSize * k) + cellSize * 0.5f;
                var screenPositionY = topLeftCornerWorldPosition.y - (cellSize * i) - cellSize * 0.5f;

                var worldPosition = new Vector3(screenPositionX, screenPositionY, 0f);

                cellT.position = worldPosition;
                cellT.localScale = new Vector3(cellSize * 1.5f, cellSize * 1.5f, 1f);
                cellObj.gameObject.SetActive(true);
            }
        }

        for (var i = totalCellCount; i < PoolSize; i++)
        {
            m_Cells[i].gameObject.SetActive(false);
        }
    }

    private void CreateCells(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var newCellObj = Instantiate(CellPrefab, transform);
            m_Cells.Add(newCellObj);
            
            newCellObj.gameObject.SetActive(false);
        }
    }
}