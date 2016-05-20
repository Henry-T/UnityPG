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

namespace NPOI.HSSF.UserModel
{
    using System;
    using System.Collections;

    using NPOI.Util;
    //using NPOI.HSSF.Model;
    using NPOI.HSSF.Record;
    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.Record.Formula.Eval;
    using NPOI.HSSF.Record.Aggregates;
    using NPOI.SS.Formula;
    using NPOI.SS;
    using NPOI.SS.UserModel;
   
    /**
     * Internal POI use only
     * 
     * @author Josh Micich
     */
    public class HSSFEvaluationWorkbook : FormulaRenderingWorkbook, EvaluationWorkbook, FormulaParsingWorkbook
    {

        private HSSFWorkbook _uBook;
        private NPOI.HSSF.Model.Workbook _iBook;

        public static HSSFEvaluationWorkbook Create(NPOI.SS.UserModel.Workbook book)
        {
            if (book == null)
            {
                return null;
            }
            return new HSSFEvaluationWorkbook((HSSFWorkbook)book);
        }

        private HSSFEvaluationWorkbook(HSSFWorkbook book)
        {
            _uBook = book;
            _iBook = book.Workbook;
        }

        public int GetExternalSheetIndex(String sheetName)
        {
            int sheetIndex = _uBook.GetSheetIndex(sheetName);
            return _iBook.CheckExternSheet(sheetIndex);
        }
        public int GetExternalSheetIndex(String workbookName, String sheetName)
        {
            return _iBook.GetExternalSheetIndex(workbookName, sheetName);
        }

        public NameXPtg GetNameXPtg(String name)
        {
            return _iBook.GetNameXPtg(name);
        }

        public EvaluationName GetName(String name,int sheetIndex)
        {
            for (int i = 0; i < _iBook.NumNames; i++)
            {
                NameRecord nr = _iBook.GetNameRecord(i);
                if (nr.SheetNumber == sheetIndex + 1 && name.Equals(nr.NameText,StringComparison.InvariantCultureIgnoreCase))
                {
                    return new Name(nr, i);
                }
            }
            return sheetIndex == -1 ? null : GetName(name, -1);
        }

        public int GetSheetIndex(EvaluationSheet evalSheet)
        {
            HSSFSheet sheet = ((HSSFEvaluationSheet)evalSheet).HSSFSheet;
            return _uBook.GetSheetIndex(sheet);
        }
        public int GetSheetIndex(String sheetName)
        {
            return _uBook.GetSheetIndex(sheetName);
        }

        public String GetSheetName(int sheetIndex)
        {
            return _uBook.GetSheetName(sheetIndex);
        }

        public EvaluationSheet GetSheet(int sheetIndex)
        {
            return new HSSFEvaluationSheet((HSSFSheet)_uBook.GetSheetAt(sheetIndex));
        }
        public int ConvertFromExternSheetIndex(int externSheetIndex)
        {
            return _iBook.GetSheetIndexFromExternSheetIndex(externSheetIndex);
        }

        public ExternalSheet GetExternalSheet(int externSheetIndex)
        {
            return _iBook.GetExternalSheet(externSheetIndex);
        }

        public String ResolveNameXText(NameXPtg n)
        {
            return _iBook.ResolveNameXText(n.SheetRefIndex, n.NameIndex);
        }

        public String GetSheetNameByExternSheet(int externSheetIndex)
        {
            return _iBook.FindSheetNameFromExternSheet(externSheetIndex);
        }
        public String GetNameText(NamePtg namePtg)
        {
            return _iBook.GetNameRecord(namePtg.Index).NameText;
        }
        public EvaluationName GetName(NamePtg namePtg)
        {
            int ix = namePtg.Index;
            return new Name(_iBook.GetNameRecord(ix), ix);
        }
        public Ptg[] GetFormulaTokens(EvaluationCell evalCell)
        {
            Cell cell = ((HSSFEvaluationCell)evalCell).HSSFCell;
            //if (false)
            //{
            //    // re-parsing the formula text also works, but is a waste of time
            //    // It is useful from time to time to run all unit tests with this code
            //    // to make sure that all formulas POI can evaluate can also be parsed.
            //    return FormulaParser.Parse(cell.CellFormula, _uBook, FormulaType.CELL, _uBook.GetSheetIndex(cell.Sheet));
            //}
            FormulaRecordAggregate fr = (FormulaRecordAggregate)((HSSFCell)cell).CellValueRecord;
            return fr.FormulaTokens;
        }

        private class Name : EvaluationName
        {

            private NameRecord _nameRecord;
            private int _index;

            public Name(NameRecord nameRecord, int index)
            {
                _nameRecord = nameRecord;
                _index = index;
            }
            public Ptg[] NameDefinition
            {
                get{
                    return _nameRecord.NameDefinition;
                }
            }
            public String NameText
            {
                get{
                    return _nameRecord.NameText;
                }
            }
            public bool HasFormula
            {
                get{
                    return _nameRecord.HasFormula;
                }
            }
            public bool IsFunctionName
            {
                get{
                    return _nameRecord.IsFunctionName;
                }
            }
            public bool IsRange
            {
                get
                {
                    return _nameRecord.HasFormula; // TODO - is this right?
                }
            }
            public NamePtg CreatePtg()
            {
                return new NamePtg(_index);
            }
        }

        public SpreadsheetVersion GetSpreadsheetVersion()
        {
            return SpreadsheetVersion.EXCEL97;
        }
    }
}