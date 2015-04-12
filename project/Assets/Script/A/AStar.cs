using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public class AStar
    {
        private float PERCEPTION_THRESHOLD = 6.0f;
		private float AVERAGEHUMANHEIGHT = 6.0f;
		private Heap<Node> openSet;
		private HashSet<Node> closedSet;
		Grid grid;
        public AStar()
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
        }

        // Sense propogation using attenuation properties of the tiles
        public bool PropogateSense(Vector3 startPosition, Vector3 targetPosition)
        {
           

			Node startNode = grid.NodeFromWorldPoint(startPosition);
			Node targetNode = grid.NodeFromWorldPoint(targetPosition);
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                 Node currentNode = openSet.RemoveFirst();
				 closedSet.Add(currentNode);
				  
				  if (currentNode == targetNode)
                {
                     return true;
                }

				 if (currentNode.gCost > PERCEPTION_THRESHOLD) // Target out of perception?
                {
						return false;
				}
					
				foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
					
                    int newMovementCostToNeighbour = g_Function(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = h_Function(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            //return false;
        }
		return false;
    }

	// H 表示从指定的方格移动到终点 B 的预计耗费
	//h(n) 是从n到目标节点最佳路径的估计代价。
	int h_Function(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		//if there is a mountain?
		float nodeHeight = Terrain.activeTerrain.SampleHeight(nodeA.worldPosition);
		//if there is a building?

		if (dstX > dstY)
			return (14 + Mathf.RoundToInt(nodeHeight)) * dstY + (10 + Mathf.RoundToInt(nodeHeight)) * (dstX - dstY);
		return (14 + Mathf.RoundToInt(nodeHeight)) * dstX + (10 + Mathf.RoundToInt(nodeHeight)) * (dstY - dstX);
	}
	//G 表示从起点 A 移动到网格上指定方格的移动耗费 (可沿斜方向移动)
	//g(n) 是在状态空间中从初始节点到n节点的实际代价，
	int g_Function(Node currentNode, Node neighbour)
	{
		int g = currentNode.gCost + h_Function(currentNode, neighbour) + neighbour.TileAttenuation;
		return g;
	}

}

