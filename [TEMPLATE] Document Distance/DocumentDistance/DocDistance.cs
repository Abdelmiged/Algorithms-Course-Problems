using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDistance
{
    class DocDistance
    {
        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO
        // *****************************************
        /// <summary>
        /// Write an efficient algorithm to calculate the distance between two documents
        /// </summary>
        /// <param name="doc1FilePath">File path of 1st document</param>
        /// <param name="doc2FilePath">File path of 2nd document</param>
        /// <returns>The angle (in degree) between the 2 documents</returns>
        public static double CalculateDistance(string doc1FilePath, string doc2FilePath)
        {
            // TODO comment the following line THEN fill your code here
            //throw new NotImplementedException();
            Dictionary<string, long> doc1Words = new Dictionary<string, long>();
            Dictionary<string, long> doc2Words = new Dictionary<string, long>();

            double magnitude1 = 0, magnitude2 = 0;

            Parallel.Invoke(
                () =>
                {
                    Splitter(doc1FilePath, doc1Words);
                    magnitude1 = Magnitude(doc1Words);
                },
                () =>
                {
                    Splitter(doc2FilePath, doc2Words);
                    magnitude2 = Magnitude(doc2Words);
                }
            );

            return Angle(doc1Words, doc2Words, magnitude1, magnitude2);
        }

        public static void Splitter(string docPath, Dictionary<string, long> docWords)
        {
            string docText = File.ReadAllText(docPath);

            docText = docText.ToLower();

            string word = "";
            for(int i = 0; i < docText.Length; i++)
            {
                if((docText[i] >= 'a' && docText[i] <= 'z') || (docText[i] >= '0' && docText[i] <= '9'))
                {
                    word += docText[i];
                }
                else
                {
                    if(word != "")
                    {
                        if (docWords.ContainsKey(word))
                        {
                            docWords[word]++;
                        }
                        else
                        {
                            docWords.Add(word, 1);
                        }
                        word = "";
                    }
                }
            }
            if (word != "")
            {
                if (docWords.ContainsKey(word))
                {
                    docWords[word]++;
                }
                else
                {
                    docWords.Add(word, 1);
                }
            }

        }

        public static double DotProduct(Dictionary<string, long> dict1, Dictionary<string, long> dict2)
        {
            double dotProductSum = 0;
            if (dict1.Count() <= dict2.Count())
            {
                foreach (KeyValuePair<string, long> pair in dict1)
                {
                    if (!dict2.ContainsKey(pair.Key))
                    {
                        continue;
                    }
                    dotProductSum += (pair.Value * dict2[pair.Key]);
                }
            }
            else
            {
                foreach (KeyValuePair<string, long> pair in dict2)
                {
                    if (!dict1.ContainsKey(pair.Key))
                    {
                        continue;
                    }
                    dotProductSum += (pair.Value * dict1[pair.Key]);
                }
            }
            return dotProductSum;
        }

        public static double Magnitude(Dictionary<string, long> dict)
        {
            double magnitude = 0;
            foreach(KeyValuePair<string, long> pair in dict)
            {
                magnitude += Math.Pow(pair.Value, 2);
            }
            return magnitude;
        }

        public static double Angle(Dictionary<string, long> dict1, Dictionary<string, long> dict2, double magnitude1, double magnitude2)
        {
            return (180/Math.PI) * Math.Acos(DotProduct(dict1, dict2) / (Math.Sqrt(magnitude1 * magnitude2)));
        }
    }
}
