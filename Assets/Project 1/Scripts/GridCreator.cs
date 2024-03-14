using System.Collections.Generic;
using Events;
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

    private bool[,] GridElements;
    
    private List<GridCell> m_MatchedCells = new List<GridCell>();

    private void Awake()
    {
        CreateGridElements();
    }

    private void OnEnable()
    {
        GEM.AddListener<GridEvent>(OnGridEvent);
    }

    private void OnGridEvent(GridEvent evt)
    {
        var i1 = evt.Index1;
        var i2 = evt.Index2;

        GridElements[i1, i2] = true;
        
        CheckCardinalNeighbors(i1, i2);
        m_MatchedCells.Clear();
    }

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

        var topLeftCornerWorldPosition = cam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f));

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

                cellObj.Index1 = i;
                cellObj.Index2 = k;
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

    private void CreateGridElements()
    {
        GridElements = new bool[CellCount, CellCount];
    }

    private void CheckCardinalNeighbors(int row, int col, int depth = 0)
    {
        if (depth > 1)
        {
            return;
        }
        
        int[] dr = { -1, 1, 0, 0 }; // deltas for up, down, left, right
        int[] dc = { 0, 0, -1, 1 };
        
        for (var i = 0; i < 4; i++)
        {
            var newRow = row + dr[i];
            var newCol = col + dc[i];

            if (newRow < 0 || newRow >= CellCount || newCol < 0 || newCol >= CellCount) continue; // skip invalid indexes
            
            if (GridElements[newRow, newCol] && !m_MatchedCells.Contains(m_Cells[(newRow * CellCount) + newCol]))
            {
                m_MatchedCells.Add(m_Cells[(newRow * CellCount) + newCol]);
                
                // if match has been made, recursively check the other cardinal neighbors for additional matches.
                CheckCardinalNeighbors(newRow, newCol, depth + 1);
            }
        }
        
        if (m_MatchedCells.Count < 3) return;
        ClearMatchedCells(row, col);
    }

    private void ClearMatchedCells(int row, int col)
    {
        for (var i = 0; i < m_MatchedCells.Count; i++)
        {
            m_MatchedCells[i].XSprite.enabled = false;
            GridElements[m_MatchedCells[i].Index1, m_MatchedCells[i].Index2] = false;
        }

        m_Cells[(row * CellCount) + col].XSprite.enabled = false;
        GridElements[row, col] = false;
    }
}