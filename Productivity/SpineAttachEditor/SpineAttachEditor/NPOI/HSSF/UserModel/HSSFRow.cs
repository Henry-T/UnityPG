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
    using System.Collections.Generic;

    using NPOI.HSSF.Record;
    using NPOI.HSSF.Model;
    using NPOI.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// High level representation of a row of a spReadsheet.
    /// Only rows that have cells should be Added to a Sheet.
    /// @author  Andrew C. Oliver (acoliver at apache dot org)
    /// @author Glen Stampoultzis (glens at apache.org)
    /// </summary>
    [Serializable]
    public class HSSFRow : IComparable,Row
    {


        // used for collections
        public const int INITIAL_CAPACITY = 5;

        private int rowNum;
        private SortedDictionary<int, Cell> cells = new SortedDictionary<int, Cell>();
         
        /**
         * reference to low level representation
         */

        private RowRecord row;

        /**
         * reference to containing low level Workbook
         */

        private HSSFWorkbook book;

        /**
         * reference to containing Sheet
         */

        private HSSFSheet sheet;

        // TODO - ditch this constructor
        public HSSFRow()
        {
        }

        /// <summary>
        /// Creates new HSSFRow from scratch. Only HSSFSheet should do this.
        /// </summary>
        /// <param name="book">low-level Workbook object containing the sheet that Contains this row</param>
        /// <param name="sheet">low-level Sheet object that Contains this Row</param>
        /// <param name="rowNum">the row number of this row (0 based)</param>
        ///<see cref="NPOI.HSSF.UserModel.HSSFSheet.CreateRow(int)"/>
        public HSSFRow(HSSFWorkbook book, HSSFSheet sheet, int rowNum)
        {
            this.rowNum = rowNum;
            this.book = book;
            this.sheet = sheet;
            row = new RowRecord(rowNum);

            RowNum=(rowNum);
        }

        /// <summary>
        /// Creates an HSSFRow from a low level RowRecord object.  Only HSSFSheet should do
        /// this.  HSSFSheet uses this when an existing file is Read in.
        /// </summary>
        /// <param name="book">low-level Workbook object containing the sheet that Contains this row</param>
        /// <param name="sheet"> low-level Sheet object that Contains this Row</param>
        /// <param name="record">the low level api object this row should represent</param>
        ///<see cref="NPOI.HSSF.UserModel.HSSFSheet.CreateRow(int)"/>
        public HSSFRow(HSSFWorkbook book, HSSFSheet sheet, RowRecord record)
        {
            this.book = book;
            this.sheet = sheet;
            row = record;

            RowNum=(record.RowNumber);
        }
        /// <summary>
        /// Use this to Create new cells within the row and return it.
        /// The cell that is returned is a CELL_TYPE_BLANK. The type can be Changed
        /// either through calling <c>SetCellValue</c> or <c>SetCellType</c>.
        /// </summary>
        /// <param name="column">the column number this cell represents</param>
        /// <returns>a high level representation of the Created cell.</returns>
        public NPOI.SS.UserModel.Cell CreateCell(int column)
        {
            return this.CreateCell(column, NPOI.SS.UserModel.CellType.BLANK);
        }

        /// <summary>
        /// Use this to create new cells within the row and return it.
        /// The cell that is returned is a CELL_TYPE_BLANK. The type can be changed
        /// either through calling setCellValue or setCellType.
        /// </summary>
        /// <param name="columnIndex">the column number this cell represents</param>
        /// <param name="type">a high level representation of the created cell.</param>
        /// <returns></returns>
        public NPOI.SS.UserModel.Cell CreateCell(int columnIndex, NPOI.SS.UserModel.CellType type)
        {
            Cell cell = new HSSFCell(book, sheet, RowNum, (short)columnIndex, type);

            AddCell(cell);
            sheet.Sheet.AddValueRecord(RowNum, ((HSSFCell)cell).CellValueRecord);
            return cell;
        }
        /// <summary>
        /// Remove the Cell from this row.
        /// </summary>
        /// <param name="cell">The cell to Remove.</param>
        public void RemoveCell(NPOI.SS.UserModel.Cell cell)
        {
            if (cell == null)
            {
                throw new ArgumentException("cell must not be null");
            }
            RemoveCell((HSSFCell)cell, true);
        }
        /// <summary>
        /// Removes the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="alsoRemoveRecords">if set to <c>true</c> [also remove records].</param>
        private void RemoveCell(NPOI.SS.UserModel.Cell cell, bool alsoRemoveRecords)
        {

            int column = cell.ColumnIndex;
            if (column < 0)
            {
                throw new Exception("Negative cell indexes not allowed");
            }
            //if (column >= cells.Count || cell != cells[column])
            if(!cells.ContainsKey(column)|| cell!=cells[column])
            {
                throw new Exception("Specified cell is not from this row");
            }
            cells.Remove(column);

            if (alsoRemoveRecords)
            {
                CellValueRecordInterface cval = ((HSSFCell)cell).CellValueRecord;
                sheet.Sheet.RemoveValueRecord(RowNum, cval);
            }

            if (cell.ColumnIndex + 1 == row.LastCol)
            {
                row.LastCol=FindLastCell(row.LastCol) + 1;
            }
            if (cell.ColumnIndex == row.FirstCol)
            {
                row.FirstCol=FindFirstCell(row.FirstCol);
            }
        }

        /// <summary>
        /// Create a high level Cell object from an existing low level record.  Should
        /// only be called from HSSFSheet or HSSFRow itself.
        /// </summary>
        /// <param name="cell">The low level cell to Create the high level representation from</param>
        /// <returns> the low level record passed in</returns>
        public Cell CreateCellFromRecord(CellValueRecordInterface cell)
        {
            Cell hcell = new HSSFCell(book, sheet, cell);

            AddCell(hcell);
            return hcell;
        }

        public bool IsHidden
        {
            get { return this.ZeroHeight; }
            set { this.ZeroHeight = value; }
        }

        /// <summary>
        /// Removes all the cells from the row, and their
        /// records too.
        /// </summary>
        public void RemoveAllCells()
        {
            int initialLen = cells.Count;
            for (int i = 0; i < initialLen; i++)
            {
                RemoveCell(cells[i], true);
            }
            //cells = new HSSFCell[INITIAL_CAPACITY];
        }

        /// <summary>
        /// Get row number this row represents
        /// </summary>
        /// <value>the row number (0 based)</value>
        public int RowNum
        {
            get
            {
                return rowNum;
            }
            set
            {
                if ((value < 0) || (value > RowRecord.MAX_ROW_NUMBER))
                {
                    throw new ArgumentException("Invalid row number (" + value
                            + ") outside allowable range (0.." + RowRecord.MAX_ROW_NUMBER + ")");
                }
                this.rowNum = value;
                if (row != null)
                {
                    row.RowNumber = (value);   // used only for KEY comparison (HSSFRow)
                }
            }
        }

        /// <summary>
        /// Returns the rows outline level. Increased as you
        /// put it into more Groups (outlines), reduced as
        /// you take it out of them.
        /// </summary>
        /// <value>The outline level.</value>
        public int OutlineLevel
        {
            get { return row.OutlineLevel; }
        }

        /// <summary>
        /// Moves the supplied cell to a new column, which
        /// must not already have a cell there!
        /// </summary>
        /// <param name="cell">The cell to move</param>
        /// <param name="newColumn">The new column number (0 based)</param>
        public void MoveCell(Cell cell, int newColumn)
        {
            // Ensure the destination is free
            //if (cells.Count > newColumn && cells[newColumn] != null)
            if(cells.ContainsKey(newColumn))
            {
                throw new ArgumentException("Asked to move cell to column " + newColumn + " but there's already a cell there");
            }

            // Check it's one of ours
            bool existflag = false;
            foreach (Cell cellinrow in cells.Values)
            {
                if (cellinrow.Equals(cell))
                {
                    existflag = true;
                    break;
                }
            }
            if (!existflag)
            {
                throw new ArgumentException("Asked to move a cell, but it didn't belong to our row");
            }            
            //if (!cells[cell.CellNum].Equals(cell))
            //{
            //    throw new ArgumentException("Asked to move a cell, but it didn't belong to our row");
            //}

            // Move the cell to the new position
            // (Don't Remove the records though)
            RemoveCell(cell, false);
            cell.ColumnIndex=(newColumn);
            AddCell(cell);
        }
        /**
 * Returns the HSSFSheet this row belongs to
 *
 * @return the HSSFSheet that owns this row
 */
        public NPOI.SS.UserModel.Sheet Sheet
        {
            get
            {
                return sheet;
            }
        }
        /// <summary>
        /// used internally to Add a cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private void AddCell(Cell cell)
        {

            int column = cell.ColumnIndex;
            // re-allocate cells array as required.
            //if (column >= cells.Count)
            //{
            //    HSSFCell[] oldCells = cells;
            //    int newSize = oldCells.Length * 2;
            //    if (newSize < column + 1)
            //    {
            //        newSize = column + 1;
            //    }
            //    cells = new HSSFCell[newSize];
            //    Array.Copy(oldCells, 0, cells, 0, oldCells.Length);
            //}
            cells.Add(column,cell);
            
            // fix up firstCol and lastCol indexes
            if (row.FirstCol == -1 || column < row.FirstCol)
            {
                row.FirstCol=(column);
            }

            if (row.LastCol == -1 || column >= row.LastCol)
            {
                row.LastCol=((short)(column + 1)); // +1 -> for one past the last index 
            }
        }

        /// <summary>
        /// Get the hssfcell representing a given column (logical cell)
        /// 0-based. If you ask for a cell that is not defined, then
        /// you Get a null.
        /// This is the basic call, with no policies applied
        /// </summary>
        /// <param name="cellnum">0 based column number</param>
        /// <returns>Cell representing that column or null if Undefined.</returns>
        private NPOI.SS.UserModel.Cell RetrieveCell(int cellnum)
        {
            if (!cells.ContainsKey(cellnum))
                return null;
            //if (cellnum < 0 || cellnum >= cells.Count) return null;
            return cells[cellnum];
        }

        /// <summary>
        /// Get the hssfcell representing a given column (logical cell)
        /// 0-based.  If you ask for a cell that is not defined then
        /// you Get a null, Unless you have Set a different
        /// MissingCellPolicy on the base workbook.
        /// Short method signature provided to retain binary
        /// compatibility.
        /// </summary>
        /// <param name="cellnum">0 based column number</param>
        /// <returns>Cell representing that column or null if Undefined.</returns>
        public NPOI.SS.UserModel.Cell GetCell(short cellnum)
        {
            int ushortCellNum = cellnum & 0x0000FFFF; // avoid sign extension
            return GetCell(ushortCellNum);
        }

        /// <summary>
        /// Get the hssfcell representing a given column (logical cell)
        /// 0-based.  If you ask for a cell that is not defined then
        /// you Get a null, Unless you have Set a different
        /// on the base workbook.
        /// </summary>
        /// <param name="cellnum">0 based column number</param>
        /// <returns>Cell representing that column or null if Undefined.</returns>
        public Cell GetCell(int cellnum)
        {
            return GetCell(cellnum, book.MissingCellPolicy);
        }

        /// <summary>
        /// Get the hssfcell representing a given column (logical cell)
        /// 0-based.  If you ask for a cell that is not defined, then
        /// your supplied policy says what to do
        /// </summary>
        /// <param name="cellnum">0 based column number</param>
        /// <param name="policy">Policy on blank / missing cells</param>
        /// <returns>that column or null if Undefined + policy allows.</returns>
        public Cell GetCell(int cellnum, MissingCellPolicy policy)
        {
            NPOI.SS.UserModel.Cell cell = RetrieveCell(cellnum);
            if (policy == MissingCellPolicy.RETURN_NULL_AND_BLANK)
            {
                return cell;
            }
            if (policy == MissingCellPolicy.RETURN_BLANK_AS_NULL)
            {
                if (cell == null) return cell;
                if (cell.CellType == NPOI.SS.UserModel.CellType.BLANK)
                {
                    return null;
                }
                return cell;
            }
            if (policy == MissingCellPolicy.CREATE_NULL_AS_BLANK)
            {
                if (cell == null)
                {
                    return CreateCell((short)cellnum, NPOI.SS.UserModel.CellType.BLANK);
                }
                return cell;
            }
            throw new ArgumentException("Illegal policy " + policy + " (" + policy.id + ")");
        }

        /// <summary>
        /// Get the number of the first cell contained in this row.
        /// </summary>
        /// <value>the first logical cell in the row, or -1 if the row does not contain any cells.</value>
        public int FirstCellNum
        {
            get
            {
                if (PhysicalNumberOfCells == 0)
                    return -1;
                else
                    return row.FirstCol;
            }
        }

        /// <summary>
        /// Gets the index of the last cell contained in this row PLUS ONE
        /// . The result also happens to be the 1-based column number of the last cell.  This value can be used as a
        /// standard upper bound when iterating over cells:
        /// </summary>
        /// <value>
        /// short representing the last logical cell in the row PLUS ONE, or -1 if the
        /// row does not contain any cells.
        ///</value>
        /// <example>
        /// short minColIx = row.GetFirstCellNum();
        /// short maxColIx = row.GetLastCellNum();
        /// for(short colIx=minColIx; colIx&lt;maxColIx; colIx++) {
        /// Cell cell = row.GetCell(colIx);
        /// if(cell == null) {
        /// continue;
        /// }
        /// //... do something with cell
        /// }
        /// </example>
        public int LastCellNum
        {
            get
            {
                if (PhysicalNumberOfCells == 0)
                {
                    return -1;
                }
                return row.LastCol;
            }
        }


        /// <summary>
        /// Gets the number of defined cells (NOT number of cells in the actual row!).
        /// That is to say if only columns 0,4,5 have values then there would be 3.
        /// </summary>
        /// <value>the number of defined cells in the row.</value>
        public int PhysicalNumberOfCells
        {
            get
            {
                //int count = 0;
                //for (int i = 0; i < cells.Count; i++)
                //{
                //    if (cells[i] != null) count++;
                //}
                //return count;
                return cells.Count;
            }
        }


        /// <summary>
        /// Gets or sets  whether or not to Display this row with 0 height
        /// </summary>
        /// <value>height is zero or not.</value>
        public bool ZeroHeight
        {
            get
            {
                return row.ZeroHeight;
            }
            set 
            {
                row.ZeroHeight=(value);
            }
        }

        /// <summary>
        /// Get or sets the row's height or ff (-1) for Undefined/default-height in twips (1/20th of a point)
        /// </summary>
        /// <value>rowheight or 0xff for Undefined (use sheet default)</value>
        public short Height
        {
            get { return row.Height; }
            set
            {
                if(value==0)
                    this.ZeroHeight = true;
                else
                    this.ZeroHeight = false;
                row.BadFontHeight = true;
                row.Height = value;
            }
        }
        /// <summary>
        /// is this row formatted? Most aren't, but some rows
        /// do have whole-row styles. For those that do, you
        /// can get the formatting from {@link #getRowStyle()}
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is formatted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFormatted
        {
            get
            {
                return row.Formatted;
            }
        }
        /// <summary>
        /// Returns the whole-row cell styles. Most rows won't
        /// have one of these, so will return null. Call IsFormmated to check first
        /// </summary>
        /// <value>The row style.</value>
        public CellStyle RowStyle
        {
            get
            {
                if (!IsFormatted) { return null; }
                short styleIndex = row.XFIndex;
                ExtendedFormatRecord xf = book.Workbook.GetExFormatAt(styleIndex);
                return new HSSFCellStyle(styleIndex, xf, book);
            }
            set 
            {
                row.Formatted=(true);
                row.XFIndex=(value.Index);
            
            }
        }
        /// <summary>
        /// Get the row's height or ff (-1) for Undefined/default-height in points (20*Height)
        /// </summary>
        /// <value>row height or 0xff for Undefined (use sheet default).</value>
        public float HeightInPoints
        {
            get
            {
                return (row.Height / 20);
            }
            set
            {
                if (value == 0)
                    this.ZeroHeight = true;
                else
                    this.ZeroHeight = false;

                row.BadFontHeight = (true);
                row.Height = (short)(value * 20);
            }
        }

        /// <summary>
        /// Get the lowlevel RowRecord represented by this object - should only be called
        /// by other parts of the high level API
        /// </summary>
        /// <value>RowRecord this row represents</value>
        public RowRecord RowRecord
        {
            get
            {
                return row;
            }
        }

        /// <summary>
        /// used internally to refresh the "last cell" when the last cell is Removed.
        /// </summary>
        /// <param name="lastcell">The last cell index</param>
        /// <returns></returns>
        private short FindLastCell(int lastcell)
        {
            short cellnum = (short)(lastcell - 1);
            NPOI.SS.UserModel.Cell r = GetCell(cellnum);

            while (r == null && cellnum >= 0)
            {
                r = GetCell(--cellnum);
            }
            return cellnum;
        }

        /// <summary>
        /// used internally to refresh the "first cell" when the first cell is Removed.
        /// </summary>
        /// <param name="firstcell">The first cell index.</param>
        /// <returns></returns>
        private short FindFirstCell(int firstcell)
        {
            short cellnum = (short)(firstcell + 1);
            NPOI.SS.UserModel.Cell r = GetCell(cellnum);

            while (r == null && cellnum <= LastCellNum)
            {
                r = GetCell(++cellnum);
            }
            if (cellnum > LastCellNum)
                return -1;
            return cellnum;
        }

        /// <summary>
        /// Get cells in the row
        /// </summary>
        public List<NPOI.SS.UserModel.Cell> Cells
        {
            get {
                return new List<NPOI.SS.UserModel.Cell>(this.cells.Values);
            }
        }

        /// <summary>
        /// Gets the cell enumerator of the physically defined cells.
        /// </summary>
        /// <remarks>
        /// Note that the 4th element might well not be cell 4, as the iterator
        /// will not return Un-defined (null) cells.
        /// Call CellNum on the returned cells to know which cell they are.
        /// </remarks>
        public IEnumerator GetCellEnumerator()
        {
            //return //new CellEnumerator(this.cells);
            return this.cells.Values.GetEnumerator();
        }
        ///// <summary>
        ///// Alias for {@link CellEnumerator} to allow
        ///// foreach loops
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerator GetEnumerator()
        //{
        //    return GetCellEnumerator();
        //}

        /**
         * An iterator over the (physical) cells in the row.
         */
        //private class CellEnumerator : IEnumerator
        //{
        //    int thisId = -1;
        //    int nextId = -1;
        //    private HSSFCell[] cells;

        //    public CellEnumerator()
        //    {
        //    }

        //    public CellEnumerator(HSSFCell[] cells)
        //    {
        //        this.cells = cells;
        //    }

        //    public bool MoveNext()
        //    {
                
        //        FindNext();
        //        return nextId < cells.Length;
        //    }

        //    public Object Current
        //    {
        //        get
        //        {
        //            thisId = nextId;
        //            Cell cell = cells[thisId];
        //            return cell;
        //        }
        //    }

        //    public void Remove()
        //    {
        //        if (thisId == -1)
        //            throw new InvalidOperationException("Remove() called before next()");
        //        cells[thisId] = null;
        //    }

        //    private void FindNext()
        //    {
        //        int i = nextId + 1;
        //        for (; i < cells.Length; i++)
        //        {
        //            if (cells[i] != null) break;
        //        }
        //        nextId = i;
        //    }
        //    public void Reset()
        //    {
        //        thisId = -1;
        //        nextId = -1;
        //    }

        //}

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="obj"/>.
        /// Zero
        /// This instance is equal to <paramref name="obj"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        public int CompareTo(Object obj)
        {
            HSSFRow loc = (HSSFRow)obj;

            if (this.RowNum == loc.RowNum)
            {
                return 0;
            }
            if (this.RowNum < loc.RowNum)
            {
                return -1;
            }
            if (this.RowNum > loc.RowNum)
            {
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(Object obj)
        {
            if (!(obj is HSSFRow))
            {
                return false;
            }
            HSSFRow loc = (HSSFRow)obj;

            if (this.RowNum == loc.RowNum)
            {
                return true;
            }
            return false;
        }
    }
}