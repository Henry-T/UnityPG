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
    using System.Collections;
    using NPOI.HSSF.Model;
    using NPOI.HSSF.Record;
    using NPOI.HSSF.Util;
    /**
     * Groups the page settings records for a worksheet.<p/>
     * 
     * See OOO excelfileformat.pdf sec 4.4 'Page Settings Block'
     * 
     * @author Josh Micich
     */
    public class PageSettingsBlock : RecordAggregate
    {
        // Every one of these component records is optional 
        // (The whole PageSettingsBlock may not be present) 
        private PageBreakRecord _rowBreaksRecord;
        private PageBreakRecord _columnBreaksRecord;
        private HeaderRecord header;
        private FooterRecord footer;
        private HCenterRecord _hCenter;
        private VCenterRecord _vCenter;
        private LeftMarginRecord _leftMargin;
        private RightMarginRecord _rightMargin;
        private TopMarginRecord _topMargin;
        private BottomMarginRecord _bottomMargin;
        private Record _pls;
        private PrintSetupRecord printSetup;
        private Record _bitmap;

        private ArrayList _rowRecords;

        public PageSettingsBlock(RecordStream rs)
        {
            _rowRecords = new ArrayList();
            while (ReadARecord(rs)) ;
        }

        /**
         * Creates a PageSettingsBlock with default settings
         */
        public PageSettingsBlock()
        {
            _rowBreaksRecord = new HorizontalPageBreakRecord();
            _columnBreaksRecord = new VerticalPageBreakRecord();
            _rowRecords = new ArrayList();
            header = CreateHeader();
            footer = CreateFooter();
            _hCenter = CreateHCenter();
            _vCenter = CreateVCenter();
            printSetup = CreatePrintSetup();
        }
        public override void Dispose()
        {
            _rowBreaksRecord = null;
            _columnBreaksRecord = null;
            header = null;
            footer = null;
            _hCenter = null;
            _vCenter = null;
            _leftMargin = null;
            _rightMargin = null;
            _topMargin = null;
            _bottomMargin = null;
            _pls = null;
            printSetup = null;
            _bitmap = null;
        }
        /**
         * @return <c>true</c> if the specified Record sid is one belonging to the 
         * 'Page Settings Block'.
         */
        public static bool IsComponentRecord(int sid)
        {
            switch (sid)
            {
                case HorizontalPageBreakRecord.sid:
                case VerticalPageBreakRecord.sid:
                case HeaderRecord.sid:
                case FooterRecord.sid:
                case HCenterRecord.sid:
                case VCenterRecord.sid:
                case LeftMarginRecord.sid:
                case RightMarginRecord.sid:
                case TopMarginRecord.sid:
                case BottomMarginRecord.sid:
                case UnknownRecord.PLS_004D:
                case PrintSetupRecord.sid:
                case UnknownRecord.BITMAP_00E9:
                    return true;
            }
            return false;
        }

        private bool ReadARecord(RecordStream rs)
        {
            switch (rs.PeekNextSid())
            {
                case HorizontalPageBreakRecord.sid:
                    _rowBreaksRecord = (PageBreakRecord)rs.GetNext();
                    _rowRecords.Add(_rowBreaksRecord);
                    break;
                case VerticalPageBreakRecord.sid:
                    _columnBreaksRecord = (PageBreakRecord)rs.GetNext();
                    _rowRecords.Add(_columnBreaksRecord);
                    break;
                case HeaderRecord.sid:
                    header = (HeaderRecord)rs.GetNext();
                    _rowRecords.Add(header);
                    break;
                case FooterRecord.sid:
                    footer = (FooterRecord)rs.GetNext();
                    _rowRecords.Add(footer);
                    break;
                case HCenterRecord.sid:
                    _hCenter = (HCenterRecord)rs.GetNext();
                    _rowRecords.Add(_hCenter);
                    break;
                case VCenterRecord.sid:
                    _vCenter = (VCenterRecord)rs.GetNext();
                    _rowRecords.Add(_vCenter);
                    break;
                case LeftMarginRecord.sid:
                    _leftMargin = (LeftMarginRecord)rs.GetNext();
                    _rowRecords.Add(_leftMargin);
                    break;
                case RightMarginRecord.sid:
                    _rightMargin = (RightMarginRecord)rs.GetNext();
                    _rowRecords.Add(_rightMargin);
                    break;
                case TopMarginRecord.sid:
                    _topMargin = (TopMarginRecord)rs.GetNext();
                    _rowRecords.Add(_topMargin);
                    break;
                case BottomMarginRecord.sid:
                    _bottomMargin = (BottomMarginRecord)rs.GetNext();
                    _rowRecords.Add(_bottomMargin);
                    break;
                case 0x004D: // PLS
                    _pls = rs.GetNext();
                    _rowRecords.Add(_pls);
                    break;
                case PrintSetupRecord.sid:
                    printSetup = (PrintSetupRecord)rs.GetNext();
                    _rowRecords.Add(printSetup);
                    break;
                case 0x00E9: // BITMAP
                    _bitmap = rs.GetNext();
                    _rowRecords.Add(_bitmap);
                    break;
                default:
                    // all other record types are not part of the PageSettingsBlock
                    return false;
            }
            return true;
        }

        private PageBreakRecord RowBreaksRecord
        {
            get
            {
                if (_rowBreaksRecord == null)
                {
                    _rowBreaksRecord = new HorizontalPageBreakRecord();
                }
                return _rowBreaksRecord;
            }
        }

        private PageBreakRecord ColumnBreaksRecord
        {
            get
            {
                if (_columnBreaksRecord == null)
                {
                    _columnBreaksRecord = new VerticalPageBreakRecord();
                }
                return _columnBreaksRecord;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _rowRecords.GetEnumerator();
        }

        /**
         * Sets a page break at the indicated column
         *
         */
        public void SetColumnBreak(int column, int fromRow, int toRow)
        {
            this.ColumnBreaksRecord.AddBreak(column, fromRow, toRow);
        }

        /**
         * Removes a page break at the indicated column
         *
         */
        public void RemoveColumnBreak(int column)
        {
            this.ColumnBreaksRecord.RemoveBreak(column);
        }

        public override void VisitContainedRecords(RecordVisitor rv)
        {
            VisitIfPresent(_rowBreaksRecord, rv);
            VisitIfPresent(_columnBreaksRecord, rv);
            VisitIfPresent(header, rv);
            VisitIfPresent(footer, rv);
            VisitIfPresent(_hCenter, rv);
            VisitIfPresent(_vCenter, rv);
            VisitIfPresent(_leftMargin, rv);
            VisitIfPresent(_rightMargin, rv);
            VisitIfPresent(_topMargin, rv);
            VisitIfPresent(_bottomMargin, rv);
            VisitIfPresent(_pls, rv);
            VisitIfPresent(printSetup, rv);
            VisitIfPresent(_bitmap, rv);
        }
        private static void VisitIfPresent(Record r, RecordVisitor rv)
        {
            if (r != null)
            {
                rv.VisitRecord(r);
            }
        }

        /**
         * Creates the Header Record and sets it to nothing/0 length
         */
        private static HeaderRecord CreateHeader()
        {
            HeaderRecord retval = new HeaderRecord();

            retval.HeaderLength = ((byte)0);
            retval.Header = (null);
            return retval;
        }

        /**
         * Creates the Footer Record and sets it to nothing/0 length
         */
        private static FooterRecord CreateFooter()
        {
            FooterRecord retval = new FooterRecord();

            retval.FooterLength = ((byte)0);
            retval.Footer = (null);
            return retval;
        }

        /**
         * Creates the HCenter Record and sets it to false (don't horizontally center)
         */
        private static HCenterRecord CreateHCenter()
        {
            HCenterRecord retval = new HCenterRecord();

            retval.HCenter = (false);
            return retval;
        }

        /**
         * Creates the VCenter Record and sets it to false (don't horizontally center)
        */
        private static VCenterRecord CreateVCenter()
        {
            VCenterRecord retval = new VCenterRecord();

            retval.VCenter = (false);
            return retval;
        }

        /**
         * Creates the PrintSetup Record and sets it to defaults and marks it invalid
         * @see org.apache.poi.hssf.record.PrintSetupRecord
         * @see org.apache.poi.hssf.record.Record
         * @return record containing a PrintSetupRecord
         */
        private static PrintSetupRecord CreatePrintSetup()
        {
            PrintSetupRecord retval = new PrintSetupRecord();

            retval.PaperSize = ((short)1);
            retval.Scale = ((short)100);
            retval.PageStart = ((short)1);
            retval.FitWidth = ((short)1);
            retval.FitHeight = ((short)1);
            retval.Options = ((short)2);
            retval.HResolution = ((short)300);
            retval.VResolution = ((short)300);
            retval.HeaderMargin = (0.5);
            retval.FooterMargin = (0.5);
            retval.Copies = ((short)0);
            return retval;
        }


        /**
         * Returns the HeaderRecord.
         * @return HeaderRecord for the sheet.
         */
        public HeaderRecord Header
        {
            get
            {
                return header;
            }
            set 
            {
                header = value;
            }
        }

        /**
         * Returns the FooterRecord.
         * @return FooterRecord for the sheet.
         */
        public FooterRecord Footer
        {
            get
            {
                return footer;
            }
            set { footer = value; }
        }

        /**
         * Returns the PrintSetupRecord.
         * @return PrintSetupRecord for the sheet.
         */
        public PrintSetupRecord PrintSetup
        {
            get
            {
                return printSetup;
            }
            set 
            {
                printSetup = value;
            }
        }


        private Margin GetMarginRec(NPOI.SS.UserModel.MarginType margin)
        {
            switch (margin)
            {
                case NPOI.SS.UserModel.MarginType.LeftMargin: return _leftMargin;
                case NPOI.SS.UserModel.MarginType.RightMargin: return _rightMargin;
                case NPOI.SS.UserModel.MarginType.TopMargin: return _topMargin;
                case NPOI.SS.UserModel.MarginType.BottomMargin: return _bottomMargin;
                default:
                    throw new InvalidOperationException("Unknown margin constant:  " + (short)margin);
            }
        }


        /**
         * Gets the size of the margin in inches.
         * @param margin which margin to Get
         * @return the size of the margin
         */
        public double GetMargin(NPOI.SS.UserModel.MarginType margin)
        {
            Margin m = GetMarginRec(margin);
            if (m != null)
            {
                return m.Margin;
            }
            else
            {
                switch (margin)
                {
                    case NPOI.SS.UserModel.MarginType.LeftMargin:
                        return .75;
                    case NPOI.SS.UserModel.MarginType.RightMargin:
                        return .75;
                    case NPOI.SS.UserModel.MarginType.TopMargin:
                        return 1.0;
                    case NPOI.SS.UserModel.MarginType.BottomMargin:
                        return 1.0;
                }
                throw new InvalidOperationException("Unknown margin constant:  " + margin);
            }
        }

        /**
         * Sets the size of the margin in inches.
         * @param margin which margin to Get
         * @param size the size of the margin
         */
        public void SetMargin(NPOI.SS.UserModel.MarginType margin, double size)
        {
            Margin m = GetMarginRec(margin);
            if (m == null)
            {
                switch (margin)
                {
                    case NPOI.SS.UserModel.MarginType.LeftMargin:
                        _leftMargin = new LeftMarginRecord();
                        m = _leftMargin;
                        break;
                    case NPOI.SS.UserModel.MarginType.RightMargin:
                        _rightMargin = new RightMarginRecord();
                        m = _rightMargin;
                        break;
                    case NPOI.SS.UserModel.MarginType.TopMargin:
                        _topMargin = new TopMarginRecord();
                        m = _topMargin;
                        break;
                    case NPOI.SS.UserModel.MarginType.BottomMargin:
                        _bottomMargin = new BottomMarginRecord();
                        m = _bottomMargin;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown margin constant:  " + margin);
                }
            }
            m.Margin= size;
        }

        /**
         * Shifts all the page breaks in the range "count" number of rows/columns
         * @param breaks The page record to be shifted
         * @param start Starting "main" value to shift breaks
         * @param stop Ending "main" value to shift breaks
         * @param count number of units (rows/columns) to shift by
         */
        private static void ShiftBreaks(PageBreakRecord breaks, int start, int stop, int count) {

		IEnumerator iterator = breaks.GetBreaksEnumerator();
		IList shiftedBreak = new ArrayList();
		while(iterator.MoveNext())
		{
			PageBreakRecord.Break breakItem = (PageBreakRecord.Break)iterator.Current;
			int breakLocation = breakItem.main;
			bool inStart = (breakLocation >= start);
			bool inEnd = (breakLocation <= stop);
			if(inStart && inEnd)
				shiftedBreak.Add(breakItem);
		}

		iterator = shiftedBreak.GetEnumerator();
		while (iterator.MoveNext()) {
			PageBreakRecord.Break breakItem = (PageBreakRecord.Break)iterator.Current;
			breaks.RemoveBreak(breakItem.main);
			breaks.AddBreak((short)(breakItem.main+count), breakItem.subFrom, breakItem.subTo);
		}
	}


        /**
         * Sets a page break at the indicated row
         * @param row
         */
        public void SetRowBreak(int row, short fromCol, short toCol)
        {
            this.RowBreaksRecord.AddBreak((short)row, fromCol, toCol);
        }

        /**
         * Removes a page break at the indicated row
         * @param row
         */
        public void RemoveRowBreak(int row)
        {
            if (this.RowBreaksRecord.GetBreaks().Length < 1)
                throw new ArgumentException("Sheet does not define any row breaks");
            this.RowBreaksRecord.RemoveBreak((short)row);
        }

        /**
         * Queries if the specified row has a page break
         * @param row
         * @return true if the specified row has a page break
         */
        public bool IsRowBroken(int row)
        {
            return this.RowBreaksRecord.GetBreak(row) != null;
        }


        /**
         * Queries if the specified column has a page break
         *
         * @return <c>true</c> if the specified column has a page break
         */
        public bool IsColumnBroken(int column)
        {
            return this.ColumnBreaksRecord.GetBreak(column) != null;
        }

        /**
         * Shifts the horizontal page breaks for the indicated count
         * @param startingRow
         * @param endingRow
         * @param count
         */
        public void ShiftRowBreaks(int startingRow, int endingRow, int count)
        {
            ShiftBreaks(this.RowBreaksRecord, startingRow, endingRow, count);
        }

        /**
         * Shifts the vertical page breaks for the indicated count
         * @param startingCol
         * @param endingCol
         * @param count
         */
        public void ShiftColumnBreaks(short startingCol, short endingCol, short count)
        {
            ShiftBreaks(this.ColumnBreaksRecord, startingCol, endingCol, count);
        }

        /**
         * @return all the horizontal page breaks, never <c>null</c>
         */
        public int[] RowBreaks
        {
            get
            {
                return this.RowBreaksRecord.GetBreaks();
            }
        }

        /**
         * @return the number of row page breaks
         */
        public int NumRowBreaks
        {
            get
            {
                return this.RowBreaksRecord.NumBreaks;
            }
        }

        /**
         * @return all the column page breaks, never <c>null</c>
         */
        public int[] ColumnBreaks
        {
            get
            {
                return this.ColumnBreaksRecord.GetBreaks();
            }
        }

        /**
         * @return the number of column page breaks
         */
        public int NumColumnBreaks
        {
            get
            {
                return this.ColumnBreaksRecord.NumBreaks;
            }
        }

        public VCenterRecord VCenter
        {
            get { return _vCenter; }
        }

        public HCenterRecord HCenter
        {
            get { return _hCenter; }
        }
    }
}