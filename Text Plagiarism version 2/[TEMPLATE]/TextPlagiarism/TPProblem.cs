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

namespace Problem
{

    public class Problem : ProblemBase, IProblem
    {
        #region ProblemBase Methods
        public override string ProblemName { get { return "TextPlagiarism"; } }

        public override void TryMyCode()
        {
            int M = 0;
            int N = 0;
            int outputVal, expectedVal;
            string[] subseq;

            {
                string text1 = "hello world how are you";
                string text2 = "hello are you world how";
                expectedVal = 3;
                outputVal = TextPlagiarism.SolveValue(text1, text2);
                subseq = TextPlagiarism.ConstructSolution(text1, text2);
                PrintCase(text1, text2, subseq, outputVal, expectedVal);
            }
            {
                string text1 = "hello world";
                string text2 = "how are you";
                expectedVal = 0;
                outputVal = TextPlagiarism.SolveValue(text1, text2);
                subseq = TextPlagiarism.ConstructSolution(text1, text2);
                PrintCase(text1, text2, subseq, outputVal, expectedVal);
            }
            {
                string text1 = "Algorithms Analysis and Design";
                string text2 = "ANALYSIS & DESIGN of ALGORITHMS \n System analysis and design \n Numerical Analysis \n Data Structures \n S/W DESIGN";
                expectedVal = 3;
                outputVal = TextPlagiarism.SolveValue(text1, text2);
                subseq = TextPlagiarism.ConstructSolution(text1, text2);
                PrintCase(text1, text2, subseq, outputVal, expectedVal);
            }
            {
                string text1 = "DP is a careful brute force and a complete D&C with overlapped sub-problems";
                string text2 = "Greedy has two conditions: optimal substructure and safe greedy choice\n\r DP has two conditions: optimal substructure (i.e. D&C) and overlapped subproblems";
                expectedVal = 3;
                outputVal = TextPlagiarism.SolveValue(text1, text2);
                subseq = TextPlagiarism.ConstructSolution(text1, text2);
                PrintCase(text1, text2, subseq, outputVal, expectedVal);
            }
        }




        Thread tstCaseThr;
        bool caseTimedOut;
        bool caseException;

