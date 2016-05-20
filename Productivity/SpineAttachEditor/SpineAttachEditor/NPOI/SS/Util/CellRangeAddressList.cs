﻿using System;
using System.Collections;
using System.Text;

namespace NPOI.SS.Util
{
    using NPOI.Util;
    using NPOI.Util.IO;
    using NPOI.HSSF.Record;

    public class CellRangeAddressList
    {
        /**
 * List of <tt>CellRangeAddress</tt>es. Each structure represents a cell range
 */
        private ArrayList _list;

        public CellRangeAddressList()
        {
            _list = new ArrayList();
        }
        /**
         * Convenience constructor for creating a <tt>CellRangeAddressList</tt> with a single 
         * <tt>CellRangeAddress</tt>.  Other <tt>CellRangeAddress</tt>es may be Added later.
         */
        public CellRangeAddressList(int firstRow, int lastRow, int firstCol, int lastCol)
            : this()
        {

            AddCellRangeAddress(firstRow, firstCol, lastRow, lastCol);
        }

        /**
         * @param in the RecordInputstream to read the record from
         */
        public CellRangeAddressList(RecordInputStream in1)
        {
            int nItems = in1.ReadUShort();
            _list = new ArrayList(nItems);

            for (int k = 0; k < nItems; k++)
            {
                _list.Add(new CellRangeAddress(in1));
            }
        }

        /**
         * Get the number of following ADDR structures. The number of this
         * structures is automatically set when reading an Excel file and/or
         * increased when you manually Add a new ADDR structure . This is the reason
         * there isn't a set method for this field .
         * 
         * @return number of ADDR structures
         */
        public int CountRanges()
        {
            return _list.Count;
        }

        /**
         * Add a cell range structure.
         * 
         * @param firstRow - the upper left hand corner's row
         * @param firstCol - the upper left hand corner's col
         * @param lastRow - the lower right hand corner's row
         * @param lastCol - the lower right hand corner's col
         * @return the index of this ADDR structure
         */
        public void AddCellRangeAddress(int firstRow, int firstCol, int lastRow, int lastCol)
        {
            CellRangeAddress region = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            AddCellRangeAddress(region);
        }
        public void AddCellRangeAddress(CellRangeAddress cra)
        {
            _list.Add(cra);
        }
        public CellRangeAddress Remove(int rangeIndex)
        {
            if (_list.Count == 0)
            {
                throw new Exception("List is empty");
            }
            if (rangeIndex < 0 || rangeIndex >= _list.Count)
            {
                throw new Exception("Range index (" + rangeIndex
                        + ") is outside allowable range (0.." + (_list.Count - 1) + ")");
            }
            CellRangeAddress cra = (CellRangeAddress)_list[rangeIndex];
            _list.Remove(rangeIndex);
            return cra;
        }

        /**
         * @return <tt>CellRangeAddress</tt> at the given index
         */
        public CellRangeAddress GetCellRangeAddress(int index)
        {
            return (CellRangeAddress)_list[index];
        }
        public int Serialize(int offset, byte[] data)
        {
            int totalSize = this.Size;
            Serialize(new LittleEndianByteArrayOutputStream(data, offset, totalSize));
            return totalSize;
        }
        public void Serialize(LittleEndianOutput out1)
        {
            int nItems = _list.Count;
            out1.WriteShort(nItems);
            for (int k = 0; k < nItems; k++)
            {
                CellRangeAddress region = (CellRangeAddress)_list[k];
                region.Serialize(out1);
            }
        }

        public int Size
        {
            get
            {
                return GetEncodedSize(_list.Count);
            }
        }
        /**
         * @return the total size of for the specified number of ranges,
         *  including the initial 2 byte range count
         */
        public static int GetEncodedSize(int numberOfRanges)
        {
            return 2 + CellRangeAddress.GetEncodedSize(numberOfRanges);
        }
        public CellRangeAddressList Copy()
        {
            CellRangeAddressList result = new CellRangeAddressList();

            int nItems = _list.Count;
            for (int k = 0; k < nItems; k++)
            {
                CellRangeAddress region = (CellRangeAddress)_list[k];
                result.AddCellRangeAddress(region.Copy());
            }
            return result;
        }
        public CellRangeAddress[] CellRangeAddresses
        {
            get
            {
                CellRangeAddress[] result =
                    (CellRangeAddress[])_list.ToArray(typeof(CellRangeAddress));
                return result;
            }
        }
    }
}