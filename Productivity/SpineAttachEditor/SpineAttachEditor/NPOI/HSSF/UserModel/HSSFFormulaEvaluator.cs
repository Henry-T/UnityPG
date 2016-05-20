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

namespace NPOI.HSSF.UserModel
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;

    using NPOI.Util;
    using NPOI.HSSF.Model;
    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.Record.Formula.Eval;
    using NPOI.SS.Formula;
    using NPOI.HSSF.Record.Formula.Udf;
    using NPOI.SS.UserModel;

    /**
     * @author Amol S. Deshmukh &lt; amolweb at ya hoo dot com &gt;
     * 
     */
    public class HSSFFormulaEvaluator:FormulaEvaluator
    {
        private WorkbookEvaluator _bookEvaluator;

        // params to lookup the right constructor using reflection
        private static Type[] VALUE_CONTRUCTOR_CLASS_ARRAY = new Type[] { typeof(Ptg) };

        private static Type[] AREA3D_CONSTRUCTOR_CLASS_ARRAY = new Type[] { typeof(Ptg), typeof(ValueEval[]) };

        private static Type[] REFERENCE_CONSTRUCTOR_CLASS_ARRAY = new Type[] { typeof(Ptg), typeof(ValueEval) };

        private static Type[] REF3D_CONSTRUCTOR_CLASS_ARRAY = new Type[] { typeof(Ptg), typeof(ValueEval) };

        // Maps for mapping *Eval to *Ptg
        private static Hashtable VALUE_EVALS_MAP = new Hashtable();

        /*
         * Following is the mapping between the Ptg tokens returned 
         * by the FormulaParser and the *Eval classes that are used 
         * by the FormulaEvaluator
         */
        static HSSFFormulaEvaluator()
        {
            VALUE_EVALS_MAP[typeof(BoolPtg)] = typeof(BoolEval);
            VALUE_EVALS_MAP[typeof(IntPtg)] = typeof(NumberEval);
            VALUE_EVALS_MAP[typeof(NumberPtg)] = typeof(NumberEval);
            VALUE_EVALS_MAP[typeof(StringPtg)] = typeof(StringEval);

        }


        protected NPOI.SS.UserModel.Row row;
        protected NPOI.SS.UserModel.Sheet sheet;
        protected NPOI.SS.UserModel.Workbook workbook;

        public HSSFFormulaEvaluator(NPOI.SS.UserModel.Sheet sheet, NPOI.SS.UserModel.Workbook workbook)
            : this(workbook)
        {
            this.sheet = sheet;
            this.workbook = workbook;
        }

        public HSSFFormulaEvaluator(NPOI.SS.UserModel.Workbook workbook)
            : this(workbook, null)
        {
            
        }
        /**
 * @param stabilityClassifier used to optimise caching performance. Pass <code>null</code>
 * for the (conservative) assumption that any cell may have its definition changed after
 * evaluation begins.
 */
        public HSSFFormulaEvaluator(NPOI.SS.UserModel.Workbook workbook, IStabilityClassifier stabilityClassifier)
            : this(workbook, stabilityClassifier, null)
        {
            
        }

        /**
         * @param udfFinder pass <code>null</code> for default (AnalysisToolPak only)
         */
        private HSSFFormulaEvaluator(NPOI.SS.UserModel.Workbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
        {
            _bookEvaluator = new WorkbookEvaluator(HSSFEvaluationWorkbook.Create(workbook), stabilityClassifier, udfFinder);
        }
        private static void SetCellType(NPOI.SS.UserModel.Cell cell, NPOI.SS.UserModel.CellValue cv)
        {
            NPOI.SS.UserModel.CellType cellType = cv.CellType;
            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                case NPOI.SS.UserModel.CellType.ERROR:
                case NPOI.SS.UserModel.CellType.NUMERIC:
                case NPOI.SS.UserModel.CellType.STRING:
                    cell.SetCellType(cellType);
                    return;
                case NPOI.SS.UserModel.CellType.BLANK:
                // never happens - blanks eventually get translated to zero
                    break;
                case NPOI.SS.UserModel.CellType.FORMULA:
                // this will never happen, we have already evaluated the formula
                    break;
            }
            throw new InvalidOperationException("Unexpected cell value type (" + cellType + ")");
        }
        private static void SetCellValue(NPOI.SS.UserModel.Cell cell, NPOI.SS.UserModel.CellValue cv)
        {
            NPOI.SS.UserModel.CellType cellType = cv.CellType;
            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    cell.SetCellValue(cv.BooleanValue);
                    break;
                case NPOI.SS.UserModel.CellType.ERROR:
                    cell.CellErrorValue=cv.ErrorValue;
                    break;
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    cell.SetCellValue(cv.NumberValue);
                    break;
                case NPOI.SS.UserModel.CellType.STRING:
                    cell.SetCellValue(new HSSFRichTextString(cv.StringValue));
                    break;
                //case NPOI.SS.UserModel.CellType.BLANK:
                //// never happens - blanks eventually get translated to zero
                //case NPOI.SS.UserModel.CellType.FORMULA:
                //// this will never happen, we have already evaluated the formula
                default:
                    throw new InvalidOperationException("Unexpected cell value type (" + cellType + ")");
            }
        }
        /**
         * Coordinates several formula evaluators together so that formulas that involve external
         * references can be evaluated.
         * @param workbookNames the simple file names used to identify the workbooks in formulas
         * with external links (for example "MyData.xls" as used in a formula "[MyData.xls]Sheet1!A1")
         * @param evaluators all evaluators for the full set of workbooks required by the formulas. 
         */
        public static void SetupEnvironment(String[] workbookNames, HSSFFormulaEvaluator[] evaluators)
        {
            WorkbookEvaluator[] wbEvals = new WorkbookEvaluator[evaluators.Length];
            for (int i = 0; i < wbEvals.Length; i++)
            {
                wbEvals[i] = evaluators[i]._bookEvaluator;
            }
            CollaboratingWorkbooksEnvironment.Setup(workbookNames, wbEvals);
        }

        /**
         * If cell Contains a formula, the formula is Evaluated and returned,
         * else the CellValue simply copies the appropriate cell value from
         * the cell and also its cell type. This method should be preferred over
         * EvaluateInCell() when the call should not modify the contents of the
         * original cell. 
         * @param cell
         */
        /**
         * If cell contains a formula, the formula is evaluated and returned,
         * else the CellValue simply copies the appropriate cell value from
         * the cell and also its cell type. This method should be preferred over
         * evaluateInCell() when the call should not modify the contents of the
         * original cell.
         * 
         * @param cell may be <c>null</c> signifying that the cell is not present (or blank)
         * @return <c>null</c> if the supplied cell is <c>null</c> or blank
         */
        public NPOI.SS.UserModel.CellValue Evaluate(NPOI.SS.UserModel.Cell cell)
        {
            if (cell == null)
            {
                return null;
            }

            switch (cell.CellType)
            {
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    return NPOI.SS.UserModel.CellValue.ValueOf(cell.BooleanCellValue);
                case NPOI.SS.UserModel.CellType.ERROR:
                    return NPOI.SS.UserModel.CellValue.GetError(cell.ErrorCellValue);
                case NPOI.SS.UserModel.CellType.FORMULA:
                    return EvaluateFormulaCellValue(cell);
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    return new NPOI.SS.UserModel.CellValue(cell.NumericCellValue);
                case NPOI.SS.UserModel.CellType.STRING:
                    return new NPOI.SS.UserModel.CellValue(cell.RichStringCellValue.String);
                case NPOI.SS.UserModel.CellType.BLANK:
                    return null;
            }
            throw new InvalidOperationException("Bad cell type (" + cell.CellType + ")");
        }


        /**
 * Should be called whenever there are major changes (e.g. moving sheets) to input cells
 * in the evaluated workbook.  If performance is not critical, a single call to this method
 * may be used instead of many specific calls to the notify~ methods.
 *  
 * Failure to call this method after changing cell values will cause incorrect behaviour
 * of the evaluate~ methods of this class
 */
        public void ClearAllCachedResultValues()
        {
            _bookEvaluator.ClearAllCachedResultValues();
        }
        /**
         * Should be called to tell the cell value cache that the specified (value or formula) cell 
         * has changed.
         * Failure to call this method after changing cell values will cause incorrect behaviour
         * of the evaluate~ methods of this class
         */
        public void NotifyUpdateCell(Cell cell)
        {
            _bookEvaluator.NotifyUpdateCell(new HSSFEvaluationCell(cell));
        }
        /**
         * Should be called to tell the cell value cache that the specified cell has just been
         * deleted. 
         * Failure to call this method after changing cell values will cause incorrect behaviour
         * of the evaluate~ methods of this class
         */
        public void NotifyDeleteCell(Cell cell)
        {
            _bookEvaluator.NotifyDeleteCell(new HSSFEvaluationCell(cell));
        }

        /**
         * Should be called to tell the cell value cache that the specified (value or formula) cell
         * has changed.
         * Failure to call this method after changing cell values will cause incorrect behaviour
         * of the evaluate~ methods of this class
         */
        public void NotifySetFormula(Cell cell)
        {
            _bookEvaluator.NotifyUpdateCell(new HSSFEvaluationCell(cell));
        }

        /**
         * If cell Contains formula, it Evaluates the formula,
         *  and saves the result of the formula. The cell
         *  remains as a formula cell.
         * Else if cell does not contain formula, this method leaves
         *  the cell UnChanged. 
         * Note that the type of the formula result is returned,
         *  so you know what kind of value is also stored with
         *  the formula. 
         * <pre>
         * int EvaluatedCellType = evaluator.EvaluateFormulaCell(cell);
         * </pre>
         * Be aware that your cell will hold both the formula,
         *  and the result. If you want the cell Replaced with
         *  the result of the formula, use {@link #EvaluateInCell(HSSFCell)}
         * @param cell The cell to Evaluate
         * @return The type of the formula result (the cell's type remains as NPOI.SS.UserModel.CellType.FORMULA however)
         */
        public NPOI.SS.UserModel.CellType EvaluateFormulaCell(NPOI.SS.UserModel.Cell cell)
        {
            if (cell == null || cell.CellType != NPOI.SS.UserModel.CellType.FORMULA)
            {
                return NPOI.SS.UserModel.CellType.Unknown;
            }
            NPOI.SS.UserModel.CellValue cv = EvaluateFormulaCellValue(cell);
            // cell remains a formula cell, but the cached value is changed
            SetCellValue(cell, cv);
            return cv.CellType;
        }
        /**
         * Returns a CellValue wrapper around the supplied ValueEval instance.
         * @param eval
         */
        private NPOI.SS.UserModel.CellValue EvaluateFormulaCellValue(NPOI.SS.UserModel.Cell cell)
        {
            ValueEval eval = _bookEvaluator.Evaluate(new HSSFEvaluationCell((HSSFCell)cell));
            if (eval is NumberEval)
            {
                NumberEval ne = (NumberEval)eval;
                return new NPOI.SS.UserModel.CellValue(ne.NumberValue);
            }
            if (eval is BoolEval)
            {
                BoolEval be = (BoolEval)eval;
                return NPOI.SS.UserModel.CellValue.ValueOf(be.BooleanValue);
            }
            if (eval is StringEval)
            {
                StringEval ne = (StringEval)eval;
                return new NPOI.SS.UserModel.CellValue(ne.StringValue);
            }
            if (eval is ErrorEval)
            {
                return NPOI.SS.UserModel.CellValue.GetError(((ErrorEval)eval).ErrorCode);
            }
            throw new InvalidOperationException("Unexpected eval class (" + eval.GetType().Name + ")");
        }
        /**
         * If cell Contains formula, it Evaluates the formula, and
         *  puts the formula result back into the cell, in place
         *  of the old formula.
         * Else if cell does not contain formula, this method leaves
         *  the cell UnChanged. 
         * Note that the same instance of Cell is returned to 
         * allow chained calls like:
         * <pre>
         * int EvaluatedCellType = evaluator.EvaluateInCell(cell).CellType;
         * </pre>
         * Be aware that your cell value will be Changed to hold the
         *  result of the formula. If you simply want the formula
         *  value computed for you, use {@link #EvaluateFormulaCell(HSSFCell)}
         * @param cell
         */
        public NPOI.SS.UserModel.Cell EvaluateInCell(NPOI.SS.UserModel.Cell cell)
        {
            if (cell == null)
            {
                return null;
            }
            if (cell.CellType == NPOI.SS.UserModel.CellType.FORMULA)
            {
                NPOI.SS.UserModel.CellValue cv = EvaluateFormulaCellValue(cell);
                SetCellValue(cell, cv);
                SetCellType(cell, cv); // cell will no longer be a formula cell
            }
            return cell;
        }

        /**
         * Loops over all cells in all sheets of the supplied
         *  workbook.
         * For cells that contain formulas, their formulas are
         *  Evaluated, and the results are saved. These cells
         *  remain as formula cells.
         * For cells that do not contain formulas, no Changes
         *  are made.
         * This is a helpful wrapper around looping over all 
         *  cells, and calling EvaluateFormulaCell on each one.
         */
        public static void EvaluateAllFormulaCells(HSSFWorkbook wb)
        {
            for (int i = 0; i < wb.NumberOfSheets; i++)
            {
                NPOI.SS.UserModel.Sheet sheet = wb.GetSheetAt(i);
                HSSFFormulaEvaluator evaluator = new HSSFFormulaEvaluator(sheet, wb);

                for (IEnumerator rit = sheet.GetRowEnumerator(); rit.MoveNext(); )
                {
                    HSSFRow r = (HSSFRow)rit.Current;
                    //evaluator.SetCurrentRow(r);

                    for (IEnumerator cit = r.GetCellEnumerator(); cit.MoveNext(); )
                    {
                        NPOI.SS.UserModel.Cell c = (HSSFCell)cit.Current;
                        if (c.CellType == NPOI.SS.UserModel.CellType.FORMULA)
                            evaluator.EvaluateFormulaCell(c);
                    }
                }
            }
        }


        /**
         * Mimics the 'data view' of a cell. This allows formula evaluator 
         * to return a CellValue instead of precasting the value to String
         * or Number or bool type.
         * @author Amol S. Deshmukh &lt; amolweb at ya hoo dot com &gt;
         */
        public class CellValue
        {
            public static CellValue TRUE = new CellValue(NPOI.SS.UserModel.CellType.BOOLEAN, 0.0, true, null, 0);
            public static CellValue FALSE = new CellValue(NPOI.SS.UserModel.CellType.BOOLEAN, 0.0, false, null, 0);


            private NPOI.SS.UserModel.CellType _cellType;
            private double _numberValue;
            private bool _booleanValue;
            private String _textValue;
            private int _errorCode;

            private CellValue(NPOI.SS.UserModel.CellType cellType, double numberValue, bool booleanValue,
        String textValue, int errorCode)
            {
                _cellType = cellType;
                _numberValue = numberValue;
                _booleanValue = booleanValue;
                _textValue = textValue;
                _errorCode = errorCode;
            }

            /* package*/
            internal CellValue(double numberValue) :
                this(NPOI.SS.UserModel.CellType.NUMERIC, numberValue, false, null, 0)
            {

            }
            /* package*/
            internal CellValue(String stringValue)
                : this(NPOI.SS.UserModel.CellType.STRING, 0.0, false, stringValue, 0)
            {

            }
            /* package*/
            internal static CellValue ValueOf(bool booleanValue)
            {
                return booleanValue ? TRUE : FALSE;
            }
            /* package*/
            internal static CellValue GetError(int errorCode)
            {
                return new CellValue(NPOI.SS.UserModel.CellType.ERROR, 0.0, false, null, errorCode);
            }
            /**
             * @return Returns the boolValue.
             */
            public bool BooleanValue
            {
                get { return _booleanValue; }
                set { this._booleanValue = value; }
            }
            /**
             * @return Returns the numberValue.
             */
            public double NumberValue
            {
                get { return _numberValue; }
                set { this._numberValue = value; }
            }
            /**
             * @return Returns the stringValue. This method is deprecated, use
             * GetRichTextStringValue instead
             * @deprecated
             */
            public String StringValue
            {
                get { return _textValue; }
            }
            /**
             * @return Returns the cellType.
             */
            public NPOI.SS.UserModel.CellType CellType
            {
                get { return _cellType; }
            }
            /**
             * @return Returns the errorValue.
             */
            public int ErrorValue
            {
                get { return _errorCode; }
            }
            /**
             * @return Returns the richTextStringValue.
             */
            public HSSFRichTextString RichTextStringValue
            {
                get { return new HSSFRichTextString(_textValue); }
            }
            public String FormatAsString()
            {
                switch (_cellType)
                {
                    case NPOI.SS.UserModel.CellType.NUMERIC:
                        return _numberValue.ToString();
                    case NPOI.SS.UserModel.CellType.STRING:
                        return '"' + _textValue + '"';
                    case NPOI.SS.UserModel.CellType.BOOLEAN:
                        return _booleanValue ? "TRUE" : "FALSE";
                    case NPOI.SS.UserModel.CellType.ERROR:
                        return ErrorEval.GetText(_errorCode);
                }
                return "<error unexpected cell type " + _cellType + ">";
            }
        }
    }
}