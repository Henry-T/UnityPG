/* ====================================================================
   Licensed To the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file To You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed To in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.SS.Formula
{
    using System;
    using System.Collections;
    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.Record.Formula.Eval;
    using NPOI.SS.Util;
    using NPOI.HSSF.Record.Formula.Functions;
    using NPOI.HSSF.Record.Formula.Udf;
    using System.Collections.Generic;
    using NPOI.SS.UserModel;

    /**
     * Evaluates formula cells.<p/>
     *
     * For performance reasons, this class keeps a cache of all previously calculated intermediate
     * cell values.  Be sure To call {@link #ClearCache()} if any workbook cells are Changed between
     * calls To evaluate~ methods on this class.<br/>
     *
     * For POI internal use only
     *
     * @author Josh Micich
     */
    public class WorkbookEvaluator
    {

        private EvaluationWorkbook _workbook;
        private EvaluationCache _cache;
        private int _workbookIx;

        private IEvaluationListener _evaluationListener;
        private Hashtable _sheetIndexesBySheet;
        private Dictionary<String, int> _sheetIndexesByName;
        private CollaboratingWorkbooksEnvironment _collaboratingWorkbookEnvironment;
        private IStabilityClassifier _stabilityClassifier;
	    private UDFFinder _udfFinder;

        public WorkbookEvaluator(EvaluationWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
            : this (workbook, null, stabilityClassifier, udfFinder)
        {

        }

        public WorkbookEvaluator(EvaluationWorkbook workbook, IEvaluationListener evaluationListener, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
        {
            _workbook = workbook;
            _evaluationListener = evaluationListener;
            _cache = new EvaluationCache(evaluationListener);
            _sheetIndexesBySheet = new Hashtable();
            _sheetIndexesByName = new Dictionary<string,int>();
            _collaboratingWorkbookEnvironment = CollaboratingWorkbooksEnvironment.EMPTY;
            _workbookIx = 0;
            _stabilityClassifier = stabilityClassifier;
            _udfFinder = udfFinder == null ? UDFFinder.DEFAULT : udfFinder;
        }

        /**
         * also for debug use. Used in ToString methods
         */
        /* package */
        public String GetSheetName(int sheetIndex)
        {
            return _workbook.GetSheetName(sheetIndex);
        }

        public WorkbookEvaluator GetOtherWorkbookEvaluator(String workbookName)
        {
            return _collaboratingWorkbookEnvironment.GetWorkbookEvaluator(workbookName);
        }


        internal EvaluationSheet GetSheet(int sheetIndex)
        {
            return _workbook.GetSheet(sheetIndex);
        }

        private static bool IsDebugLogEnabled()
        {
            return false;
        }
        private static void LogDebug(String s)
        {
            if (IsDebugLogEnabled())
            {
                Console.WriteLine(s);
            }
        }
        /* package */
        public void AttachToEnvironment(CollaboratingWorkbooksEnvironment collaboratingWorkbooksEnvironment, EvaluationCache cache, int workbookIx)
        {
            _collaboratingWorkbookEnvironment = collaboratingWorkbooksEnvironment;
            _cache = cache;
            _workbookIx = workbookIx;
        }
        /* package */
        public CollaboratingWorkbooksEnvironment GetEnvironment()
        {
            return _collaboratingWorkbookEnvironment;
        }

        /* package */
        public void DetachFromEnvironment()
        {
            _collaboratingWorkbookEnvironment = CollaboratingWorkbooksEnvironment.EMPTY;
            _cache = new EvaluationCache(_evaluationListener);
            _workbookIx = 0;
        }
        /* package */
        public IEvaluationListener GetEvaluationListener()
        {
            return _evaluationListener;
        }

        /**
         * Should be called whenever there are Changes To input cells in the evaluated workbook.
         * Failure To call this method after changing cell values will cause incorrect behaviour
         * of the evaluate~ methods of this class
         */
        public void ClearAllCachedResultValues()
        {
            _cache.Clear();
            _sheetIndexesBySheet.Clear();
        }

        /**
         * Should be called To tell the cell value cache that the specified (value or formula) cell 
         * Has Changed.
         */
        public void NotifyUpdateCell(EvaluationCell cell)
        {
            int sheetIndex = GetSheetIndex(cell.Sheet);
            _cache.NotifyUpdateCell(_workbookIx, sheetIndex, cell);
        }
        /**
         * Should be called To tell the cell value cache that the specified cell Has just been
         * deleted. 
         */
        public void NotifyDeleteCell(EvaluationCell cell)
        {
            int sheetIndex = GetSheetIndex(cell.Sheet);
            _cache.NotifyDeleteCell(_workbookIx, sheetIndex, cell);
        }

        public int GetSheetIndex(EvaluationSheet sheet)
        {
            object result = _sheetIndexesBySheet[sheet];
            if (result == null)
            {
                int sheetIndex = _workbook.GetSheetIndex(sheet);
                if (sheetIndex < 0)
                {
                    throw new Exception("Specified sheet from a different book");
                }
                result = sheetIndex;
                _sheetIndexesBySheet[sheet]= result;
            }
            return (int)result;
        }

        /**
 * Case-insensitive.
 * @return -1 if sheet with specified name does not exist
 */
        /* package */
        public int GetSheetIndex(String sheetName)
        {
            int result;
            if (_sheetIndexesByName.ContainsKey(sheetName))
            {
                result = _sheetIndexesByName[sheetName];
            }
            else
            {
                int sheetIndex = _workbook.GetSheetIndex(sheetName);
                if (sheetIndex < 0)
                {
                    return -1;
                }
                result = sheetIndex;
                _sheetIndexesByName[sheetName] = result;
            }
            return result;
        }

        public ValueEval Evaluate(EvaluationCell srcCell)
        {
            int sheetIndex = GetSheetIndex(srcCell.Sheet);
            return EvaluateAny(srcCell, sheetIndex, srcCell.RowIndex, srcCell.ColumnIndex, new EvaluationTracker(_cache));
        }


        /**
         * @return never <c>null</c>, never {@link BlankEval}
         */
        private ValueEval EvaluateAny(EvaluationCell srcCell, int sheetIndex,
                    int rowIndex, int columnIndex, EvaluationTracker tracker)
        {

            if (srcCell == null || srcCell.CellType != CellType.FORMULA)
            {
                ValueEval result = GetValueFromNonFormulaCell(srcCell);
                tracker.AcceptPlainValueDependency(_workbookIx, sheetIndex, rowIndex, columnIndex, result);
                return result;
            }

            FormulaCellCacheEntry cce = _cache.GetOrCreateFormulaCellEntry(srcCell);
            tracker.AcceptFormulaDependency(cce);
            IEvaluationListener evalListener = _evaluationListener;
            if (cce.GetValue() == null)
            {
                if (!tracker.StartEvaluate(cce))
                {
                    return ErrorEval.CIRCULAR_REF_ERROR;
                }
                OperationEvaluationContext ec = new OperationEvaluationContext(this, _workbook, sheetIndex, rowIndex, columnIndex, tracker);

                try
                {
                    ValueEval result;

                    Ptg[] ptgs = _workbook.GetFormulaTokens(srcCell);
                    if (evalListener == null)
                    {
                        result = EvaluateFormula(ec, ptgs);
                    }
                    else
                    {
                        evalListener.OnStartEvaluate(srcCell, cce);
                        result = EvaluateFormula(ec, ptgs);
                        evalListener.OnEndEvaluate(cce, result);
                    }

                    tracker.UpdateCacheResult(result);
                }
                finally
                {
                    tracker.EndEvaluate(cce);
                }
            }
            else
            {
                if (evalListener != null)
                {
                    evalListener.OnCacheHit(sheetIndex, rowIndex, columnIndex, cce.GetValue());
                }
                return cce.GetValue();
            }
            if (IsDebugLogEnabled())
            {
                String sheetName = GetSheetName(sheetIndex);
                CellReference cr = new CellReference(rowIndex, columnIndex);
                LogDebug("Evaluated " + sheetName + "!" + cr.FormatAsString() + " To " + cce.GetValue().ToString());
            }
            return cce.GetValue();
        }
        /**
         * Gets the value from a non-formula cell.
         * @param cell may be <c>null</c>
         * @return {@link BlankEval} if cell is <c>null</c> or blank, never <c>null</c>
         */
        /* package */
        internal static ValueEval GetValueFromNonFormulaCell(EvaluationCell cell)
        {
            if (cell == null)
            {
                return BlankEval.instance;
            }
            CellType cellType = cell.CellType;
            switch (cellType)
            {
                case CellType.NUMERIC:
                    return new NumberEval(cell.NumericCellValue);
                case CellType.STRING:
                    return new StringEval(cell.StringCellValue);
                case CellType.BOOLEAN:
                    return BoolEval.ValueOf(cell.BooleanCellValue);
                case CellType.BLANK:
                    return BlankEval.instance;
                case CellType.ERROR:
                    return ErrorEval.ValueOf(cell.ErrorCellValue);
            }
            throw new Exception("Unexpected cell type (" + cellType + ")");
        }
        // visibility raised for testing
        /* package */
        public ValueEval EvaluateFormula(OperationEvaluationContext ec, Ptg[] ptgs)
        {

            Stack<ValueEval> stack = new Stack<ValueEval>();
            for (int i = 0, iSize = ptgs.Length; i < iSize; i++)
            {

                // since we don't know how To handle these yet :(
                Ptg ptg = ptgs[i];
                if (ptg is AttrPtg)
                {
                    AttrPtg attrPtg = (AttrPtg)ptg;
                    if (attrPtg.IsSum)
                    {
                        // Excel prefers To encode 'SUM()' as a tAttr Token, but this evaluator
                        // expects the equivalent function Token
                        byte nArgs = 1;  // tAttrSum always Has 1 parameter
                        ptg = new FuncVarPtg("SUM", nArgs);
                    }
                    				if (attrPtg.IsOptimizedChoose) {
					ValueEval arg0 = stack.Pop();
					int[] jumpTable = attrPtg.JumpTable;
					int dist;
					int nChoices = jumpTable.Length;
					try {
						int switchIndex = Choose.EvaluateFirstArg(arg0, ec.RowIndex, ec.ColumnIndex);
						if (switchIndex<1 || switchIndex > nChoices) {
							stack.Push(ErrorEval.VALUE_INVALID);
							dist = attrPtg.ChooseFuncOffset + 4; // +4 for tFuncFar(CHOOSE)
						} else {
							dist = jumpTable[switchIndex-1];
						}
					} catch (EvaluationException e) {
						stack.Push(e.GetErrorEval());
						dist = attrPtg.ChooseFuncOffset + 4; // +4 for tFuncFar(CHOOSE)
					}
					// Encoded dist for tAttrChoose includes size of jump table, but
					// countTokensToBeSkipped() does not (it counts whole tokens).
					dist -= nChoices*2+2; // subtract jump table size
					i+= CountTokensToBeSkipped(ptgs, i, dist);
					continue;
				}
				if (attrPtg.IsOptimizedIf) {
					ValueEval arg0 = stack.Pop();
					bool evaluatedPredicate;
					try {
						evaluatedPredicate = If.EvaluateFirstArg(arg0, ec.RowIndex, ec.ColumnIndex);
					} catch (EvaluationException e) {
						stack.Push(e.GetErrorEval());
						int dist = attrPtg.Data;
						i+= CountTokensToBeSkipped(ptgs, i, dist);
						attrPtg = (AttrPtg) ptgs[i];
						dist = attrPtg.Data+1;
						i+= CountTokensToBeSkipped(ptgs, i, dist);
						continue;
					}
					if (evaluatedPredicate) {
						// nothing to skip - true param folows
					} else {
						int dist = attrPtg.Data;
						i+= CountTokensToBeSkipped(ptgs, i, dist);
						Ptg nextPtg = ptgs[i+1];
						if (ptgs[i] is AttrPtg && nextPtg is FuncVarPtg) {
							// this is an if statement without a false param (as opposed to MissingArgPtg as the false param)
							i++;
							stack.Push(BoolEval.FALSE);
						}
					}
					continue;
				}
				if (attrPtg.IsSkip) {
					int dist = attrPtg.Data+1;
					i+= CountTokensToBeSkipped(ptgs, i, dist);
					if (stack.Peek() == MissingArgEval.instance) {
						stack.Pop();
						stack.Push(BlankEval.instance);
					}
					continue;
				}
                }
                if (ptg is ControlPtg)
                {
                    // skip Parentheses, Attr, etc
                    continue;
                }
                if (ptg is MemFuncPtg)
                {
                    // can ignore, rest of Tokens for this expression are in OK RPN order
                    continue;
                }
                if (ptg is MemErrPtg) { continue; }

                ValueEval opResult;
                if (ptg is OperationPtg)
                {
                    OperationPtg optg = (OperationPtg)ptg;

                    if (optg is UnionPtg) { continue; }

                    int numops = optg.NumberOfOperands;
                    ValueEval[] ops = new ValueEval[numops];

                    // storing the ops in reverse order since they are popping
                    for (int j = numops - 1; j >= 0; j--)
                    {
                        ValueEval p = (ValueEval)stack.Pop();
                        ops[j] = p;
                    }
                    //				logDebug("Invoke " + operation + " (nAgs=" + numops + ")");
                    opResult = OperationEvaluatorFactory.Evaluate(optg, ops, ec);
                }
                else
                {
                    opResult = GetEvalForPtg(ptg, ec);
                }
                if (opResult == null)
                {
                    throw new Exception("Evaluation result must not be null");
                }
                //			logDebug("push " + opResult);
                stack.Push(opResult);
            }

            ValueEval value = ((ValueEval)stack.Pop());
            if (stack.Count != 0)
            {
                throw new InvalidOperationException("evaluation stack not empty");
            }
            value = DereferenceValue(value, ec.RowIndex, ec.ColumnIndex);
            if (value == BlankEval.instance)
            {
                // Note Excel behaviour here. A blank value is converted To zero.
                return NumberEval.ZERO;
                // Formulas _never_ evaluate To blank.  If a formula appears To have evaluated To
                // blank, the actual value is empty string. This can be verified with ISBLANK().
            }
            return value;
        }
        /**
 * Calculates the number of tokens that the evaluator should skip upon reaching a tAttrSkip.
 *
 * @return the number of tokens (starting from <tt>startIndex+1</tt>) that need to be skipped
 * to achieve the specified <tt>distInBytes</tt> skip distance.
 */
        private static int CountTokensToBeSkipped(Ptg[] ptgs, int startIndex, int distInBytes)
        {
            int remBytes = distInBytes;
            int index = startIndex;
            while (remBytes != 0)
            {
                index++;
                remBytes -= ptgs[index].Size;
                if (remBytes < 0)
                {
                    throw new Exception("Bad skip distance (wrong token size calculation).");
                }
                if (index >= ptgs.Length)
                {
                    throw new Exception("Skip distance too far (ran out of formula tokens).");
                }
            }
            return index - startIndex;
        }
        /**
         * Dereferences a single value from any AreaEval or RefEval evaluation result.
         * If the supplied evaluationResult is just a plain value, it is returned as-is.
         * @return a <tt>NumberEval</tt>, <tt>StringEval</tt>, <tt>BoolEval</tt>,
         *  <tt>BlankEval</tt> or <tt>ErrorEval</tt>. Never <c>null</c>.
         */
        private static ValueEval DereferenceValue(ValueEval evaluationResult, int srcRowNum, int srcColNum)
        {
            if (evaluationResult is RefEval)
            {
                RefEval rv = (RefEval)evaluationResult;
                return rv.InnerValueEval;
            }
            if (evaluationResult is AreaEval)
            {
                AreaEval ae = (AreaEval)evaluationResult;
                if (ae.IsRow)
                {
                    if (ae.IsColumn)
                    {
                        return ae.GetRelativeValue(0, 0);
                    }
                    return ae.GetValueAt(ae.FirstRow, srcColNum);
                }
                if (ae.IsColumn)
                {
                    return ae.GetValueAt(srcRowNum, ae.FirstColumn);
                }
                return ErrorEval.VALUE_INVALID;
            }
            return evaluationResult;
        }
        /**
         * returns an appropriate Eval impl instance for the Ptg. The Ptg must be
         * one of: Area3DPtg, AreaPtg, ReferencePtg, Ref3DPtg, IntPtg, NumberPtg,
         * StringPtg, BoolPtg <br/>special Note: OperationPtg subtypes cannot be
         * passed here!
         */
        private ValueEval GetEvalForPtg(Ptg ptg, OperationEvaluationContext ec)
        {
            //  consider converting all these (ptg is XxxPtg) expressions To (ptg.GetType() == XxxPtg.class)

            if (ptg is NamePtg)
            {
                // named ranges, macro functions
                NamePtg namePtg = (NamePtg)ptg;
                EvaluationName nameRecord = _workbook.GetName(namePtg);
                if (nameRecord.IsFunctionName)
                {
                    return new NameEval(nameRecord.NameText);
                }
                if (nameRecord.HasFormula)
                {
                    return EvaluateNameFormula(nameRecord.NameDefinition, ec);
                }

                throw new Exception("Don't now how To evalate name '" + nameRecord.NameText + "'");
            }
            if (ptg is NameXPtg)
            {
                return new NameXEval(((NameXPtg)ptg));
            }

            if (ptg is IntPtg)
            {
                return new NumberEval(((IntPtg)ptg).Value);
            }
            if (ptg is NumberPtg)
            {
                return new NumberEval(((NumberPtg)ptg).Value);
            }
            if (ptg is StringPtg)
            {
                return new StringEval(((StringPtg)ptg).Value);
            }
            if (ptg is BoolPtg)
            {
                return BoolEval.ValueOf(((BoolPtg)ptg).Value);
            }
            if (ptg is ErrPtg)
            {
                return ErrorEval.ValueOf(((ErrPtg)ptg).ErrorCode);
            }
            if (ptg is MissingArgPtg)
            {
                return MissingArgEval.instance;
            }
            if (ptg is AreaErrPtg || ptg is RefErrorPtg
                    || ptg is DeletedArea3DPtg || ptg is DeletedRef3DPtg)
            {
                return ErrorEval.REF_INVALID;
            }
            if (ptg is Ref3DPtg)
            {
                Ref3DPtg rptg = (Ref3DPtg)ptg;
                return ec.GetRef3DEval(rptg.Row, rptg.Column, rptg.ExternSheetIndex);
            }
            if (ptg is Area3DPtg)
            {
                Area3DPtg aptg = (Area3DPtg)ptg;
                return ec.GetArea3DEval(aptg.FirstRow, aptg.FirstColumn, aptg.LastRow, aptg.LastColumn, aptg.ExternSheetIndex);
            }
            if (ptg is RefPtg)
            {
                RefPtg rptg = (RefPtg)ptg;
                return ec.GetRefEval(rptg.Row, rptg.Column);
            }
            if (ptg is AreaPtg)
            {
                AreaPtg aptg = (AreaPtg)ptg;
                return ec.GetAreaEval(aptg.FirstRow, aptg.FirstColumn, aptg.LastRow, aptg.LastColumn);
            }

            if (ptg is UnknownPtg)
            {
                // POI uses UnknownPtg when the encoded Ptg array seems To be corrupted.
                // This seems To occur in very rare cases (e.g. unused name formulas in bug 44774, attachment 21790)
                // In any case, formulas are re-parsed before execution, so UnknownPtg should not Get here
                throw new Exception("UnknownPtg not allowed");
            }

            throw new Exception("Unexpected ptg class (" + ptg.GetType().Name + ")");
        }
        private ValueEval EvaluateNameFormula(Ptg[] ptgs, OperationEvaluationContext ec)
        {
            if (ptgs.Length > 1)
            {
                throw new Exception("Complex name formulas not supported yet");
            }
            return GetEvalForPtg(ptgs[0], ec);
        }

        /**
         * Used by the lazy ref evals whenever they need To Get the value of a contained cell.
         */
        /* package */
        public ValueEval EvaluateReference(EvaluationSheet sheet, int sheetIndex, int rowIndex,
            int columnIndex, EvaluationTracker tracker)
        {

            EvaluationCell cell = sheet.GetCell(rowIndex, columnIndex);
            return EvaluateAny(cell, sheetIndex, rowIndex, columnIndex, tracker);
        }

        public FreeRefFunction FindUserDefinedFunction(String functionName)
        {
            return _udfFinder.FindFunction(functionName);
        }
    }
}