using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GraphGenerator;

namespace Problem
{

    public class Problem : ProblemBase, IProblem
    {
        #region ProblemBase Methods
        public override string ProblemName { get { return "GraphAnalysis"; } }

        public override void TryMyCode()
        {            
            //Case1
            string[] vertices1 = { "A1", "A2", "A3"};
            KeyValuePair<string, string>[] edges1 = new KeyValuePair<string, string>[2];
            edges1[0] = new KeyValuePair<string, string>("A1", "A2");
            edges1[1] = new KeyValuePair<string, string>("A2", "A3");
            int[] expected1 = { 0, 0, 0 };
            int[] output1 = GraphAnalysis.AnalyzeEdges(vertices1, edges1, "A1");
            PrintCase(vertices1, edges1, output1, expected1);

            //Case2
            string[] vertices2 = { "A", "B", "C", "D", "E", "F", "G" };
            KeyValuePair<string, string>[] edges2 = new KeyValuePair<string, string>[8];
            edges2[0] = new KeyValuePair<string, string>("A", "B");
            edges2[1] = new KeyValuePair<string, string>("B", "C");
            edges2[2] = new KeyValuePair<string, string>("B", "D");
            edges2[3] = new KeyValuePair<string, string>("B", "E");
            edges2[4] = new KeyValuePair<string, string>("C", "E");
            edges2[5] = new KeyValuePair<string, string>("D", "E");
            edges2[6] = new KeyValuePair<string, string>("E", "F");
            edges2[7] = new KeyValuePair<string, string>("G", "D");
            int[] expected2 = { 0, 1, 1 };
            int[] output2 = GraphAnalysis.AnalyzeEdges(vertices2, edges2, "A");
            PrintCase(vertices2, edges2, output2, expected2);

            //Case3
            string[] vertices3 = { "A", "B", "C", "D", "E", "F"};
            KeyValuePair<string, string>[] edges3 = new KeyValuePair<string, string>[6];
            edges3[0] = new KeyValuePair<string, string>("A", "B");
            edges3[1] = new KeyValuePair<string, string>("B", "C");
            edges3[2] = new KeyValuePair<string, string>("E", "D");
            edges3[3] = new KeyValuePair<string, string>("E", "F");
            edges3[4] = new KeyValuePair<string, string>("C", "E");
            edges3[5] = new KeyValuePair<string, string>("D", "B");
            int[] expected3 = { 1, 0, 0 };
            int[] output3 = GraphAnalysis.AnalyzeEdges(vertices3, edges3, "A");
            PrintCase(vertices3, edges3, output3, expected3);

            //Case4
            string[] vertices4 = { "A", "B", "C", "D", "E", "F", "G" };
            KeyValuePair<string, string>[] edges4 = new KeyValuePair<string, string>[10];
            edges4[0] = new KeyValuePair<string, string>("A", "D");
            edges4[1] = new KeyValuePair<string, string>("A", "C");
            edges4[2] = new KeyValuePair<string, string>("A", "B");
            edges4[3] = new KeyValuePair<string, string>("B", "D");
            edges4[4] = new KeyValuePair<string, string>("C", "E");
            edges4[5] = new KeyValuePair<string, string>("D", "F");
            edges4[6] = new KeyValuePair<string, string>("E", "F");
            edges4[7] = new KeyValuePair<string, string>("E", "G");
            edges4[8] = new KeyValuePair<string, string>("G", "D");
            edges4[9] = new KeyValuePair<string, string>("F", "A");

            int[] expected4 = { 1, 1, 2 };
            int[] output4 = GraphAnalysis.AnalyzeEdges(vertices4, edges4, "A");
            PrintCase(vertices4, edges4, output4, expected4);
        }

        

        Thread tstCaseThr;
        bool caseTimedOut ;
        bool caseException;

