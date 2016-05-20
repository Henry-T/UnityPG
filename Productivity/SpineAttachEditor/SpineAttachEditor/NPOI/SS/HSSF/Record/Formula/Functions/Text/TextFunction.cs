/*
* Licensed to the Apache Software Foundation (ASF) Under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for Additional information regarding copyright ownership.
* The ASF licenses this file to You Under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed Under the License is distributed on an "AS Is" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations Under the License.
*/
/*
 * Created on May 22, 2005
 *
 */
namespace NPOI.HSSF.Record.Formula.Functions
{
    using System;
    using NPOI.HSSF.Record.Formula.Eval;

    public abstract class SingleArgTextFunc : TextFunction
    {

        protected SingleArgTextFunc()
        {
            // no fields to initialise
        }
        public override ValueEval EvaluateFunc(ValueEval[] args, int srcCellRow, int srcCellCol)
        {
            if (args.Length != 1)
            {
                return ErrorEval.VALUE_INVALID;
            }
            String arg = EvaluateStringArg(args[0], srcCellRow, srcCellCol);
            return Evaluate(arg);
        }
        public abstract ValueEval Evaluate(String arg);
    }

    /**
     * @author Amol S. Deshmukh &lt; amolweb at ya hoo dot com &gt;
     *
     */
    public abstract class TextFunction : Function
    {

        protected static String EMPTY_STRING = "";

        public static String EvaluateStringArg(ValueEval eval, int srcRow, int srcCol)
        {
            ValueEval ve = OperandResolver.GetSingleValue(eval, srcRow, srcCol);
            return OperandResolver.CoerceValueToString(ve);
        }
        public static int EvaluateIntArg(ValueEval arg, int srcCellRow, int srcCellCol)
        {
            ValueEval ve = OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol);
            return OperandResolver.CoerceValueToInt(ve);
        }

        public ValueEval Evaluate(ValueEval[] args, int srcCellRow, int srcCellCol)
        {
            try
            {
                return EvaluateFunc(args, srcCellRow, srcCellCol);
            }
            catch (EvaluationException e)
            {
                return e.GetErrorEval();
            }
        }

        public abstract ValueEval EvaluateFunc(ValueEval[] args, int srcCellRow, int srcCellCol);

        /* ---------------------------------------------------------------------- */



        public static Function LEN = new Len();
        public static Function LOWER = new Lower();
        public static Function UPPER = new Upper();
        /**
         * An implementation of the TRIM function:
         * Removes leading and trailing spaces from value if evaluated operand
         *  value is string.
         * @author Manda Wilson &lt; wilson at c bio dot msk cc dot org &gt;
         */
        public static Function TRIM = new Trim();

        /**
         * An implementation of the MID function<br/>
         * MID returns a specific number of
         * characters from a text string, starting at the specified position.<p/>
         * 
         * <b>Syntax<b>:<br/> <b>MID</b>(<b>text</b>, <b>start_num</b>,
         * <b>num_chars</b>)<br/>
         * 
         * @author Manda Wilson &lt; wilson at c bio dot msk cc dot org &gt;
         */
        public static Function MID = new Mid();



        public static Function LEFT = new LeftRight(true);
        public static Function RIGHT = new LeftRight(false);

        public static Function CONCATENATE = new Concatenate();

        public static Function EXACT = new Exact();

        /**
         * Implementation of the FIND() function.<p/>
         *
         * <b>Syntax</b>:<br/>
         * <b>FIND</b>(<b>Find_text</b>, <b>within_text</b>, start_num)<p/>
         *
         * FIND returns the character position of the first (case sensitive) occurrence of
         * <tt>Find_text</tt> inside <tt>within_text</tt>.  The third parameter,
         * <tt>start_num</tt>, is optional (default=1) and specifies where to start searching
         * from.  Character positions are 1-based.<p/>
         *
         * @author Torstein Tauno Svendsen (torstei@officenet.no)
         */
        public static Function FIND = new SearchFind(true);
        /**
         * Implementation of the FIND() function.<p/>
         *
         * <b>Syntax</b>:<br/>
         * <b>SEARCH</b>(<b>Find_text</b>, <b>within_text</b>, start_num)<p/>
         *
         * SEARCH is a case-insensitive version of FIND()
         */
        public static Function SEARCH = new SearchFind(false);
    }
}