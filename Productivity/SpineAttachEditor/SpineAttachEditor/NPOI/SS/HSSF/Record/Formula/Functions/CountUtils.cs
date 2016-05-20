/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.HSSF.Record.Formula.Functions
{
    using System;
    using NPOI.HSSF.Record.Formula.Eval;
    /**
 * Common interface for the matching criteria.
 */
    public interface I_MatchPredicate
    {
        bool Matches(ValueEval x);
    }
    /**
     * Common logic for COUNT, COUNTA and COUNTIF
     *
     * @author Josh Micich 
     */
    class CountUtils
    {

        private CountUtils()
        {
            // no instances of this class
        }


        /**
         * @return the number of evaluated cells in the range that match the specified criteria
         */
        public static int CountMatchingCellsInArea(AreaEval areaEval, I_MatchPredicate criteriaPredicate)
        {
            int result = 0;

            int height = areaEval.Height;
            int width = areaEval.Width;
            for (int rrIx = 0; rrIx < height; rrIx++)
            {
                for (int rcIx = 0; rcIx < width; rcIx++)
                {
                    ValueEval ve = areaEval.GetRelativeValue(rrIx, rcIx);
                    if (criteriaPredicate.Matches(ve))
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        /**
         * @return 1 if the evaluated cell matches the specified criteria
         */
        public static int CountMatchingCell(RefEval refEval, I_MatchPredicate criteriaPredicate)
        {
            if (criteriaPredicate.Matches(refEval.InnerValueEval))
            {
                return 1;
            }
            return 0;
        }
        public static int CountArg(ValueEval eval, I_MatchPredicate criteriaPredicate)
        {
            if (eval == null)
            {
                throw new ArgumentException("eval must not be null");
            }
            if (eval is AreaEval)
            {
                return CountUtils.CountMatchingCellsInArea((AreaEval)eval, criteriaPredicate);
            }
            if (eval is RefEval)
            {
                return CountUtils.CountMatchingCell((RefEval)eval, criteriaPredicate);
            }
            return criteriaPredicate.Matches(eval) ? 1 : 0;
        }
    }
}