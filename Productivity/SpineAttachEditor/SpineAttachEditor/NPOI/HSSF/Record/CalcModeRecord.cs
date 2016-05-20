
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
    using System.IO;
    using System.Text;
    using NPOI.Util;


    /**
     * Title:        Calc Mode Record
     * Description:  Tells the gui whether to calculate formulas
     *               automatically, manually or automatically
     *               except for tables.
     * REFERENCE:  PG 292 Microsoft Excel 97 Developer's Kit (ISBN: 1-57231-498-2)
     * @author Andrew C. Oliver (acoliver at apache dot org)
     * @author Jason Height (jheight at chariot dot net dot au)
     * @version 2.0-pre
     * @see org.apache.poi.hssf.record.CalcCountRecord
     */

    public class CalcModeRecord
       : Record
    {
        public const short sid = 0xD;

        /**
         * manually calculate formulas (0)
         */

        public static short MANUAL = 0;

        /**
         * automatically calculate formulas (1)
         */

        public static short AUTOMATIC = 1;

        /**
         * automatically calculate formulas except for tables (-1)
         */

        public static short AUTOMATIC_EXCEPT_TABLES = -1;
        private short field_1_calcmode;

        public CalcModeRecord()
        {
        }

        /**
         * Constructs a CalcModeRecord and Sets its fields appropriately
         * @param in the RecordInputstream to Read the record from
         */

        public CalcModeRecord(RecordInputStream in1)
        {
            field_1_calcmode = in1.ReadShort();           
        }


        /**
         * Set the calc mode flag for formulas
         *
         * @see #MANUAL
         * @see #AUTOMATIC
         * @see #AUTOMATIC_EXCEPT_TABLES
         *
         * @param calcmode one of the three flags above
         */

        public void SetCalcMode(short calcmode)
        {
            field_1_calcmode = calcmode;
        }

        /**
         * Get the calc mode flag for formulas
         *
         * @see #MANUAL
         * @see #AUTOMATIC
         * @see #AUTOMATIC_EXCEPT_TABLES
         *
         * @return calcmode one of the three flags above
         */

        public short GetCalcMode()
        {
            return field_1_calcmode;
        }

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[CALCMODE]\n");
            buffer.Append("    .Calcmode       = ")
                .Append(StringUtil.ToHexString(GetCalcMode())).Append("\n");
            buffer.Append("[/CALCMODE]\n");
            return buffer.ToString();
        }

        public override int Serialize(int offset, byte [] data)
        {
            LittleEndian.PutShort(data, 0 + offset, sid);
            LittleEndian.PutShort(data, 2 + offset, (short)0x2);
            LittleEndian.PutShort(data, 4 + offset, GetCalcMode());
            return RecordSize;
        }

        public override int RecordSize
        {
            get { return 6; }
        }

        public override short Sid
        {
            get { return sid; }
        }

        public override Object Clone()
        {
            CalcModeRecord rec = new CalcModeRecord();
            rec.field_1_calcmode = field_1_calcmode;
            return rec;
        }
    }
}