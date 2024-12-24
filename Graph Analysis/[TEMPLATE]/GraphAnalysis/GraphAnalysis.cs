using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    public static class GraphAnalysis
    {

        #region YOUR CODE IS HERE
        //Your Code is Here:
        //==================
        /// <param name="vertices"></param>
        /// <param name="edges"></param>
        /// <param name="startVertex"></param>
        /// <param name="outputs"></param>

        /// <summary>
        /// Analyze the edges of the given DIRECTED graph by applying DFS starting from the given "startVertex" and count the occurrence of each type of edges
        /// NOTE: during search, break ties (if any) by selecting the vertices in ASCENDING alpha-numeric order
        /// </summary>
        /// <param name="vertices">array of vertices in the graph</param>
        /// <param name="edges">array of edges in the graph</param>
        /// <param name="startVertex">name of the start vertex to begin from it</param>
        /// <returns>return array of 3 numbers: outputs[0] number of backward edges, outputs[1] number of forward edges, outputs[2] number of cross edges</returns>

        static Dictionary<string, List<string>> hashedEdges;
        static Dictionary<string, char> vertexColorStatus = new Dictionary<string, char>();
        static int[] edgeOccurences;
        static Stack<string> adjacentVertices = new Stack<string>();

        public static int[] AnalyzeEdges(string[] vertices, KeyValuePair<string, string>[] edges, string startVertex)
        {
            edgeOccurences = new int[3];
            return DepthFirstSearch(vertices, edges, startVertex);
        }

        public static int[] DepthFirstSearch(string[] vertices, KeyValuePair<string, string>[] edges, string startVertex)
        {
            hashedEdges = new Dictionary<string, List<string>>(vertices.Length);
            Parallel.Invoke(
                () =>
                {
                    setVertexHash(hashedEdges, edges);
                },
                () =>
                {
                    setVertexColorStatusWhite(vertices, vertexColorStatus);
                }
            );
            
            adjacentVertices.Push(startVertex);
            DepthFirstSearchTraversalLoop();

            Parallel.Invoke(
                () =>
                {
                    hashedEdges.Clear();
                },
                () =>
                {
                    vertexColorStatus.Clear();
                }
            );
            return edgeOccurences;
        }

        public static void DepthFirstSearchTraversalLoop()
        {
            while(adjacentVertices.Count > 0)
            {
                string topItem = adjacentVertices.Peek();
                if (vertexColorStatus[topItem] == 'G')
                {
                    vertexColorStatus[topItem] = 'B';
                    adjacentVertices.Pop();
                    continue;
                }
                else if (vertexColorStatus[topItem] == 'B')
                {
                    edgeOccurences[1]++;
                    adjacentVertices.Pop();
                    continue;
                }

                vertexColorStatus[topItem] = 'G';

                if (hashedEdges.ContainsKey(topItem))
                {
                    var enums = hashedEdges[topItem].OrderByDescending(x => x);
                    foreach(var item in enums)
                    {
                        if (vertexColorStatus[item] == 'W')
                        {
                            adjacentVertices.Push(item);
                        }
                        else if (vertexColorStatus[item] == 'G')
                        {
                            edgeOccurences[0]++;
                        }
                        else
                        {
                            edgeOccurences[2]++;
                        }
                    }
                }
                else
                {
                    vertexColorStatus[topItem] = 'B';
                    adjacentVertices.Pop();
                }
            }
        }

        public static void setVertexHash(Dictionary<string, List<string>> hashed, KeyValuePair<string, string>[] edges)
        {
            foreach(var edge in edges)
            {
                if (!hashed.ContainsKey(edge.Key))
                {
                    List<string> vertexNeighbours = new List<string>();
                    vertexNeighbours.Add(edge.Value);
                    hashed.Add(edge.Key, vertexNeighbours);
                }
                else
                {
                    hashed[edge.Key].Add(edge.Value);
                }
            }
        }

        public static void setVertexColorStatusWhite(string[] vertices, Dictionary<string, char> colorStatus)
        {
            foreach (var vertex in vertices)
            {
                colorStatus.Add(vertex, 'W');
            }
        }
        #endregion
    }
}