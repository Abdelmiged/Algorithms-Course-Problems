using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public static class TextPlagiarism
    {
        #region YOUR CODE IS HERE
        static int[,] maxTableData = new int[0, 0];
        static string[] words;
        static int maxPlagiarism = 0;
        #region FUNCTION#1: Calculate the Value
        //Your Code is Here:
        //==================
        /// <summary>
        /// Given a paragraph and a complete text, find the plagiarism similarity of the give paragraph vs the given text.
        /// Plagiarism similarity = max common subsequence of words between the given paragraph and EACH paragraph in the given text
        /// Comparison is case IN-SENSITIVE (i.e. Cat = CAT = cat = CaT)
        /// Definitions:
        ///     Word: a set of continuous characters seperated by space or tab (Words seperator: ' ' '\t')
        ///     Paragraph in Text: any continuous set of words/chars ended by new line(s) (Paragraphs seperator: '\n' '\r')
        /// </summary>
        /// <param name="paragraph">query paragraph</param>
        /// <param name="text">complete text (consists of 1 or more paragraph(s)</param>
        /// <returns>Plagiarism similarity between the query paragraph and the complete text</returns>
        static public int SolveValue(string paragraph, string text)
        {
            string[] singleParagraph = new string[0];
            string[][] multipleParagraphs = new string[0][];

            maxPlagiarism = 0;
            maxTableData = new int[0, 0];

            Parallel.Invoke(
                () =>
                {
                    paragraph = paragraph.ToLower();
                    singleParagraph = SingleParagraphWordsExtractor(paragraph);
                    words = singleParagraph;
                },
                () =>
                {
                    text = text.ToLower();
                    multipleParagraphs = MultipleParagraphsWordsExtractor(text);
                }
            );

            foreach(var item in multipleParagraphs)
            {
                int[,] tableData = new int[singleParagraph.Length, item.Length];

                for(int i = 0; i < singleParagraph.Length; i++)
                {
                    for(int j = 0; j < item.Length; j++)
                    {
                        if (i == 0 || j == 0)
                        {
                            tableData[i, j] = 0;
                        }
                        else if (singleParagraph[i] == item[j])
                        {
                            tableData[i, j] = 1 + tableData[i - 1, j - 1];
                        }
                        else
                        {
                            if (tableData[i - 1, j] >= tableData[i, j - 1])
                            {
                                tableData[i, j] = tableData[i - 1, j];
                            }
                            else
                            {
                                tableData[i, j] = tableData[i, j - 1];
                            }
                        }
                    }
                }

                int lastCell = tableData[tableData.GetLength(0) - 1, tableData.GetLength(1) - 1];

                if (lastCell > maxPlagiarism)
                {
                    maxPlagiarism = lastCell;
                    maxTableData = tableData;
                }
            }

            return maxPlagiarism;
        }
        #endregion

        #region FUNCTION#2: Construct the Solution

        //Your Code is Here:
        //==================
        /// <returns>the common subsequence words themselves (if any) or null if no common words </returns>
        static public string[] ConstructSolution(string paragraph, string text)
        {
            if(maxTableData.Length == 0)
            {
                return null;
            }

            int i = maxTableData.GetLength(0) - 1;
            int j = maxTableData.GetLength(1) - 1;
            int k = maxPlagiarism - 1;
            string[] output = new string[maxPlagiarism];

            while (true)
            {
                if(i == 0 || j == 0)
                {
                    break;
                }
                else
                {
                    int currentCell = maxTableData[i, j];
                    int upCell = maxTableData[i - 1, j];
                    int leftCell = maxTableData[i, j - 1];

                    if(upCell == leftCell && currentCell == upCell)
                    {
                        i--;
                    }
                    else if(upCell == leftCell && currentCell != upCell)
                    {
                        output[k] = words[i];
                        k--;
                        i--;
                        j--;
                    }
                    else
                    {
                        if(upCell > leftCell)
                        {
                            i--;
                        }
                        else if(leftCell > upCell)
                        {
                            j--;
                        }
                    }
                }
            }

            maxTableData = new int[0, 0];
            return output;
        }
        #endregion

        public static string[] SingleParagraphWordsExtractor(string text)
        {
            char[] splitters = new char[] { ' ', '\t' };
            string[] extractedWords = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            string[] output = new string[extractedWords.Length + 1];
            output[0] = null;
            extractedWords.CopyTo(output, 1);
            return output;
        }

        public static string[][] MultipleParagraphsWordsExtractor(string text)
        {

            char[] splitters = new char[] { '\n', '\r' };
            int indx = 0;

            string[] paragraphs = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            string[][] output = new string[paragraphs.Length][];

            foreach (var item in paragraphs)
            {
                string[] words = SingleParagraphWordsExtractor(item);
                output[indx] = words;
                indx++;
            }
            return output;
        }

        #endregion
    }
}