        protected override void RunOnSpecificFile(string fileName, HardniessLevel level, int timeOutInMillisec)
        {
            int testCases;
            int[] actualResult = new int[3];
            int[] output = null;

            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(file);
            string line = sr.ReadLine();
            testCases = int.Parse(line);
   
            int totalCases = testCases;
            int correctCases = 0;
            int wrongCases = 0;
            int timeLimitCases = 0;
            bool readTimeFromFile = false;
            if (timeOutInMillisec == -1)
            {
                readTimeFromFile = true;
            }
            int i = 1;
            while (testCases-- > 0)
            {
                line = sr.ReadLine();
                int v = int.Parse(line);
                line = sr.ReadLine();
                int e = int.Parse(line);
                string startVertex = sr.ReadLine();
                
                line = sr.ReadLine();
                string[] vertices = line.Split(',');
                var edges = new KeyValuePair<string, string>[e];
                for (int j = 0; j < e; j++)
                {
                    line = sr.ReadLine();
                    string[] lineParts = line.Split(',');
                    edges[j] = new KeyValuePair<string, string>(lineParts[0], lineParts[1]);
                }
                line = sr.ReadLine();
                string[] results = line.Split(',');

                actualResult[0] = int.Parse(results[0]);
                actualResult[1] = int.Parse(results[1]);
                actualResult[2] = int.Parse(results[2]);
                caseTimedOut = true;
                caseException = false;
                {
                    tstCaseThr = new Thread(() =>
                    {
                        try
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            output = GraphAnalysis.AnalyzeEdges(vertices, edges, startVertex);
                            sw.Stop();
                            //PrintCase(vertices,edges, output, actualResult);
                            Console.WriteLine("|V| = {0}, |E| = {1}, time in ms = {2}", vertices.Length, edges.Length, sw.ElapsedMilliseconds);
                            Console.WriteLine("{0},{1},{2}", output[0], output[1], output[2]);

                        }
                        catch
                        {
                            caseException = true;
                            output = null;
                        }
                        caseTimedOut = false;
                    });

                    //StartTimer(timeOutInMillisec);
                    if (readTimeFromFile)
                    {
                        timeOutInMillisec = int.Parse(sr.ReadLine().Split(':')[1]);
                    }
                    tstCaseThr.Start();
                    tstCaseThr.Join(timeOutInMillisec);
                }

                if (caseTimedOut)       //Timedout
                {
                    Console.WriteLine("Time Limit Exceeded in Case {0}.", i);
					tstCaseThr.Abort();
                    timeLimitCases++;
                }
                else if (caseException) //Exception 
                {
                    Console.WriteLine("Exception in Case {0}.", i);
                    wrongCases++;
                }
                else if (output[0] == actualResult[0] && output[1] == actualResult[1] && output[2] == actualResult[2])    //Passed
                {
                    Console.WriteLine("Test Case {0} Passed!", i);
                    correctCases++;
                }
                else                    //WrongAnswer
                {
                    Console.WriteLine("Wrong Answer in Case {0}.", i);
                    Console.WriteLine(" your answer = {0}, {1}, {2}, correct answer = {3}, {4}, {5}", output[0], output[1], output[2], actualResult[0], actualResult[1], actualResult[2]);
                    wrongCases++;
                }

                i++;
            }
            file.Close();
            sr.Close();
            Console.WriteLine();
            Console.WriteLine("# correct = {0}", correctCases);
            Console.WriteLine("# time limit = {0}", timeLimitCases);
            Console.WriteLine("# wrong = {0}", wrongCases);
            Console.WriteLine("\nFINAL EVALUATION (%) = {0}", Math.Round((float)correctCases / totalCases * 100, 0)); 
        }

        protected override void OnTimeOut(DateTime signalTime)
        {
        }

        public override void GenerateTestCases(HardniessLevel level, int numOfCases, bool includeTimeInFile = false, float timeFactor = 1)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods
        private static void PrintCase(string[] vertices, KeyValuePair<string, string>[] edges, int[] output, int[] expected)
        {
            Console.Write("Vertices: ");
            for (int i = 0; i < vertices.Length; i++)
            {
                Console.Write("{0}  ", vertices[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Edges: ");
            for (int i = 0; i < edges.Length; i++)
            {
                Console.WriteLine("{0}, {1}", edges[i].Key, edges[i].Value);
            }
            Console.WriteLine("Outputs: # backward = {0}, # forward = {1}, # cross = {2}", output[0], output[1], output[2]);
            Console.WriteLine("Expected: # backward = {0}, # forward = {1}, # cross = {2}", expected[0], expected[1], expected[2]);
            if (output[0] == expected[0] && output[1] == expected[1] && output[2] == expected[2])    //Passed
            {
                Console.WriteLine("CORRECT");
            }
            else                    //WrongAnswer
            {
                Console.WriteLine("WRONG");
            }
            Console.WriteLine();
        }
        
        #endregion
   
    }
}
