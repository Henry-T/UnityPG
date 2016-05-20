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


namespace NPOI.HSSF.Record.Aggregates
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using NPOI.HSSF.Record;
    using NPOI.SS.Util;

    /// <summary>
    /// Manages various auxiliary records while constructing a RowRecordsAggregate
    /// @author Josh Micich
    /// </summary>
    [Serializable]
    public class SharedValueManager : IDisposable
    {

        private class SharedFormulaGroup
        {
            private SharedFormulaRecord _sfr;
            private FormulaRecordAggregate[] _frAggs;
            private int _numberOfFormulas;
            /**
             * Coordinates of the first cell having a formula that uses this shared formula.
             * This is often <i>but not always</i> the top left cell in the range covered by
             * {@link #_sfr}
             */
            private CellReference _firstCell;

            public SharedFormulaGroup(SharedFormulaRecord sfr, CellReference firstCell)
            {
                if (!sfr.IsInRange(firstCell.Row, firstCell.Col))
                {
                    throw new ArgumentException("First formula cell " + firstCell.FormatAsString()
                            + " is not shared formula range " + sfr.Range.ToString() + ".");
                }
                _sfr = sfr;
                _firstCell = firstCell;
                int width = sfr.LastColumn - sfr.FirstColumn + 1;
                int height = sfr.LastRow - sfr.FirstRow + 1;
                _frAggs = new FormulaRecordAggregate[width * height];
                _numberOfFormulas = 0;
            }

            public void Add(FormulaRecordAggregate agg)
            {
                if (_numberOfFormulas == 0)
                {
                    if (_firstCell.Row != agg.Row || _firstCell.Col != agg.Column)
                    {
                        throw new InvalidOperationException("shared formula coding error");
                    }
                }
                if (_numberOfFormulas >= _frAggs.Length)
                {
                    throw new Exception("Too many formula records for shared formula group");
                }
                _frAggs[_numberOfFormulas++] = agg;
            }

            public void UnlinkSharedFormulas()
            {
                for (int i = 0; i < _numberOfFormulas; i++)
                {
                    _frAggs[i].UnlinkSharedFormula();
                }
            }

            public SharedFormulaRecord SFR
            {
                get
                {
                    return _sfr;
                }
            }

            public override String ToString()
            {
                StringBuilder sb = new StringBuilder(64);
                sb.Append(GetType().Name).Append(" [");
                sb.Append(_sfr.Range.ToString());
                sb.Append("]");
                return sb.ToString();
            }

            /**
             * Note - the 'first cell' of a shared formula group is not always the top-left cell
             * of the enclosing range.
             * @return <code>true</code> if the specified coordinates correspond to the 'first cell'
             * of this shared formula group.
             */
            public bool IsFirstCell(int row, int column)
            {
                return _firstCell.Row == row && _firstCell.Col == column;
            }
        }

        public static SharedValueManager EMPTY = new SharedValueManager(
                new SharedFormulaRecord[0], new CellReference[0], new ArrayRecord[0], new TableRecord[0]);
        private ArrayRecord[] _arrayRecords;
        private TableRecord[] _tableRecords;
        private Dictionary<SharedFormulaRecord, SharedFormulaGroup> _groupsBySharedFormulaRecord;
        /** cached for optimization purposes */
        private SharedFormulaGroup[] _groups;

        private SharedValueManager(SharedFormulaRecord[] sharedFormulaRecords,
                CellReference[] firstCells, ArrayRecord[] arrayRecords, TableRecord[] tableRecords)
        {
            int nShF = sharedFormulaRecords.Length;
            if (nShF != firstCells.Length)
            {
                throw new ArgumentException("array sizes don't match: " + nShF + "!=" + firstCells.Length + ".");
            }
            _arrayRecords = arrayRecords;
            _tableRecords = tableRecords;
            Dictionary<SharedFormulaRecord, SharedFormulaGroup> m = new Dictionary<SharedFormulaRecord, SharedFormulaGroup>(nShF * 3 / 2);
            for (int i = 0; i < nShF; i++)
            {
                SharedFormulaRecord sfr = sharedFormulaRecords[i];
                m[sfr] = new SharedFormulaGroup(sfr, firstCells[i]);
            }
            _groupsBySharedFormulaRecord = m;
        }

        /**
         * @param firstCells
         * @param recs list of sheet records (possibly Contains records for other parts of the Excel file)
         * @param startIx index of first row/cell record for current sheet
         * @param endIx one past index of last row/cell record for current sheet.  It is important
         * that this code does not inadvertently collect <tt>SharedFormulaRecord</tt>s from any other
         * sheet (which could happen if endIx is chosen poorly).  (see bug 44449)
         */
        public static SharedValueManager Create(SharedFormulaRecord[] sharedFormulaRecords,
                CellReference[] firstCells, ArrayRecord[] arrayRecords, TableRecord[] tableRecords)
        {
            if (sharedFormulaRecords.Length + firstCells.Length + arrayRecords.Length + tableRecords.Length < 1)
            {
                return EMPTY;
            }
            return new SharedValueManager(sharedFormulaRecords, firstCells, arrayRecords, tableRecords);
        }


        /**
         * @param firstCell as extracted from the {@link ExpPtg} from the cell's formula.
         * @return never <code>null</code>
         */
        public SharedFormulaRecord LinkSharedFormulaRecord(CellReference firstCell, FormulaRecordAggregate agg)
        {

            SharedFormulaGroup result = FindFormulaGroup(GetGroups(), firstCell);
            result.Add(agg);
            return result.SFR;
        }

        private static SharedFormulaGroup FindFormulaGroup(SharedFormulaGroup[] groups, CellReference firstCell)
        {
            int row = firstCell.Row;
            int column = firstCell.Col;
            // Traverse the list of shared formulas and try to find the correct one for us

            // perhaps this could be optimised to some kind of binary search
            for (int i = 0; i < groups.Length; i++)
            {
                SharedFormulaGroup svg = groups[i];
                if (svg.IsFirstCell(row, column))
                {
                    return svg;
                }
            }
            // TODO - fix file "15228.xls" so it opens in Excel after rewriting with POI
            throw new Exception("Failed to find a matching shared formula record");
        }

        private SharedFormulaGroup[] GetGroups()
        {
            if (_groups == null)
            {
                SharedFormulaGroup[] groups = new SharedFormulaGroup[_groupsBySharedFormulaRecord.Count];
                _groupsBySharedFormulaRecord.Values.CopyTo(groups, 0);
                Array.Sort(groups, SVGComparator); // make search behaviour more deterministic
                _groups = groups;
            }
            return _groups;
        }

        private SharedFormulaGroupComparator SVGComparator = new SharedFormulaGroupComparator();
        private class SharedFormulaGroupComparator : Comparer<SharedFormulaGroup>
        {
            public override int Compare(SharedFormulaGroup a, SharedFormulaGroup b)
            {
                CellRangeAddress8Bit rangeA = a.SFR.Range;
                CellRangeAddress8Bit rangeB = b.SFR.Range;

                int cmp;
                cmp = rangeA.FirstRow - rangeB.FirstRow;
                if (cmp != 0)
                {
                    return cmp;
                }
                cmp = rangeA.FirstColumn - rangeB.FirstColumn;
                if (cmp != 0)
                {
                    return cmp;
                }
                return 0;
            }
        }

        /**
         * Gets the {@link SharedValueRecordBase} record if it should be encoded immediately after the
         * formula record Contained in the specified {@link FormulaRecordAggregate} agg.  Note - the
         * shared value record always appears after the first formula record in the group.  For arrays
         * and tables the first formula is always the in the top left cell.  However, since shared
         * formula groups can be sparse and/or overlap, the first formula may not actually be in the
         * top left cell.
         *
         * @return the SHRFMLA, TABLE or ARRAY record for the formula cell, if it is the first cell of
         * a table or array region. <code>null</code> if the formula cell is not shared/array/table,
         * or if the specified formula is not the the first in the group.
         */
        public SharedValueRecordBase GetRecordForFirstCell(FormulaRecordAggregate agg)
        {
            CellReference firstCell = agg.FormulaRecord.Formula.ExpReference;
            // perhaps this could be optimised by consulting the (somewhat unreliable) isShared flag
            // and/or distinguishing between tExp and tTbl.
            if (firstCell == null)
            {
                // not a shared/array/table formula
                return null;
            }


            int row = firstCell.Row;
            int column = firstCell.Col;
            if (agg.Row != row || agg.Column != column)
            {
                // not the first formula cell in the group
                return null;
            }
            SharedFormulaGroup[] groups = GetGroups();
            for (int i = 0; i < groups.Length; i++)
            {
                // note - logic for Finding correct shared formula group is slightly
                // more complicated since the first cell
                SharedFormulaGroup sfg = groups[i];
                if (sfg.IsFirstCell(row, column))
                {
                    return sfg.SFR;
                }
            }

            // Since arrays and tables cannot be sparse (all cells in range participate)
            // The first cell will be the top left in the range.  So we can match the
            // ARRAY/TABLE record directly.

            for (int i = 0; i < _tableRecords.Length; i++)
            {
                TableRecord tr = _tableRecords[i];
                if (tr.IsFirstCell(row, column))
                {
                    return tr;
                }
            }
            for (int i = 0; i < _arrayRecords.Length; i++)
            {
                ArrayRecord ar = _arrayRecords[i];
                if (ar.IsFirstCell(row, column))
                {
                    return ar;
                }
            }
            return null;
        }

        /**
         * Converts all {@link FormulaRecord}s handled by <tt>sharedFormulaRecord</tt>
         * to plain unshared formulas
         */
        public void Unlink(SharedFormulaRecord sharedFormulaRecord)
        {
            SharedFormulaGroup svg = _groupsBySharedFormulaRecord[sharedFormulaRecord];
            _groupsBySharedFormulaRecord.Remove(sharedFormulaRecord);
            _groups = null; // be sure to reset cached value
            if (svg == null)
            {
                throw new InvalidOperationException("Failed to find formulas for shared formula");
            }
            svg.UnlinkSharedFormulas();
        }

        public void Dispose()
        {
            _arrayRecords = null;
            _tableRecords = null;
            //_sfrs = null;
        }
    }
}