/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) Under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You Under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed Under the License is distributed on an "AS Is" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations Under the License.
==================================================================== */


namespace NPOI.HSSF.UserModel
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using NPOI.Util;
    using NPOI.HSSF.Model;
    using NPOI.HSSF.Record;
    using NPOI.HSSF.Util;
    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.Record.Aggregates;
    using System.Collections.Generic;

    /// <summary>
    /// High level representation of a cell in a row of a spReadsheet.
    /// Cells can be numeric, formula-based or string-based (text).  The cell type
    /// specifies this.  String cells cannot conatin numbers and numeric cells cannot
    /// contain strings (at least according to our model).  Client apps should do the
    /// conversions themselves.  Formula cells have the formula string, as well as
    /// the formula result, which can be numeric or string.
    /// Cells should have their number (0 based) before being Added to a row.  Only
    /// cells that have values should be Added.
    /// </summary>
    /// <remarks>
    /// @author  Andrew C. Oliver (acoliver at apache dot org)
    /// @author  Dan Sherman (dsherman at Isisph.com)
    /// @author  Brian Sanders (kestrel at burdell dot org) Active Cell support
    /// @author  Yegor Kozlov cell comments support
    /// </remarks>
    [Serializable]
    public class HSSFCell:NPOI.SS.UserModel.Cell
    {

        public const short ENCODING_UNCHANGED = -1;
        public const short ENCODING_COMPRESSED_UNICODE = 0;
        public const short ENCODING_UTF_16 = 1;
        private NPOI.SS.UserModel.CellType cellType;
        private HSSFRichTextString stringValue;
        private short encoding = ENCODING_UNCHANGED;
        private HSSFWorkbook book;
        private HSSFSheet sheet;
        private CellValueRecordInterface record;
        private NPOI.SS.UserModel.Comment comment;

        /// <summary>
        /// Creates new Cell - Should only be called by HSSFRow.  This Creates a cell
        /// from scratch.
        /// When the cell is initially Created it is Set to NPOI.SS.UserModel.CellType.BLANK. Cell types
        /// can be Changed/overwritten by calling SetCellValue with the appropriate
        /// type as a parameter although conversions from one type to another may be
        /// prohibited.
        /// </summary>
        /// <param name="book">Workbook record of the workbook containing this cell</param>
        /// <param name="sheet">Sheet record of the sheet containing this cell</param>
        /// <param name="row">the row of this cell</param>
        /// <param name="col">the column for this cell</param>
        public HSSFCell(HSSFWorkbook book, HSSFSheet sheet, int row, short col):this(book,sheet,row,col,NPOI.SS.UserModel.CellType.BLANK)
        {
        }

        /// <summary>
        /// Creates new Cell - Should only be called by HSSFRow.  This Creates a cell
        /// from scratch.
        /// </summary>
        /// <param name="book">Workbook record of the workbook containing this cell</param>
        /// <param name="sheet">Sheet record of the sheet containing this cell</param>
        /// <param name="row">the row of this cell</param>
        /// <param name="col">the column for this cell</param>
        /// <param name="type">NPOI.SS.UserModel.CellType.NUMERIC, NPOI.SS.UserModel.CellType.STRING, NPOI.SS.UserModel.CellType.FORMULA, NPOI.SS.UserModel.CellType.BLANK,
        /// NPOI.SS.UserModel.CellType.BOOLEAN, NPOI.SS.UserModel.CellType.ERROR</param>
        public HSSFCell(HSSFWorkbook book, HSSFSheet sheet, int row, short col,
                           NPOI.SS.UserModel.CellType type)
        {
            CheckBounds(col);
            cellType = NPOI.SS.UserModel.CellType.Unknown; // Force 'SetCellType' to Create a first Record
            stringValue = null;
            this.book = book;
            this.sheet = sheet;

            short xfindex = sheet.Sheet.GetXFIndexForColAt(col);
            SetCellType(type, false, row, col, xfindex);
        }

        /// <summary>
        /// Creates an Cell from a CellValueRecordInterface.  HSSFSheet uses this when
        /// Reading in cells from an existing sheet.
        /// </summary>
        /// <param name="book">Workbook record of the workbook containing this cell</param>
        /// <param name="sheet">Sheet record of the sheet containing this cell</param>
        /// <param name="row">The row.</param>
        /// <param name="cval">the Cell Value Record we wish to represent</param>
        public HSSFCell(HSSFWorkbook book, HSSFSheet sheet, CellValueRecordInterface cval)
        {
            record = cval;
            cellType = DetermineType(cval);
            stringValue = null;
            this.book = book;
            this.sheet = sheet;
            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.STRING:
                    stringValue = new HSSFRichTextString(book.Workbook, (LabelSSTRecord)cval);
                    break;

                case NPOI.SS.UserModel.CellType.BLANK:
                    break;

                case NPOI.SS.UserModel.CellType.FORMULA:
                    stringValue = new HSSFRichTextString(((FormulaRecordAggregate)cval).StringValue);
                    break;
            }
            ExtendedFormatRecord xf = book.Workbook.GetExFormatAt(cval.XFIndex);

            CellStyle = new HSSFCellStyle((short)cval.XFIndex, xf, book);
        }

        /**
         * private constructor to prevent blank construction
         */
        private HSSFCell()
        {
        }

        /**
 * set a error value for the cell
 *
 * @param errorCode the error value to set this cell to.  For formulas we'll set the
 *        precalculated value , for errors we'll set
 *        its value. For other types we will change the cell to an error
 *        cell and set its value.
 */
        public byte CellErrorValue
        {
            get 
            {
                switch (cellType)
                {
                    case NPOI.SS.UserModel.CellType.ERROR:
                        return ((BoolErrRecord)record).ErrorValue;
                    default:
                        throw TypeMismatch(NPOI.SS.UserModel.CellType.ERROR, cellType, false);
                    case NPOI.SS.UserModel.CellType.FORMULA:
                        break;
                }
                FormulaRecord fr = ((FormulaRecordAggregate)record).FormulaRecord;
                CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType.ERROR, fr);
                return (byte)fr.CachedErrorValue;

            }
            set
            {
                int row = record.Row;
                int col = record.Column;
                short styleIndex = record.XFIndex;
                switch (cellType)
                {

                    case NPOI.SS.UserModel.CellType.ERROR:
                        ((BoolErrRecord)record).SetValue(value);
                        break;
                    case NPOI.SS.UserModel.CellType.FORMULA:
                        ((FormulaRecordAggregate)record).SetCachedErrorResult(value);
                        break;
                    default:
                        SetCellType(NPOI.SS.UserModel.CellType.ERROR, false, row, col, styleIndex);
                        break;
                }
            }
        }

        //public override int GetHashCode()
        //{
        //    int prime = 31;
        //    int result = 1;
        //    result = prime * result + this.RowIndex;
        //    result = prime * result + this.ColumnIndex;
        //    result = prime * result + this.CellType;
        //    result = prime * result + this.formula.GetHashCode();
        //    return result;
        //}

        /**
         * used internally -- given a cell value record, figure out its type
         */
        private NPOI.SS.UserModel.CellType DetermineType(CellValueRecordInterface cval)
        {
            if (cval is FormulaRecordAggregate)
            {
                return NPOI.SS.UserModel.CellType.FORMULA;
            }

            Record record = (Record)cval;
            int sid = record.Sid;

            switch (sid)
            {

                case NumberRecord.sid:
                    return NPOI.SS.UserModel.CellType.NUMERIC;


                case BlankRecord.sid:
                    return NPOI.SS.UserModel.CellType.BLANK;


                case LabelSSTRecord.sid:
                    return NPOI.SS.UserModel.CellType.STRING;


                case FormulaRecordAggregate.sid:
                    return NPOI.SS.UserModel.CellType.FORMULA;


                case BoolErrRecord.sid:
                    BoolErrRecord boolErrRecord = (BoolErrRecord)record;

                    return (boolErrRecord.IsBoolean)
                             ? NPOI.SS.UserModel.CellType.BOOLEAN
                             : NPOI.SS.UserModel.CellType.ERROR;

            }
            throw new Exception("Bad cell value rec (" + cval.GetType().Name + ")");
        }

        /**
         * Returns the Workbook that this Cell is bound to
         * @return
         */
        public Workbook BoundWorkbook
        {
            get
            {
                return book.Workbook;
            }
        }

        public NPOI.SS.UserModel.Sheet Sheet
        {
            get
            {
                return this.sheet;
            }
        }
        /**
 * Returns the HSSFRow this cell belongs to
 *
 * @return the HSSFRow that owns this cell
 */
        public NPOI.SS.UserModel.Row Row
        {
            get
            {
                int rowIndex = this.RowIndex;
                return sheet.GetRow(rowIndex);
            }
        }

        /// <summary>
        /// Set the cells type (numeric, formula or string)
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// @see #NPOI.SS.UserModel.CellType.NUMERIC
        /// @see #NPOI.SS.UserModel.CellType.STRING
        /// @see #NPOI.SS.UserModel.CellType.FORMULA
        /// @see #NPOI.SS.UserModel.CellType.BLANK
        /// @see #NPOI.SS.UserModel.CellType.BOOLEAN
        /// @see #NPOI.SS.UserModel.CellType.ERROR
        public void SetCellType(NPOI.SS.UserModel.CellType cellType)
        {
            NotifyFormulaChanging();
            int row = record.Row;
            int col = record.Column;
            short styleIndex = record.XFIndex;
            SetCellType(cellType, true, row, col, styleIndex);
        }

        /// <summary>
        /// Sets the cell type. The SetValue flag indicates whether to bother about
        /// trying to preserve the current value in the new record if one is Created.
        /// The SetCellValue method will call this method with false in SetValue
        /// since it will overWrite the cell value later
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="setValue">if set to <c>true</c> [set value].</param>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="styleIndex">Index of the style.</param>
        private void SetCellType(NPOI.SS.UserModel.CellType cellType, bool setValue, int row, int col, short styleIndex)
        {
            if (cellType > NPOI.SS.UserModel.CellType.ERROR)
            {
                throw new Exception("I have no idea what type that Is!");
            }
            switch (cellType)
            {

                case NPOI.SS.UserModel.CellType.FORMULA:
                    FormulaRecordAggregate frec = null;

                    if (cellType != this.cellType)
                    {
                        frec = sheet.Sheet.RowsAggregate.CreateFormula(row, col);
                    }
                    else
                    {
                        frec = (FormulaRecordAggregate)record;
                    }
                    frec.Column = col;
                    if (setValue)
                    {
                        frec.FormulaRecord.Value = NumericCellValue;
                    }
                    frec.XFIndex = styleIndex;
                    frec.Row = row;
                    record = frec;
                    break;

                case NPOI.SS.UserModel.CellType.NUMERIC:
                    NumberRecord nrec = null;

                    if (cellType != this.cellType)
                    {
                        nrec = new NumberRecord();
                    }
                    else
                    {
                        nrec = (NumberRecord)record;
                    }
                    nrec.Column = col;
                    if (setValue)
                    {
                        nrec.Value = NumericCellValue;
                    }
                    nrec.XFIndex = styleIndex;
                    nrec.Row = row;
                    record = nrec;
                    break;

                case NPOI.SS.UserModel.CellType.STRING:
                    LabelSSTRecord lrec = null;

                    if (cellType != this.cellType)
                    {
                        lrec = new LabelSSTRecord();
                    }
                    else
                    {
                        lrec = (LabelSSTRecord)record;
                    }
                    lrec.Column = col;
                    lrec.Row = row;
                    lrec.XFIndex = styleIndex;
                    if (setValue)
                    {
                        String str = ConvertCellValueToString();
                        int sstIndex = book.Workbook.AddSSTString(new UnicodeString(str));
                        lrec.SSTIndex = (sstIndex);
                        UnicodeString us = book.Workbook.GetSSTString(sstIndex);
                        stringValue = new HSSFRichTextString();
                        stringValue.UnicodeString = us;
                    }
                    record = lrec;
                    break;

                case NPOI.SS.UserModel.CellType.BLANK:
                    BlankRecord brec = null;

                    if (cellType != this.cellType)
                    {
                        brec = new BlankRecord();
                    }
                    else
                    {
                        brec = (BlankRecord)record;
                    }
                    brec.Column = col;

                    // During construction the cellStyle may be null for a Blank cell.
                    brec.XFIndex = styleIndex;
                    brec.Row = row;
                    record = brec;
                    break;

                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    BoolErrRecord boolRec = null;

                    if (cellType != this.cellType)
                    {
                        boolRec = new BoolErrRecord();
                    }
                    else
                    {
                        boolRec = (BoolErrRecord)record;
                    }
                    boolRec.Column = col;
                    if (setValue)
                    {
                        boolRec.SetValue(ConvertCellValueToBoolean());
                    }
                    boolRec.XFIndex = styleIndex;
                    boolRec.Row = row;
                    record = boolRec;
                    break;

                case NPOI.SS.UserModel.CellType.ERROR:
                    BoolErrRecord errRec = null;

                    if (cellType != this.cellType)
                    {
                        errRec = new BoolErrRecord();
                    }
                    else
                    {
                        errRec = (BoolErrRecord)record;
                    }
                    errRec.Column = col;
                    if (setValue)
                    {
                        errRec.SetValue((byte)HSSFErrorConstants.ERROR_VALUE);
                    }
                    errRec.XFIndex = styleIndex;
                    errRec.Row = row;
                    record = errRec;
                    break;
            }
            if (cellType != this.cellType &&
                this.cellType != NPOI.SS.UserModel.CellType.Unknown)  // Special Value to indicate an Uninitialized Cell
            {
                sheet.Sheet.ReplaceValueRecord(record);
            }
            this.cellType = cellType;
        }

        /// <summary>
        /// Get the cells type (numeric, formula or string)
        /// </summary>
        /// <value>The type of the cell.</value>
        public NPOI.SS.UserModel.CellType CellType
        {
            get
            {
                return cellType;
            }
        }
        private String ConvertCellValueToString()
        {

            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.BLANK:
                    return "";
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    return ((BoolErrRecord)record).BooleanValue ? "TRUE" : "FALSE";
                case NPOI.SS.UserModel.CellType.STRING:
                    int sstIndex = ((LabelSSTRecord)record).SSTIndex;
                    return book.Workbook.GetSSTString(sstIndex).String;
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    return ((NumberRecord)record).Value.ToString();
                case NPOI.SS.UserModel.CellType.ERROR:
                    return HSSFErrorConstants.GetText(((BoolErrRecord)record).ErrorValue);
                case NPOI.SS.UserModel.CellType.FORMULA:
                    // should really evaluate, but Cell can't call HSSFFormulaEvaluator
                    // just use cached formula result instead
                    break;
                default:
                    throw new InvalidDataException("Unexpected cell type (" + cellType + ")");
                    break;
            }
            FormulaRecordAggregate fra = ((FormulaRecordAggregate)record);
            FormulaRecord fr = fra.FormulaRecord;
            switch (fr.CachedResultType)
            {
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    return fr.CachedBooleanValue ? "TRUE" : "FALSE";
                case NPOI.SS.UserModel.CellType.STRING:
                    return fra.StringValue;
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    return fr.Value.ToString(); //return NumberToTextConverter.toText(fr.Value);
                case NPOI.SS.UserModel.CellType.ERROR:
                    return HSSFErrorConstants.GetText(fr.CachedErrorValue);
            }
            throw new InvalidDataException("Unexpected formula result type (" + cellType + ")");

        }
        /// <summary>
        /// Set a numeric value for the cell
        /// </summary>
        /// <param name="value">the numeric value to Set this cell to.  For formulas we'll Set the
        /// precalculated value, for numerics we'll Set its value. For other types we
        /// will Change the cell to a numeric cell and Set its value.</param>
        public void SetCellValue(double value)
        {
            int row = record.Row;
            int col = record.Column;
            short styleIndex = record.XFIndex;

            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    ((NumberRecord)record).Value=value;
                    break;
                case NPOI.SS.UserModel.CellType.FORMULA:
                    ((FormulaRecordAggregate)record).SetCachedDoubleResult(value);
                    break;
                default:
                    SetCellType(NPOI.SS.UserModel.CellType.NUMERIC, false, row, col, styleIndex);
                    ((NumberRecord)record).Value = value;
                    break;
            }
        }

        /// <summary>
        /// Set a date value for the cell. Excel treats dates as numeric so you will need to format the cell as
        /// a date.
        /// </summary>
        /// <param name="value">the date value to Set this cell to.  For formulas we'll Set the
        /// precalculated value, for numerics we'll Set its value. For other types we
        /// will Change the cell to a numeric cell and Set its value.</param>
        public void SetCellValue(DateTime value)
        {
            SetCellValue(NPOI.SS.UserModel.DateUtil.GetExcelDate(value, this.book.Workbook.IsUsing1904DateWindowing));
        }


        /// <summary>
        /// Set a string value for the cell. Please note that if you are using
        /// full 16 bit Unicode you should call SetEncoding() first.
        /// </summary>
        /// <param name="value">value to Set the cell to.  For formulas we'll Set the formula
        /// string, for String cells we'll Set its value.  For other types we will
        /// Change the cell to a string cell and Set its value.
        /// If value is null then we will Change the cell to a Blank cell.</param>
        public void SetCellValue(String value)
        {
            HSSFRichTextString str = new HSSFRichTextString(value);
            SetCellValue(str);
        }
        /**
 * set a error value for the cell
 *
 * @param errorCode the error value to set this cell to.  For formulas we'll set the
 *        precalculated value , for errors we'll set
 *        its value. For other types we will change the cell to an error
 *        cell and set its value.
 */
        public void SetCellErrorValue(byte errorCode)
        {
            int row = record.Row;
            int col = record.Column;
            short styleIndex = record.XFIndex;
            switch (cellType)
            {

                case NPOI.SS.UserModel.CellType.ERROR:
                    ((BoolErrRecord)record).SetValue(errorCode);
                    break;
                case NPOI.SS.UserModel.CellType.FORMULA:
                    ((FormulaRecordAggregate)record).SetCachedErrorResult(errorCode);
                    break;
                default:
                    SetCellType(NPOI.SS.UserModel.CellType.ERROR, false, row, col, styleIndex);
                    break;
            }
        }
        /// <summary>
        /// Set a string value for the cell. Please note that if you are using
        /// full 16 bit Unicode you should call SetEncoding() first.
        /// </summary>
        /// <param name="value">value to Set the cell to.  For formulas we'll Set the formula
        /// string, for String cells we'll Set its value.  For other types we will
        /// Change the cell to a string cell and Set its value.
        /// If value is null then we will Change the cell to a Blank cell.</param>
        public void SetCellValue(NPOI.SS.UserModel.RichTextString value)
        {
            HSSFRichTextString hvalue = (HSSFRichTextString)value;

            int row = record.Row;
            int col = record.Column;
            short styleIndex = record.XFIndex;
            if (hvalue == null)
            {
                NotifyFormulaChanging();
                SetCellType(NPOI.SS.UserModel.CellType.BLANK, false, row, col, styleIndex);
                return;
            }

            if (hvalue.Length > NPOI.SS.SpreadsheetVersion.EXCEL97.MaxTextLength)
            {
                throw new ArgumentException("The maximum length of cell contents (text) is 32,767 characters");
            }
            if (cellType == NPOI.SS.UserModel.CellType.FORMULA)
            {
                // Set the 'pre-Evaluated result' for the formula 
                // note - formulas do not preserve text formatting.
                FormulaRecordAggregate fr = (FormulaRecordAggregate)record;
                fr.SetCachedStringResult(value.String);
                // Update our local cache to the un-formatted version
                stringValue = new HSSFRichTextString(value.String);
                return;
            }

            if (cellType != NPOI.SS.UserModel.CellType.STRING)
            {
                SetCellType(NPOI.SS.UserModel.CellType.STRING, false, row, col, styleIndex);
            }
            int index = 0;

            UnicodeString str = hvalue.UnicodeString;
            index = book.Workbook.AddSSTString(str);
            ((LabelSSTRecord)record).SSTIndex = index;
            stringValue = hvalue;
            stringValue.SetWorkbookReferences(book.Workbook, ((LabelSSTRecord)record));
            stringValue.UnicodeString = book.Workbook.GetSSTString(index);
        }

        /**
 * Should be called any time that a formula could potentially be deleted.
 * Does nothing if this cell currently does not hold a formula
 */
        private void NotifyFormulaChanging()
        {
            if (record is FormulaRecordAggregate)
            {   
                ((FormulaRecordAggregate)record).NotifyFormulaChanging();
            }
        }

        /// <summary>
        /// Gets or sets the cell formula.
        /// </summary>
        /// <value>The cell formula.</value>
        public String CellFormula
        {
            get
            {
                if (record is FormulaRecordAggregate)
                {
                    return HSSFFormulaParser.ToFormulaString(book, ((FormulaRecordAggregate)record).FormulaRecord.ParsedExpression);
                }
                else
                {
                    return "";
                }
            }
            set 
            {
                int row = record.Row;
                int col = record.Column;
                short styleIndex = record.XFIndex;

                if (string.IsNullOrEmpty(value))
                {
                    NotifyFormulaChanging();
                    SetCellType(NPOI.SS.UserModel.CellType.BLANK, false, row, col, styleIndex);
                    return;
                }
                int sheetIndex = book.GetSheetIndex(sheet);
                Ptg[] ptgs = HSSFFormulaParser.Parse(value, book, NPOI.SS.Formula.FormulaType.CELL, sheetIndex);

                SetCellType(NPOI.SS.UserModel.CellType.FORMULA, false, row, col, styleIndex);
                FormulaRecordAggregate agg = (FormulaRecordAggregate)record;
                FormulaRecord frec = agg.FormulaRecord;
                frec.Options = ((short)2);
                frec.Value = (0);

                //only set to default if there is no extended format index already set
                if (agg.XFIndex == (short)0)
                {
                    agg.XFIndex = ((short)0x0f);
                }
                agg.SetParsedExpression(ptgs);
            }
        }


        /// <summary>
        /// Get the value of the cell as a number.  For strings we throw an exception.
        /// For blank cells we return a 0.
        /// </summary>
        /// <value>The numeric cell value.</value>
        public double NumericCellValue
        {
            get
            {
                switch (cellType)
                {
                    case NPOI.SS.UserModel.CellType.BLANK:
                        return 0.0;
                        
                    case NPOI.SS.UserModel.CellType.NUMERIC:
                        return ((NumberRecord)record).Value;    
                    case NPOI.SS.UserModel.CellType.FORMULA:
                        break;
                    default:
                        throw TypeMismatch(NPOI.SS.UserModel.CellType.NUMERIC, cellType, false);
                        break;
                }
                FormulaRecord fr=((FormulaRecordAggregate)record).FormulaRecord;
                CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType.NUMERIC, fr);
                return fr.Value;
            }
        }

        /// <summary>
        /// Used to help format error messages
        /// </summary>
        /// <param name="cellTypeCode">The cell type code.</param>
        /// <returns></returns>
        private String GetCellTypeName(NPOI.SS.UserModel.CellType cellTypeCode)
        {
            switch (cellTypeCode)
            {
                case NPOI.SS.UserModel.CellType.BLANK: return "blank";
                case NPOI.SS.UserModel.CellType.STRING: return "text";
                case NPOI.SS.UserModel.CellType.BOOLEAN: return "boolean";
                case NPOI.SS.UserModel.CellType.ERROR: return "error";
                case NPOI.SS.UserModel.CellType.NUMERIC: return "numeric";
                case NPOI.SS.UserModel.CellType.FORMULA: return "formula";
            }
            return "#unknown cell type (" + cellTypeCode + ")#";
        }


        /// <summary>
        /// Types the mismatch.
        /// </summary>
        /// <param name="expectedTypeCode">The expected type code.</param>
        /// <param name="actualTypeCode">The actual type code.</param>
        /// <param name="isFormulaCell">if set to <c>true</c> [is formula cell].</param>
        /// <returns></returns>
        private Exception TypeMismatch(NPOI.SS.UserModel.CellType expectedTypeCode, NPOI.SS.UserModel.CellType actualTypeCode, bool isFormulaCell)
        {
            String msg = "Cannot get a "
                + GetCellTypeName(expectedTypeCode) + " value from a "
                + GetCellTypeName(actualTypeCode) + " " + (isFormulaCell ? "formula " : "") + "cell";
            return new InvalidOperationException(msg);
        }

        /// <summary>
        /// Checks the type of the formula cached value.
        /// </summary>
        /// <param name="expectedTypeCode">The expected type code.</param>
        /// <param name="fr">The fr.</param>
        private void CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType expectedTypeCode, FormulaRecord fr)
        {
            NPOI.SS.UserModel.CellType cachedValueType = fr.CachedResultType;
            if (cachedValueType != expectedTypeCode)
            {
                throw TypeMismatch(expectedTypeCode, cachedValueType, true);
            }
        }


        /// <summary>
        /// Get the value of the cell as a date.  For strings we throw an exception.
        /// For blank cells we return a null.
        /// </summary>
        /// <value>The date cell value.</value>
        public DateTime DateCellValue
        {
            get
            {
                if (cellType == NPOI.SS.UserModel.CellType.BLANK)
                {
                    return DateTime.MaxValue;
                }
                if (cellType == NPOI.SS.UserModel.CellType.STRING)
                {
                    throw new InvalidDataException(
                        "You cannot Get a date value from a String based cell");
                }
                if (cellType == NPOI.SS.UserModel.CellType.BOOLEAN)
                {
                    throw new InvalidDataException(
                        "You cannot Get a date value from a bool cell");
                }
                if (cellType == NPOI.SS.UserModel.CellType.ERROR)
                {
                    throw new InvalidDataException(
                        "You cannot Get a date value from an error cell");
                }
                double value = this.NumericCellValue;
                if (book.Workbook.IsUsing1904DateWindowing)
                {
                    return NPOI.SS.UserModel.DateUtil.GetJavaDate(value, true);
                }
                else
                {
                    return NPOI.SS.UserModel.DateUtil.GetJavaDate(value, false);
                }
            }
        }

        /// <summary>
        /// Get the value of the cell as a string - for numeric cells we throw an exception.
        /// For blank cells we return an empty string.
        /// For formulaCells that are not string Formulas, we return empty String
        /// </summary>
        /// <value>The string cell value.</value>
        public String StringCellValue
        {
            get
            {
                NPOI.SS.UserModel.RichTextString str = RichStringCellValue;
                return str.String;
            }
        }

        /// <summary>
        /// Get the value of the cell as a string - for numeric cells we throw an exception.
        /// For blank cells we return an empty string.
        /// For formulaCells that are not string Formulas, we return empty String
        /// </summary>
        /// <value>The rich string cell value.</value>
        public NPOI.SS.UserModel.RichTextString RichStringCellValue
        {
            get
            {
                switch (cellType)
                {
                    case NPOI.SS.UserModel.CellType.BLANK:
                        return new HSSFRichTextString("");
                    case NPOI.SS.UserModel.CellType.STRING:
                        return stringValue;
                    case NPOI.SS.UserModel.CellType.FORMULA:
                        break;
                    default:
                        throw TypeMismatch(NPOI.SS.UserModel.CellType.STRING, cellType, false);
                }
                FormulaRecordAggregate fra = ((FormulaRecordAggregate)record);
                CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType.STRING, fra.FormulaRecord);
                String strVal = fra.StringValue;
                return new HSSFRichTextString(strVal == null ? "" : strVal);
            }
        }

        /// <summary>
        /// Set a bool value for the cell
        /// </summary>
        /// <param name="value">the bool value to Set this cell to.  For formulas we'll Set the
        /// precalculated value, for bools we'll Set its value. For other types we
        /// will Change the cell to a bool cell and Set its value.</param>
        public void SetCellValue(bool value)
        {
            int row = record.Row;
            int col = record.Column;
            short styleIndex = record.XFIndex;
            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    ((BoolErrRecord)record).SetValue(value);
                    break;
                case NPOI.SS.UserModel.CellType.FORMULA:
                    ((FormulaRecordAggregate)record).SetCachedBooleanResult(value);
                    break;
                default:
                    SetCellType(NPOI.SS.UserModel.CellType.BOOLEAN, false, row, col, styleIndex);
                    ((BoolErrRecord)record).SetValue(value);
                    break;
            }
        }
        /// <summary>
        /// Chooses a new bool value for the cell when its type is changing.
        /// Usually the caller is calling SetCellType() with the intention of calling
        /// SetCellValue(bool) straight afterwards.  This method only exists to give
        /// the cell a somewhat reasonable value Until the SetCellValue() call (if at all).
        /// TODO - perhaps a method like SetCellTypeAndValue(int, Object) should be introduced to avoid this
        /// </summary>
        /// <returns></returns>
        private bool ConvertCellValueToBoolean()
        {

            switch (cellType)
            {
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    return ((BoolErrRecord)record).BooleanValue;
                case NPOI.SS.UserModel.CellType.STRING:
                    return ((StringRecord)record).String.ToLower() == "true" ? true : false;
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    return ((NumberRecord)record).Value != 0;

                // All other cases Convert to false
                // These choices are not well justified.
                case NPOI.SS.UserModel.CellType.FORMULA:
                    // use cached formula result if it's the right type: 
                    FormulaRecord fr = ((FormulaRecordAggregate)record).FormulaRecord;
                    CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType.BOOLEAN, fr);
                    return fr.CachedBooleanValue;
                // Other cases convert to false 
                // These choices are not well justified. 
                case NPOI.SS.UserModel.CellType.ERROR:
                case NPOI.SS.UserModel.CellType.BLANK:
                    return false;
            }
            throw new Exception("Unexpected cell type (" + cellType + ")");
        }

        /// <summary>
        /// Get the value of the cell as a bool.  For strings, numbers, and errors, we throw an exception.
        /// For blank cells we return a false.
        /// </summary>
        /// <value><c>true</c> if [boolean cell value]; otherwise, <c>false</c>.</value>
        public bool BooleanCellValue
        {
            get
            {
                switch (cellType)
                {
                    case NPOI.SS.UserModel.CellType.BLANK:
                        return false;
                    case NPOI.SS.UserModel.CellType.BOOLEAN:
                        return ((BoolErrRecord)record).BooleanValue;
                    case NPOI.SS.UserModel.CellType.FORMULA:
                        break;
                    default:
                        throw TypeMismatch(NPOI.SS.UserModel.CellType.BOOLEAN, cellType, false);

                }
                FormulaRecord fr = ((FormulaRecordAggregate)record).FormulaRecord;
                CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType.BOOLEAN, fr);
                return fr.CachedBooleanValue;
            }
        }

        /// <summary>
        /// Get the value of the cell as an error code.  For strings, numbers, and bools, we throw an exception.
        /// For blank cells we return a 0.
        /// </summary>
        /// <value>The error cell value.</value>
        public byte ErrorCellValue
        {
            get
            {
                switch (cellType)
                {
                    case NPOI.SS.UserModel.CellType.ERROR:
                        return ((BoolErrRecord)record).ErrorValue;
                    case NPOI.SS.UserModel.CellType.FORMULA:
                        break;
                    default:
                        throw TypeMismatch(NPOI.SS.UserModel.CellType.ERROR, cellType, false);

                }
                FormulaRecord fr = ((FormulaRecordAggregate)record).FormulaRecord;
                CheckFormulaCachedValueType(NPOI.SS.UserModel.CellType.ERROR, fr);
                return (byte)fr.CachedErrorValue;
            }
        }

        /// <summary>
        /// Get the style for the cell.  This is a reference to a cell style contained in the workbook
        /// object.
        /// </summary>
        /// <value>The cell style.</value>
        public NPOI.SS.UserModel.CellStyle CellStyle
        {
            get
            {
                short styleIndex = record.XFIndex;
                ExtendedFormatRecord xf = book.Workbook.GetExFormatAt(styleIndex);
                return new HSSFCellStyle(styleIndex, xf, book);
            }
            set
            {
                // Verify it really does belong to our workbook
                ((HSSFCellStyle)value).VerifyBelongsToWorkbook(book);

                // Change our cell record to use this style
                record.XFIndex = value.Index;
            }
        }

        /// <summary>
        /// used for internationalization, currently -1 for UnChanged, 0 for compressed Unicode or 1 for 16-bit
        /// </summary>
        /// <value>-1, 1 or 0 for UnChanged, compressed or Uncompressed (used only with String type)</value>
        /// @see #ENCODING_UNCHANGED
        /// @see #ENCODING_COMPRESSED_UNICODE
        /// @see #ENCODING_UTF_16
        [Obsolete("deprecated As of 3-Jan-06 POI now automatically handles Unicode without forcing the encoding.")]
        public short Encoding
        {
            get
            {
                return encoding;
            }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Should only be used by HSSFSheet and friends.  Returns the low level CellValueRecordInterface record
        /// </summary>
        /// <value>the cell via the low level api.</value>
        public CellValueRecordInterface CellValueRecord
        {
            get { return record; }
        }

        /// <summary>
        /// Checks the bounds.
        /// </summary>
        /// <param name="cellNum">The cell num.</param>
        /// <exception cref="Exception">if the bounds are exceeded.</exception>
        private void CheckBounds(int cellNum)
        {
            if (cellNum > 255)
            {
                throw new Exception("You cannot have more than 255 columns " +
                          "in a given row (IV).  Because Excel can't handle it");
            }
            else if (cellNum < 0)
            {
                throw new Exception("You cannot reference columns with an index of less then 0.");
            }
        }

        /// <summary>
        /// Sets this cell as the active cell for the worksheet
        /// </summary>
        public void SetAsActiveCell()
        {
            int row = record.Row;
            int col = record.Column;

            this.sheet.Sheet.SetActiveCell(row, col);
        }

        /// <summary>
        /// Returns a string representation of the cell
        /// This method returns a simple representation,
        /// anthing more complex should be in user code, with
        /// knowledge of the semantics of the sheet being Processed.
        /// Formula cells return the formula string,
        /// rather than the formula result.
        /// Dates are Displayed in dd-MMM-yyyy format
        /// Errors are Displayed as #ERR&lt;errIdx&gt;
        /// </summary>
        public override String ToString()
        {
            switch (CellType)
            {
                case NPOI.SS.UserModel.CellType.BLANK:
                    return "";
                case NPOI.SS.UserModel.CellType.BOOLEAN:
                    return BooleanCellValue ? "TRUE" : "FALSE";
                case NPOI.SS.UserModel.CellType.ERROR:
                    return NPOI.HSSF.Record.Formula.Eval.ErrorEval.GetText(((BoolErrRecord)record).ErrorValue);
                case NPOI.SS.UserModel.CellType.FORMULA:
                    return CellFormula;
                case NPOI.SS.UserModel.CellType.NUMERIC:
                    string format = this.CellStyle.GetDataFormatString();
                    NPOI.SS.UserModel.DataFormatter formatter = new NPOI.SS.UserModel.DataFormatter();
                    return formatter.FormatCellValue(this);
                case NPOI.SS.UserModel.CellType.STRING:
                    return StringCellValue;
                default:
                    return "Unknown Cell Type: " + CellType;
            }

        }


        /// <summary>
        /// Returns comment associated with this cell
        /// </summary>
        /// <value>The cell comment associated with this cell.</value>
        public NPOI.SS.UserModel.Comment CellComment
        {
            get
            {
                if (comment == null)
                {
                    comment = FindCellComment(sheet.Sheet, record.Row, record.Column);
                }
                return comment;
            }
            set
            {
                if (value == null)
                {
                    RemoveCellComment();
                    return;
                }

                value.Row = record.Row;
                value.Column = record.Column;
                this.comment = value;
            }
        }

        /// <summary>
        /// Removes the comment for this cell, if
        /// there is one.
        /// </summary>
        /// <remarks>WARNING - some versions of excel will loose
        /// all comments after performing this action!</remarks>
        public void RemoveCellComment()
        {
            HSSFComment comment = FindCellComment(sheet.Sheet, record.Row, record.Column);
            this.comment = null;

            if (comment == null)
            {
                // Nothing to do
                return;
            }

            // Zap the underlying NoteRecord
            IList sheetRecords = sheet.Sheet.Records;
            sheetRecords.Remove(comment.NoteRecord);

            // If we have a TextObjectRecord, is should
            //  be proceeed by:
            // MSODRAWING with container
            // OBJ
            // MSODRAWING with EscherTextboxRecord
            if (comment.TextObjectRecord != null)
            {
                TextObjectRecord txo = comment.TextObjectRecord;
                int txoAt = sheetRecords.IndexOf(txo);

                if (sheetRecords[txoAt - 3] is DrawingRecord &&
                    sheetRecords[txoAt - 2] is ObjRecord &&
                    sheetRecords[txoAt - 1] is DrawingRecord)
                {
                    // Zap these, in reverse order
                    sheetRecords.RemoveAt(txoAt - 1);
                    sheetRecords.RemoveAt(txoAt - 2);
                    sheetRecords.RemoveAt(txoAt - 3);
                }
                else
                {
                    throw new InvalidOperationException("Found the wrong records before the TextObjectRecord, can't Remove comment");
                }

                // Now Remove the text record
                sheetRecords.Remove(txo);
            }
        }

        /// <summary>
        /// Cell comment Finder.
        /// Returns cell comment for the specified sheet, row and column.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>cell comment or 
        /// <c>null</c>
        ///  if not found</returns>
        public static HSSFComment FindCellComment(Sheet sheet, int row, int column)
        {
            HSSFComment comment = null;
            Dictionary<short, TextObjectRecord> noteTxo = new Dictionary<short, TextObjectRecord>(); //map shapeId and TextObjectRecord
            int i=0;
            for (IEnumerator it = sheet.Records.GetEnumerator(); it.MoveNext(); )
            {
                RecordBase rec = (RecordBase)it.Current;
                if (rec is NoteRecord)
                {
                    NoteRecord note = (NoteRecord)rec;
                    if (note.Row == row && note.Column == column)
                    {
                        if (i < noteTxo.Count)
                        {
                            TextObjectRecord txo = (TextObjectRecord)noteTxo[note.ShapeId];
                            comment = new HSSFComment(note, txo);
                            comment.Row = note.Row;
                            comment.Column = note.Column;
                            comment.Author = (note.Author);
                            comment.Visible = (note.Flags == NoteRecord.NOTE_VISIBLE);
                            comment.String = txo.Str;
                            break;
                        }
                    }
                }
                else if (rec is ObjRecord)
                {
                    ObjRecord obj = (ObjRecord)rec;
                    SubRecord sub = (SubRecord)obj.GetSubRecords()[0];
                    if (sub is CommonObjectDataSubRecord)
                    {
                        CommonObjectDataSubRecord cmo = (CommonObjectDataSubRecord)sub;
                        if (cmo.ObjectType == CommonObjectType.COMMENT)
                        {
                            //Find the nearest TextObjectRecord which holds comment's text and map it to its shapeId
                            while (it.MoveNext())
                            {
                                rec = (Record)it.Current;
                                if (rec is TextObjectRecord)
                                {
                                    noteTxo.Add(cmo.ObjectId, (TextObjectRecord)rec);
                                    break;
                                }
                            }

                        }
                    }
                }
            }
            return comment;
        }
        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get
            {
                return record.Column & 0xFFFF;
            }
            set { record.Column = value; }
        }
        /// <summary>
        /// Gets the (zero based) index of the row containing this cell
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get
            {
                return record.Row;
            }
        }
        /// <summary>
        /// Returns hyperlink associated with this cell
        /// </summary>
        /// <value>The hyperlink associated with this cell or null if not found</value>
        public NPOI.SS.UserModel.Hyperlink Hyperlink
        {
            get
            {
                for (IEnumerator it = sheet.Sheet.Records.GetEnumerator(); it.MoveNext(); )
                {
                    RecordBase rec = (RecordBase)it.Current;
                    if (rec is HyperlinkRecord)
                    {
                        HyperlinkRecord link = (HyperlinkRecord)rec;
                        if (link.FirstColumn == record.Column && link.FirstRow == record.Row)
                        {
                            return new HSSFHyperlink(link);
                        }
                    }
                }
                return null;
            }
            set
            {
                value.FirstRow = record.Row;
                value.LastRow = record.Row;
                value.FirstColumn = record.Column;
                value.LastColumn = record.Column;

                switch (value.Type)
                {
                    case NPOI.SS.UserModel.HyperlinkType.EMAIL:
                    case NPOI.SS.UserModel.HyperlinkType.URL:
                        value.Label = ("url");
                        break;
                    case NPOI.SS.UserModel.HyperlinkType.FILE:
                        value.Label = ("file");
                        break;
                    case NPOI.SS.UserModel.HyperlinkType.DOCUMENT:
                        value.Label = ("place");
                        break;
                }

                int eofLoc = sheet.Sheet.FindFirstRecordLocBySid(EOFRecord.sid);
                sheet.Sheet.Records.Insert(eofLoc, ((HSSFHyperlink)value).record);
            }
        }
        /// <summary>
        /// Only valid for formula cells
        /// </summary>
        /// <value>one of (NPOI.SS.UserModel.CellType.NUMERIC,NPOI.SS.UserModel.CellType.STRING, NPOI.SS.UserModel.CellType.BOOLEAN, NPOI.SS.UserModel.CellType.ERROR) depending
        /// on the cached value of the formula</value>
        public NPOI.SS.UserModel.CellType CachedFormulaResultType
        {
            get
            {
                if (this.cellType != NPOI.SS.UserModel.CellType.FORMULA)
                {
                    throw new InvalidOperationException("Only formula cells have cached results");
                }
                return ((FormulaRecordAggregate)record).FormulaRecord.CachedResultType;
            }
        }
    }
}