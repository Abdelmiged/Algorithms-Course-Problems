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
    public static class OfficeOrganization
    {
        #region YOUR CODE IS HERE
        /// <summary>
        /// find the minimum costs in MOST EFFICIENT WAY to organize your office to meet your father needs.
        /// </summary>
        /// <param name="N">initial load</param>
        /// <param name="M">target load required by your father</param>
        /// <param name="A">cost of reducing the load by half</param>
        /// <param name="B">cost of reducing the load by 1</param>
        /// <returns>Min total cost to reduce the load from N to M</returns>
        public static int OrganizeTheOffice(int N, int M, int A, int B)
        {
            return getMinCost(N, M, A, B);
        }

        public static int getMinCost(int N, int M, int A, int B)
        {
            int current_AB_min_cost = 0;
            int final_AB_min_cost = int.MaxValue;
            int division_times = 0;
            while (true)
            {
                current_AB_min_cost = (division_times * A) + ((N - M) * B);
                final_AB_min_cost = Math.Min(current_AB_min_cost, final_AB_min_cost);
                N /= 2;
                if (N < M || current_AB_min_cost > final_AB_min_cost)
                {
                    break;
                }
                division_times++;
            }
            return final_AB_min_cost;
        }
        #endregion
    }
}