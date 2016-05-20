/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) Under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You Under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed Under the License Is distributed on an "AS Is" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations Under the License.
==================================================================== */

namespace NPOI.HSSF.Model
{
    using System;
    using System.Text;
    using NPOI.HSSF.Record.Formula;

    /**
     * This class performs 'operand class' Transformation. Non-base tokens are classified into three 
     * operand classes:
     * <ul>
     * <li>reference</li> 
     * <li>value</li> 
     * <li>array</li> 
     * </ul>
     * <p/>
     * 
     * The operand class chosen for each token depends on the formula type and the token's place
     * in the formula. If POI Gets the operand class wrong, Excel <em>may</em> interpret the formula
     * incorrectly.  This condition Is typically manifested as a formula cell that Displays as '#VALUE!',
     * but resolves correctly when the user presses F2, enter.<p/>
     * 
     * The logic implemented here was partially inspired by the description in
     * "OpenOffice.org's Documentation of the Microsoft Excel File Format".  The model presented there
     * seems to be inconsistent with observed Excel behaviour (These differences have not been fully
     * investigated). The implementation in this class has been heavily modified in order to satisfy
     * concrete examples of how Excel performs the same logic (see TestRVA).<p/>
     * 
     * Hopefully, as Additional important test cases are identified and Added to the test suite, 
     * patterns might become more obvious in this code and allow for simplification.
     * 
     * @author Josh Micich
     */
    class OperandClassTransformer
    {

        private int _formulaType;

        public OperandClassTransformer(int formulaType)
        {
            _formulaType = formulaType;
        }

        /**
         * Traverses the supplied formula parse tree, calling <tt>Ptg.SetClass()</tt> for each non-base
         * token to Set its operand class.
         */
        public void TransformFormula(ParseNode rootNode)
        {
            byte rootNodeOperandClass;
            switch (_formulaType)
            {
                case FormulaParser.FORMULA_TYPE_CELL:
                    rootNodeOperandClass = Ptg.CLASS_VALUE;
                    break;
                default:
                    throw new Exception("Incomplete code - formula type ("
                            + _formulaType + ") not supported yet");

            }
            TransformNode(rootNode, rootNodeOperandClass, false);
        }

        private void TransformNode(ParseNode node, byte desiredOperandClass,
                bool callerForceArrayFlag)
        {
            Ptg token = node.GetToken();
            ParseNode[] children = node.Children;
            if (token is ValueOperatorPtg || token is ControlPtg)
            {
                // Value Operator Ptgs and Control are base tokens, so token will be UnChanged

                // but any child nodes are Processed according to desiredOperandClass and callerForceArrayFlag
                for (int i = 0; i < children.Length; i++)
                {
                    ParseNode child = children[i];
                    TransformNode(child, desiredOperandClass, callerForceArrayFlag);
                }
                return;
            }
            if (token is AbstractFunctionPtg)
            {
                TransformFunctionNode((AbstractFunctionPtg)token, children, desiredOperandClass,
                        callerForceArrayFlag);
                return;
            }
            if (children.Length > 0)
            {
                throw new Exception("Node should not have any children");
            }

            if (token.IsBaseToken)
            {
                // nothing to do
                return;
            }
            if (callerForceArrayFlag)
            {
                switch (desiredOperandClass)
                {
                    case Ptg.CLASS_VALUE:
                    case Ptg.CLASS_ARRAY:
                        token.PtgClass=(Ptg.CLASS_ARRAY);
                        break;
                    case Ptg.CLASS_REF:
                        token.PtgClass=(Ptg.CLASS_REF);
                        break;
                    default:
                        throw new Exception("Unexpected operand class ("
                                + desiredOperandClass + ")");
                }
            }
            else
            {
                token.PtgClass=(desiredOperandClass);
            }
        }

        private void TransformFunctionNode(AbstractFunctionPtg afp, ParseNode[] children,
                byte desiredOperandClass, bool callerForceArrayFlag)
        {

            bool localForceArrayFlag;
            byte defaultReturnOperandClass = afp.DefaultOperandClass;

            if (callerForceArrayFlag)
            {
                switch (defaultReturnOperandClass)
                {
                    case Ptg.CLASS_REF:
                        if (desiredOperandClass == Ptg.CLASS_REF)
                        {
                            afp.PtgClass=(Ptg.CLASS_REF);
                        }
                        else
                        {
                            afp.PtgClass=(Ptg.CLASS_ARRAY);
                        }
                        localForceArrayFlag = false;
                        break;
                    case Ptg.CLASS_ARRAY:
                        afp.PtgClass=(Ptg.CLASS_ARRAY);
                        localForceArrayFlag = false;
                        break;
                    case Ptg.CLASS_VALUE:
                        afp.PtgClass=(Ptg.CLASS_ARRAY);
                        localForceArrayFlag = true;
                        break;
                    default:
                        throw new Exception("Unexpected operand class ("
                                + defaultReturnOperandClass + ")");
                }
            }
            else
            {
                if (defaultReturnOperandClass == desiredOperandClass)
                {
                    localForceArrayFlag = false;
                    // an alternative would have been to for non-base Ptgs to Set their operand class 
                    // from their default, but this would require the call in many subclasses because
                    // the default OC Is not known Until the end of the constructor
                    afp.PtgClass=(defaultReturnOperandClass);
                }
                else
                {
                    switch (desiredOperandClass)
                    {
                        case Ptg.CLASS_VALUE:
                            // always OK to Set functions to return 'value'
                            afp.PtgClass=(Ptg.CLASS_VALUE);
                            localForceArrayFlag = false;
                            break;
                        case Ptg.CLASS_ARRAY:
                            switch (defaultReturnOperandClass)
                            {
                                case Ptg.CLASS_REF:
                                    afp.PtgClass=(Ptg.CLASS_REF);
                                    break;
                                case Ptg.CLASS_VALUE:
                                    afp.PtgClass=(Ptg.CLASS_ARRAY);
                                    break;
                                default:
                                    throw new Exception("Unexpected operand class ("
                                            + defaultReturnOperandClass + ")");
                            }
                            localForceArrayFlag = (defaultReturnOperandClass == Ptg.CLASS_VALUE);
                            break;
                        case Ptg.CLASS_REF:
                            switch (defaultReturnOperandClass)
                            {
                                case Ptg.CLASS_ARRAY:
                                    afp.PtgClass=(Ptg.CLASS_ARRAY);
                                    break;
                                case Ptg.CLASS_VALUE:
                                    afp.PtgClass=(Ptg.CLASS_VALUE);
                                    break;
                                default:
                                    throw new Exception("Unexpected operand class ("
                                            + defaultReturnOperandClass + ")");
                            }
                            localForceArrayFlag = false;
                            break;
                        default:
                            throw new Exception("Unexpected operand class ("
                                    + desiredOperandClass + ")");
                    }

                }
            }

            for (int i = 0; i < children.Length; i++)
            {
                ParseNode child = children[i];
                byte paramOperandClass = afp.GetParameterClass(i);
                TransformNode(child, paramOperandClass, localForceArrayFlag);
            }
        }
    }
}