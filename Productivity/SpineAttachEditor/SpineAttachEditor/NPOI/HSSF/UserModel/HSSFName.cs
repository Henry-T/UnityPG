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
    using System.IO;
    using System.Collections;
    using System.Text;

    using NPOI.HSSF.Record;
    using NPOI.Util;
    using NPOI.HSSF.Model;
    using NPOI.HSSF.Util;
    using NPOI.HSSF.Record.Formula;


    /// <summary>
    /// High Level Represantion of Named Range
    /// </summary>
    /// <remarks>@author Libin Roman (Vista Portal LDT. Developer)</remarks>
    public class HSSFName:NPOI.SS.UserModel.Name,IDisposable
    {
        private HSSFWorkbook book;
        private NameRecord _definedNameRec;

        /// <summary>
        /// Creates new HSSFName   - called by HSSFWorkbook to Create a sheet from
        /// scratch.
        /// </summary>
        /// <param name="book">lowlevel Workbook object associated with the sheet.</param>
        /// <param name="name">the Name Record</param>
        public HSSFName(HSSFWorkbook book, NameRecord name)
        {
            this.book = book;
            this._definedNameRec = name;
        }
        public void Dispose()
        {
            //this.book = null;
            //this._definedNameRec = null;
        }

        /// <summary>
        /// Gets or sets the sheets name which this named range is referenced to
        /// </summary>
        /// <value>sheet name, which this named range refered to</value>
        public String SheetName
        {
            get
            {
                String result;
                int indexToExternSheet = _definedNameRec.ExternSheetNumber;

                result = book.Workbook.FindSheetNameFromExternSheet(indexToExternSheet);

                return result;
            }
            //set
            //{
            //    int sheetNumber = book.GetSheetIndex(value);

            //    int externSheetNumber = book.GetExternalSheetIndex(sheetNumber);
            //    name.ExternSheetNumber = externSheetNumber;
            //}
        }

        /// <summary>
        /// Gets or sets the name of the named range
        /// </summary>
        /// <value>named range name</value>
        public String NameName
        {
            get
            {
                String result = _definedNameRec.NameText;

                return result;
            }
            set
            {
                _definedNameRec.NameText = value;
                Workbook wb = book.Workbook;

                //Check to Ensure no other names have the same case-insensitive name
                for (int i = wb.NumNames- 1; i >= 0; i--)
                {
                    NameRecord rec = wb.GetNameRecord(i);
                    if (rec != _definedNameRec)
                    {
                        if (rec.NameText.Equals(NameName,StringComparison.InvariantCultureIgnoreCase))
                            throw new ArgumentException("The workbook already Contains this name (case-insensitive)");
                    }
                }
            }
        }

        public String RefersToFormula
        {
            get
            {
                if (_definedNameRec.IsFunctionName)
                {
                    throw new InvalidOperationException("Only applicable to named ranges");
                }
                Ptg[] ptgs = _definedNameRec.NameDefinition;
                if (ptgs.Length < 1)
                {
                    // 'refersToFormula' has not been set yet
                    return null;
                }
                return HSSFFormulaParser.ToFormulaString(book, ptgs);
            }
            set
            {
                Ptg[] ptgs = HSSFFormulaParser.Parse(value, book, NPOI.SS.Formula.FormulaType.NAMEDRANGE, SheetIndex);
                _definedNameRec.NameDefinition = ptgs;
            }
        }
        /**
         * Returns the sheet index this name applies to.
         *
         * @return the sheet index this name applies to, -1 if this name applies to the entire workbook
         */
        public int SheetIndex
        {
            get
            {
                return _definedNameRec.SheetNumber - 1;
            }
            set 
            {
                int lastSheetIx = book.NumberOfSheets - 1;
                if (value < -1 || value > lastSheetIx)
                {
                    throw new ArgumentException("Sheet index (" + value + ") is out of range" +
                            (lastSheetIx == -1 ? "" : (" (0.." + lastSheetIx + ")")));
                }

                _definedNameRec.SheetNumber = (value + 1);
            }
        }
        public string Comment
        {
            get { return _definedNameRec.DescriptionText; }
            set { _definedNameRec.DescriptionText = value; }
        }

        /// <summary>
        /// Tests if this name points to a cell that no longer exists
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the name refers to a deleted cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted
        {
            get
            {
                Ptg[] ptgs = _definedNameRec.NameDefinition;
                return Ptg.DoesFormulaReferToDeletedCell(ptgs);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is function name.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is function name; otherwise, <c>false</c>.
        /// </value>
        public bool IsFunctionName
        {
            get
            {
                return _definedNameRec.IsFunctionName;
            }
            set 
            {
                _definedNameRec.IsFunctionName = value;
            }
        }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append(GetType().Name).Append(" [");
            sb.Append(_definedNameRec.NameText);
            sb.Append("]");
            return sb.ToString();
        }
    }
}