        protected override void RunOnSpecificFile(string fileName, HardniessLevel level, int timeOutInMillisec)
        {
            int testCases;
            int M = 0, N = 0;
            string text1, text2 = null;
            string path1, path2 = null;
            int output = -1;
            int actualResult = -1;
            int j = 0;

            Stream s = new FileStream(fileName, FileMode.Open);
            StreamReader br = new StreamReader(s);

            testCases = int.Parse(br.ReadLine());

            int totalCases = testCases;
            int[] correctCases = new int[2];
            int[] wrongCases = new int[2];
            int[] timeLimitCases = new int[2];
            for (int i = 0; i < 2; i++)
            {
                correctCases[i] = 0;
                wrongCases[i] = 0;
                timeLimitCases[i] = 0;
            }
            bool readTimeFromFile = false;
            if (timeOutInMillisec == -1)
            {
                readTimeFromFile = true;
            }
            float maxTime = float.MinValue;
            float avgTime = 0;
            for (int i = 1; i <= testCases; i++)
            {
                path1 = br.ReadLine();
                path2 = br.ReadLine();
                actualResult = int.Parse(br.ReadLine());
                if (readTimeFromFile)
                {
                    timeOutInMillisec = int.Parse(br.ReadLine().Split(':')[1]);
                }

                //if (i < 4)
                //    continue;

                StreamReader sr = new StreamReader(path1);
                text1 = sr.ReadToEnd();
                sr.Close();
                sr = new StreamReader(path2);
                text2 = sr.ReadToEnd();
                sr.Close();

                Console.WriteLine("\n===========================");
                Console.WriteLine("CASE#{0}:", i);
                Console.WriteLine("===========================");

                for (int c = 0; c < 2; c++)
                {
                    caseTimedOut = true;
                    Stopwatch sw = null;
                    caseException = false;
                    string[] outputVals = null;
                    {
                        tstCaseThr = new Thread(() =>
                        {
                            try
                            {
                                sw = Stopwatch.StartNew();
                                if (c == 0)
                                {
                                    output = TextPlagiarism.SolveValue(text1, text2);
                                }
                                else
                                {
                                    outputVals = TextPlagiarism.ConstructSolution(text1, text2);
                                    if (outputVals == null)
                                        output = 0;
                                }
                                sw.Stop();
                                Console.WriteLine("time = {0} ms", sw.ElapsedMilliseconds);
                                Console.WriteLine("output = {0}", output);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                caseException = true;
                                output = -1;
                                outputVals = null;
                            }
                            caseTimedOut = false;
                        });

                        //StartTimer(timeOutInMillisec);
                        tstCaseThr.Start();
                        tstCaseThr.Join(timeOutInMillisec);
                    }

                    if (caseTimedOut)       //Timedout
                    {
                        tstCaseThr.Abort();
                        Console.WriteLine("Time Limit Exceeded in Case {0} [FUNCTION#{1}].", i, c + 1);
                        timeLimitCases[c]++;
                    }
                    else if (caseException) //Exception 
                    {
                        Console.WriteLine("Exception in Case {0} [FUNCTION#{1}].", i, c + 1);
                        wrongCases[c]++;
                    }
                    else if (output == actualResult)    //Passed
                    {
                        if (c == 0)
                        {
                            Console.WriteLine("Test Case {0} [FUNCTION#{1}] Passed!", i, c + 1);
                            correctCases[c]++;
                        }
                        else if (CheckOutput(text1, text2, outputVals, output, actualResult))
                        {
                            Console.WriteLine("Test Case {0} [FUNCTION#{1}] Passed!", i, c + 1);
                            correctCases[c]++;
                        }
                        else
                        {
                            Console.WriteLine("Wrong Answer in Case {0} [FUNCTION#{1}].", i, c + 1);
                            wrongCases[c]++;
                        }
                        //maxTime = Math.Max(maxTime, sw.ElapsedMilliseconds);
                        //avgTime += sw.ElapsedMilliseconds;
                    }
                    else                    //WrongAnswer
                    {
                        Console.WriteLine("Wrong Answer in Case {0} [FUNCTION#{1}].", i, c + 1);
                        if (level == HardniessLevel.Easy)
                        {
                            PrintCase(text1, text2, outputVals, output, actualResult, false);
                        }
                        wrongCases[c]++;
                    }
                }
            }
            s.Close();
            br.Close();
            for (int c = 0; c < 2; c++)
            {
                Console.WriteLine("EVALUATION OF FUNCTION#{0}:", c + 1);
                Console.WriteLine("# correct = {0}", correctCases[c]);
                Console.WriteLine("# time limit = {0}", timeLimitCases[c]);
                Console.WriteLine("# wrong = {0}", wrongCases[c]);
                //Console.WriteLine("\nFINAL EVALUATION (%) = {0}", Math.Round((float)correctCases / totalCases * 100, 0));
                //Console.WriteLine("AVERAGE EXECUTION TIME (ms) = {0}", Math.Round(avgTime / (float)correctCases, 2));
                //Console.WriteLine("MAX EXECUTION TIME (ms) = {0}", maxTime); 
            }
            Console.WriteLine("\nFINAL EVALUATION: FUNCTION#1 (%), FUNCTION#2 (%) = {0} {1}", Math.Round((float)correctCases[0] / totalCases * 100, 0), Math.Round((float)correctCases[1] / totalCases * 100, 0));

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
        private static void PrintVals(string[] X)
        {
            int N = X.Length;
            for (int i = 0; i < N; i++)
            {
                Console.Write(X[i] + "  ");
            }
            Console.WriteLine();
        }

        private void PrintCase(string text1, string text2, string[] subseq, int outputVal, int expectedVal, bool check = true)
        {
            Console.WriteLine("text1 = {0}", text1);
            Console.WriteLine("text2 = {0}", text2);
            Console.WriteLine("expected value = {0}", expectedVal);
            Console.WriteLine("output value = {0}", outputVal);
            if (subseq != null)
            {
                Console.Write("output subseq = "); PrintVals(subseq);
            }

            if (check)
            {
                if (outputVal != expectedVal)
                {
                    Console.WriteLine("WRONG");
                }
                else
                {
                    if (CheckOutput(text1, text2, subseq, outputVal, expectedVal)) Console.WriteLine("CORRECT");
                    else Console.WriteLine("WRONG");
                }
            }
            Console.WriteLine();
        }

        private bool CheckOutput(string paragraph, string text, string[] subseq, int output, int expectedResult)
        {
            if (subseq == null)
            {
                if (expectedResult > 0)
                    return false;
                else
                    return true;
            }

            if (output != expectedResult)
            {
                Console.WriteLine("WRONG");
                return false;
            }

            if (subseq.Length != expectedResult)
            {
                Console.WriteLine("WRONG");
                return false;
            }
            paragraph = paragraph.ToLower();
            text = text.ToLower();

            char[] paragraphSeperators = { '\n', '\r' };
            string[] paragraphs = text.Split(paragraphSeperators, StringSplitOptions.RemoveEmptyEntries);

            char[] wordSeperators = { ' ', '\t' };
            string[] w1 = paragraph.Split(wordSeperators, StringSplitOptions.RemoveEmptyEntries);

            bool found = false;
            for (int p = 0; p < paragraphs.Length; p++)
            {
                string[] w2 = paragraphs[p].Split(wordSeperators, StringSplitOptions.RemoveEmptyEntries);
                found = CheckWordsOccurance(subseq, w1, w2);
                if (found) break;
            }
            if (!found)
            {
                Console.WriteLine("WRONG: Invalid subsequence in paragraph or text! one or more word is not found or not in a correct subseq order");
                return false;
            }

            return true;
        }

        private bool CheckWordsOccurance(string[] subseq, string[] w1, string[] w2)
        {
            int j1 = 0, j2 = 0;
            for (int i = 0; i < subseq.Length; i++)
            {
                bool found = false;
                for (; j1 < w1.Length; j1++)
                {
                    if (w1[j1] == subseq[i].ToLower())
                    {
                        found = true;
                        j1++;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
                found = false;
                for (; j2 < w2.Length; j2++)
                {
                    if (w2[j2] == subseq[i].ToLower())
                    {
                        found = true;
                        j2++;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}