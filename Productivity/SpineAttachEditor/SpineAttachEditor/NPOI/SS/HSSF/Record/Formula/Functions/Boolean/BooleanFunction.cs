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
 * Created on May 15, 2005
 *
 */
namespace NPOI.HSSF.Record.Formula.Functions
{
    using System;
    using NPOI.HSSF.Record.Formula.Eval;

    /**
     * @author Amol S. Deshmukh &lt; amolweb at ya hoo dot com &gt;
     * Here are the general rules concerning bool functions:
     * <ol>
     * <li> Blanks are not either true or false
     * <li> Strings are not either true or false (even strings "true" 
     * or "TRUE" or "0" etc.)
     * <li> Numbers: 0 Is false. Any other number Is TRUE.
     * <li> References are Evaluated and above rules apply.
     * <li> Areas: Individual cells in area are Evaluated and Checked to 
     * see if they are blanks, strings etc.
     * </ol>
     */
    public abstract class BooleanFunction : Function
    {
        protected abstract bool InitialResultValue { get; }
        protected abstract bool PartialEvaluate(bool cumulativeResult, bool currentValue);


        private bool Calculate(ValueEval[] args)
        {

            bool result = InitialResultValue;
            bool atleastOneNonBlank = false;
            bool? tempVe;
            /*
             * Note: no short-circuit bool loop exit because any ErrorEvals will override the result
             */
            for (int i = 0, iSize = args.Length; i < iSize; i++)
            {
                ValueEval arg = args[i];
                if (arg is AreaEval)
                {
                    AreaEval ae = (AreaEval)arg;
                    int height = ae.Height;
                    int width = ae.Width;
                    for (int rrIx = 0; rrIx < height; rrIx++)
                    {
                        for (int rcIx = 0; rcIx < width; rcIx++)
                        {
                            ValueEval ve = ae.GetRelativeValue(rrIx, rcIx);
                            tempVe = OperandResolver.CoerceValueToBoolean(ve, true);
                            if (tempVe != null)
                            {
                                result = PartialEvaluate(result, Convert.ToBoolean(tempVe));
                                atleastOneNonBlank = true;
                            }
                        }
                    }
                    continue;
                }

                if (arg is RefEval)
                {
                    ValueEval ve = ((RefEval)arg).InnerValueEval;
                    tempVe = OperandResolver.CoerceValueToBoolean(ve, true);
                }
                else if (arg is ValueEval)
                {
                    ValueEval ve = (ValueEval)arg;
                    tempVe = OperandResolver.CoerceValueToBoolean(ve, false);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected eval (" + arg.GetType().Name + ")");
                }


                if (tempVe != null)
                {
                    result = PartialEvaluate(result, Convert.ToBoolean(tempVe));
                    atleastOneNonBlank = true;
                }
            }

            if (!atleastOneNonBlank)
            {
                throw new EvaluationException(ErrorEval.VALUE_INVALID);
            }
            return result;
        }

        public ValueEval Evaluate(ValueEval[] args, int srcRow, int srcCol)
        {
            if (args.Length < 1)
            {
                return ErrorEval.VALUE_INVALID;
            }
            bool boolResult;
            try
            {
                boolResult = Calculate(args);
            }
            catch (EvaluationException e)
            {
                return e.GetErrorEval();
            }
            return BoolEval.ValueOf(boolResult);
        }
    }
}