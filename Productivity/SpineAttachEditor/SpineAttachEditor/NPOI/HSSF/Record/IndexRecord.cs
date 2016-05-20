
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


namespace NPOI.HSSF.Record
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using NPOI.Util;

    /**
     * Title:        Index Record
     * Description:  Occurs right after BOF, tells you where the DBCELL records are for a sheet
     *               Important for locating cells
     * NOT USED IN THIS RELEASE
     * REFERENCE:  PG 323 Microsoft Excel 97 Developer's Kit (ISBN: 1-57231-498-2)
     * @author Andrew C. Oliver (acoliver at apache dot org)
     * @author Jason Height (jheight at chariot dot net dot au)
     * @version 2.0-pre
     */

    public class IndexRecord
       : Record
    {
        public const short sid = 0x20B;
        public static int DBCELL_CAPACITY = 30;
        public int field_1_zero;            // reserved must be 0
        public int field_2_first_row;       // first row on the sheet
        public int field_3_last_row_add1;   // last row
        public int field_4_ibXF;            // DefColWidth 
        public List<int> field_5_dbcells;         // array of offsets to DBCELL records

        public IndexRecord()
        {
        }

        /**
         * Constructs an Index record and Sets its fields appropriately.
         * @param in the RecordInputstream to Read the record from
         */

        public IndexRecord(RecordInputStream in1)
        {
            field_5_dbcells =
                new List<int>(DBCELL_CAPACITY);   // initial capacity of 30
            field_1_zero = in1.ReadInt();
            field_2_first_row = in1.ReadInt();
            field_3_last_row_add1 = in1.ReadInt();
            field_4_ibXF = in1.ReadInt();
            while (in1.Remaining > 0)
            {

                // Console.WriteLine("Getting " + k);
                field_5_dbcells.Add(in1.ReadInt());
            }
        }


        public void AddDbcell(int cell)
        {
            if (field_5_dbcells == null)
            {
                field_5_dbcells = new List<int>();
            }
            field_5_dbcells.Add(cell);
        }

        public void SetDbcell(int cell, int value)
        {
            field_5_dbcells[cell]=value;
        }

        public int FirstRow
        {
            get
            {
                return field_2_first_row;
            }
            set { field_2_first_row = value; }
        }

        public int LastRowAdd1
        {
            get
            {
                return field_3_last_row_add1;
            }
            set { field_3_last_row_add1 = value; }
        }

        public int PosOfDefColWidthRecord
        {
            get
            {
                return field_4_ibXF;
            }
            set
            {
                field_4_ibXF = value;
            }
        }

        public int NumDbcells
        {
            get
            {
                if (field_5_dbcells == null)
                {
                    return 0;
                }
                return field_5_dbcells.Count;
            }
        }

        public int GetDbcellAt(int cellnum)
        {
            return field_5_dbcells[cellnum];
        }

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[INDEX]\n");
            buffer.Append("    .firstrow       = ")
                .Append(StringUtil.ToHexString(FirstRow)).Append("\n");
            buffer.Append("    .lastrowAdd1    = ")
                .Append(StringUtil.ToHexString(LastRowAdd1)).Append("\n");
            for (int k = 0; k < NumDbcells; k++)
            {
                buffer.Append("    .dbcell_" + k + "       = ")
                    .Append(StringUtil.ToHexString(GetDbcellAt(k))).Append("\n");
            }
            buffer.Append("[/INDEX]\n");
            return buffer.ToString();
        }

        public override int Serialize(int offset, byte [] data)
        {
            LittleEndian.PutShort(data, 0 + offset, sid);
            LittleEndian.PutShort(data, 2 + offset,
                                  (short)(16 + (NumDbcells * 4)));
            LittleEndian.PutInt(data, 4 + offset, 0);
            LittleEndian.PutInt(data, 8 + offset, field_2_first_row);
            LittleEndian.PutInt(data, 12 + offset, field_3_last_row_add1);
            LittleEndian.PutInt(data, 16 + offset, field_4_ibXF);
            for (int k = 0; k < NumDbcells; k++)
            {
                LittleEndian.PutInt(data, (k * 4) + 20 + offset, GetDbcellAt(k));
            }
            return RecordSize;
        }

        public override int RecordSize
        {
            get { return 20 + (NumDbcells * 4); }
        }

        /** Returns the size of an INdexRecord when it needs to index the specified number of blocks
          *
          */
        public static int GetRecordSizeForBlockCount(int blockCount)
        {
            return 20 + (4 * blockCount);
        }

        public override short Sid
        {
            get { return sid; }
        }

        public override Object Clone()
        {
            IndexRecord rec = new IndexRecord();
            rec.field_1_zero = field_1_zero;
            rec.field_2_first_row = field_2_first_row;
            rec.field_3_last_row_add1 = field_3_last_row_add1;
            rec.field_4_ibXF = field_4_ibXF;
            rec.field_5_dbcells = new List<int>();
            rec.field_5_dbcells.AddRange(field_5_dbcells);
            return rec;
        }
    }
}