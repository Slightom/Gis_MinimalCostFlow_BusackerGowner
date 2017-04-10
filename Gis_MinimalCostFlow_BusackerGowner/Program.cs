// This program has been made for university classes
// It solve problem of minimum cost flow, perfect bipartite matching using Successive Shortest Path Algorithm, Busackera-Gowena
//
// Input: In the first line of the file there are two integers n and m (2 = <n, m <= 200) respectively of programs and computers respectively. 
// Computers and programs are numbered with natural numbers.
// In the next n lines, there are written down the cost of execution time of the programs on the individual computers.
// Cij are integers (1 = <cij <= 100000). In line i + 1 and in column j the cost of executing Pi on the Kj computer is written.
//
// Output: In the output file we save the first n lines of numbers indicating the resultant optimal allocation of programs to computers. 
// The second line contains one number indicating the total cost of executing all programs on the allocated computers.

// Example:
// in.txt
// 3 3
// 1 3 2
// 2 6 4
// 3 7 6
// out. txt
// 2 3 1
// 10


// Time complexity: (C*n^2*log(n) + C*n^2*m) -> (n^2 + n^3). You can improve it by implement dijkstry using fibonacci mound

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gis_MinimalCostFlow_BusackerGowner
{
    public class Heap // priority Queue, needed for dijkstry
    {
        int[] h;
        int heapSize;
        int heapLength;

        public Heap(int n)               //               0
        {                                //             /   \
            h = new int[n];              //            1     2
            for (int i = 0; i < n; i++)  //           / \   / \
            { h[i] = i; }                //          3   4 5   6
            heapSize = n;                //
            heapLength = n;              //
        }                                //

        public void repair1(int v) // putting our top on the heap top
        {
            h[0] = h[v];
            h[v] = 0;
        }

        public bool empty()
        {
            return heapSize == 0 ? true : false;
        }

        public int top()
        {
            if (!empty())
            {
                return h[0];
            }
            return -1;
        }

        public void step()
        {
            h[0] = h[--heapSize];
        }

        public void repair(int[] hp, int[] d)
        {
            int left, right, parent = 0, dmin, pmin, x;
            while (true)
            {
                left = parent + parent + 1;
                right = left + 1;
                if (left >= heapSize) break;
                dmin = d[h[left]];
                pmin = left;
                if ((right < heapSize) && (dmin > d[h[right]])) // if right son exists and has shorter route
                {
                    dmin = d[h[right]];
                    pmin = right;
                }
                if (d[h[parent]] <= dmin) break;
                x = h[parent]; h[parent] = h[pmin]; h[pmin] = x;
                hp[h[parent]] = parent; hp[h[pmin]] = pmin;
                parent = pmin;
            }
        }

        public void repair3(int[] hp, List<Edge>[] li, int[] d, int v)
        {
            int child, parent, x;

            for (child = hp[v]; child > 0; child = parent)
            {
                parent = child / 2;
                if (d[h[parent]] <= d[h[child]]) break;
                x = h[parent]; h[parent] = h[child]; h[child] = x;
                hp[h[parent]] = parent; hp[h[child]] = child;
            }
        }


    }
    public class Edge // needed for list of incidence
    {
        public int end { get; set; }
        public int cost { get; set; }
        public int capacity { get; set; }

        public Edge() { }

        public Edge(int parameterEnd, int parameterCost, int parameterCapacity)
        {
            this.end = parameterEnd;
            this.cost = parameterCost;
            this.capacity = parameterCapacity;
        }
    }

    class Graph
    {
        public int n { get; set; }

        public int[] e;
        public List<Edge>[] listEdge { get; set; }

        public Graph() // neded for tests
        {
            n = 4;
            e = new int[4];
            e[0] = 4;
            e[3] = -4;
            this.listEdge = new List<Edge>[4];
            for (int k = 0; k < n; k++)
            {
                listEdge[k] = new List<Edge>();
            }

            Edge edge;
            edge = new Edge(1, 2, 4); listEdge[0].Add(edge);
            edge = new Edge(2, 2, 2); listEdge[0].Add(edge);

            edge = new Edge(2, 1, 2); listEdge[1].Add(edge);
            edge = new Edge(3, 3, 3); listEdge[1].Add(edge);

            edge = new Edge(3, 1, 5); listEdge[2].Add(edge);
        }

        public Graph(string parameterFileName)
        {
            readGraphFromFile(parameterFileName);
        }

        private void readGraphFromFile(string fileName)
        {
            string[] fileLine = System.IO.File.ReadAllLines(fileName);
            string[] line;

            line = fileLine[0].Split();

            this.n = Int32.Parse(line[0]);
            this.e = new int[this.n*2 +2];
            this.listEdge = new List<Edge>[2 * n + 2];
            for (int k = 0; k < 2 * n + 2; k++)
            {
                listEdge[k] = new List<Edge>();
            }

            Edge edge;
            int i, j, length;

            // adding edges
            for (i = 1; i <= n; i++)
            {
                line = fileLine[i].Split();
                length = line.Length - 1;
                for (j = 0; j < length; j++)
                {
                    edge = new Edge(j + 1 + this.n, Int32.Parse(line[j]), 1);
                    listEdge[i].Add(edge);
                }
            }

            // adding 0-cost edges
            for (i = 1; i <= n; i++)
            {
                edge = new Edge(i, 0, 1);
                listEdge[0].Add(edge);
                //edge = new Edge(0, 0, 1);
                //listEdge[i].Add(edge);

                edge = new Edge(2 * n + 1, 0, 1);
                listEdge[i + n].Add(edge);
                //edge = new Edge(i + n, 0, 1);
                //listEdge[2*n+1].Add(edge);
            }

            #region display edges
            //for (i = 0; i <= 2 * n + 1; i++)
            //{
            //    foreach (var e in listEdge[i])
            //    {
            //        Console.Write("(" + e.cost + "," + e.capacity + "," + e.end + ") ");
            //    }
            //    Console.WriteLine();
            //}
            #endregion
        }
    }



    class BusackerGownerAlgorithm
    {
        public string fileName { get; set; }

        private Graph g;              // residual network
        private int[,] originalEdge;  // needed on the end to read costs
        private int minimum;          // minimum flow we can push through  determined flow
        private List<int> listN;
        private List<int> listD;
        private int[] d;              // dijkstry
        private int k=0;              // super source
        private int l;
        private int[] p;              // table of precursor - from dijkstry
        private List<int> listP;      // actual flow
        private int[] resultTab;
        private int resultCost=0;

        public BusackerGownerAlgorithm()
        {
            this.listN = new List<int>();
            this.listD = new List<int>();
            this.listP = new List<int>();
            g = new Graph();           
            this.d = new int[g.n];
            this.p = new int[g.n];

            listN.Add(0);
            listD.Add(g.n - 1);
        } // needed for tests

        public BusackerGownerAlgorithm(string parameterFileName)
        {
            this.fileName = parameterFileName;
            this.listN = new List<int>();
            this.listD = new List<int>();
            this.listP = new List<int>();

            g = new Graph(fileName);
            this.resultTab = new int[g.n];
            loadOriginalEdges();
            this.d = new int[2 * g.n + 2];
            this.p = new int[2 * g.n + 2];

            //for (int i = 1; i <= g.n; i++)
            //{
            //    listN.Add(i);
            //    g.e[i] = 1;
            //    listD.Add(i + g.n);
            //    g.e[i + g.n] = -1;
            //}

            g.e[0] = g.n;
            g.e[g.n * 2 + 1] = -g.n;
            listN.Add(0);
            listD.Add(g.n * 2 + 1);
        }

        private void loadOriginalEdges()
        {
            this.originalEdge = new int[g.n, g.n];
            int i;

            for(i=1; i<=g.n; i++)
            {
                foreach(var e in g.listEdge[i])
                {
                    originalEdge[i - 1, e.end - g.n - 1] = e.cost;
                }
            }
        }

        public void runAlgorithm()
        {

            while(listN.Count>0)
            {
                k = listN.First();
                l = listD.First();
                dijkstry(2*g.n+2);
                //dijkstry(4);
                reduceCosts();
                setP();
                setMinimum();
                increaseFlowOnP(); // this method also update graph, "e", listN, listD
            }


            // printing result
            for(int i=0; i<g.n; i++)
            {
                Console.Write(resultTab[i] + " ");
                resultCost += originalEdge[i, resultTab[i] - 1];
            }
            Console.WriteLine("\n" + resultCost);
        }


        private void increaseFlowOnP()
        {
            int i, eStart, eEnd;
            Edge e, eBack;
            List<Edge>[] li = g.listEdge;

            for(i=0; i<listP.Count()-1; i++)
            {
                eStart = listP[i];
                eEnd = listP[i + 1];
                e = li[eStart].Where(p => p.end == eEnd).First();
                e.capacity -= minimum;

                if(li[eEnd].Where(p => p.end == eStart).Count()>0) // if already exists
                {
                    eBack = li[eEnd].Where(p => p.end == eStart).First();
                    eBack.capacity += minimum;
                }
                else
                {
                    eBack = new Edge(eStart, e.cost, minimum);
                    li[eEnd].Add(eBack);
                }

                if(e.capacity <= 0)
                {
                    li[eStart].Remove(e);
                }
            }

            g.e[k] -= minimum;
            g.e[l] += minimum;

            if (g.e[k] == 0) { listN.Remove(k); }
            if (g.e[l] == 0) { listD.Remove(l); }
        }

        private void setMinimum()
        {
            if (g.e[k] < minimum) { minimum = g.e[k]; }
            else if (g.e[l]*(-1) < minimum) { minimum = g.e[l]*(-1); }
        }

        private void setP()
        {
            int v = l;
            minimum = Int32.MaxValue;
            List<Edge>[] li = g.listEdge;
            Edge e;

            listP.Clear();
            listP.Add(v);

            do
            {
                e = li[p[v]].Where(p => p.end == v).First();
                if (e.capacity < minimum) { minimum = e.capacity; }

                v = p[v];
                listP.Add(v);
            }
            while (v != k);

            listP.Reverse();

            int length = listP.Count();
            for(int i=1; i<length-1; i+=2)
            {
                resultTab[listP[i]-1] = listP[i + 1]-g.n;
            }
        }

        private void reduceCosts()
        {
            List<Edge>[] li = g.listEdge;
            for(int i=0; i< g.n * 2 + 1; i++) //g.n*2+1
            {
                foreach(Edge e in li[i])
                {
                    e.cost = e.cost - (d[e.end] - d[i]);
                }
            }
        }

        private void dijkstry(int n) // determine shortest routes from k to every top
        {
            List<Edge>[] li = g.listEdge;
            int j, u;
            Heap heap;
            bool[] QS;
            int[] hp;


            heap = new Heap(n);
            QS = new bool[n];
            hp = new int[n];
            for (j = 0; j < n; j++)
            {
                QS[j] = false;
                d[j] = Int32.MaxValue;
                hp[j] = j;
            }

            d[k] = 0; //  step 4
            heap.repair1(k); // set v in HeapTop and repair Heap
            hp[0] = k;
            hp[k] = 0;

            for (j = 1; j < n; j++)
            {
                u = heap.top(); // we take the minimum, HeapTop
                heap.step(); // remove heaptop, insert there the last top
                hp[heap.top()] = k;
                heap.repair(hp, d);
                
                QS[u] = true;


                foreach (Edge edge in li[u])
                {
                    if (!QS[edge.end] && (d[edge.end] > d[u] + edge.cost))
                    {
                        d[edge.end] = d[u] + edge.cost;
                        p[edge.end] = u;
                        heap.repair3(hp, li, d, edge.end);
                    }
                }
            }

            //for(int i=0; i<g.n*2+2; i++)
            //{
            //    if (d[i] == Int32.MaxValue)
            //        d[i] = 0;
            //}

            #region show shourtest routs
            //for (int i = 0; i < n; i++)  // show shortest routes
            //{
            //    for (j = 0; j < n; j++)
            //    {
            //        Console.Write("{0,4}", d[i, j]);
            //    }
            //    Console.WriteLine("\n");
            //}
            #endregion
        }

    }

class Program
{
    static void Main(string[] args)
    {
            BusackerGownerAlgorithm busackerGownerAlgorithm = new BusackerGownerAlgorithm("in2.txt");
            //BusackerGownerAlgorithm busackerGownerAlgorithm = new BusackerGownerAlgorithm();
            busackerGownerAlgorithm.runAlgorithm();
    }
}
}
