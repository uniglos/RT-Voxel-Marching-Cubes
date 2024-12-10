using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Chat GPT A* pathfinding tailored to suit my VoxelGrid

public static class VoxelPathFind
{
    public static List<Vector3> PathFind(Vector3[] positions, Vector3 targetPosition, float voxelSize)
    {
        HashSet<Vector3> positionSet = new HashSet<Vector3>(positions); // For quick lookup
        Vector3[] Directions = {
        Vector3.right,
        Vector3.left,
        Vector3.up,
        Vector3.down,
        Vector3.forward,
        Vector3.back
    };
        List<Vector3> ConfirmedPositions = new List<Vector3>();
        foreach (Vector3 startPosition in positions)
        {
            Dictionary<Vector3, float> gCosts = new Dictionary<Vector3, float> { { startPosition, 0 } };
            Dictionary<Vector3, float> fCosts = new Dictionary<Vector3, float> { { startPosition, Heuristic(startPosition, targetPosition) } };
            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

            PriorityQueue<Vector3> openSet = new PriorityQueue<Vector3>();
            openSet.Enqueue(startPosition, fCosts[startPosition]);

            HashSet<Vector3> closedSet = new HashSet<Vector3>();
            bool isPathFound = false;

            while (openSet.Count > 0)
            {
                Vector3 current = openSet.Dequeue();
                if (current == targetPosition)
                {
                    isPathFound = true;
                    break;
                }

                closedSet.Add(current);

                foreach (Vector3 direction in Directions)
                {
                    Vector3 neighbor = current + direction * voxelSize;

                    if (!positionSet.Contains(neighbor) || closedSet.Contains(neighbor))
                        continue;

                    float tentativeGCost = gCosts[current] + Vector3.Distance(current, neighbor);

                    if (!gCosts.ContainsKey(neighbor) || tentativeGCost < gCosts[neighbor])
                    {
                        gCosts[neighbor] = tentativeGCost;
                        float hCost = Heuristic(neighbor, targetPosition);
                        fCosts[neighbor] = tentativeGCost + hCost;
                        cameFrom[neighbor] = current;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fCosts[neighbor]);
                        }
                    }
                }
            }

            if (isPathFound)
            {
               ConfirmedPositions.Add(startPosition);
            }
        }
        return ConfirmedPositions;
    }

    // Heuristic: Estimated cost from current position to target
    private static float Heuristic(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to); // Euclidean distance
    }

    // Priority queue implementation
    public class PriorityQueue<T>
    {
        private readonly SortedDictionary<float, Queue<T>> elements = new SortedDictionary<float, Queue<T>>();

        public int Count { get; private set; }

        public void Enqueue(T item, float priority)
        {
            if (!elements.ContainsKey(priority))
            {
                elements[priority] = new Queue<T>();
            }

            elements[priority].Enqueue(item);
            Count++;
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
                throw new System.InvalidOperationException("The priority queue is empty.");

            var firstPair = elements.First();
            var item = firstPair.Value.Dequeue();
            if (firstPair.Value.Count == 0)
            {
                elements.Remove(firstPair.Key);
            }

            Count--;
            return item;
        }

        public bool Contains(T item)
        {
            foreach (var pair in elements)
            {
                if (pair.Value.Contains(item))
                    return true;
            }
            return false;
        }
    }
}
