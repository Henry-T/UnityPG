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
    using NPOI.HSSF.Record;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Class to Read and manipulate the header.
    /// The header works by having a left, center, and right side.  The total cannot
    /// be more that 255 bytes long.  One uses this class by Getting the HSSFHeader
    /// from HSSFSheet and then Getting or Setting the left, center, and right side.
    /// For special things (such as page numbers and date), one can use a the methods
    /// that return the Chars used to represent these.  One can also Change the
    /// fonts by using similar methods.
    /// @author Shawn Laubach (slaubach at apache dot org)
    /// </summary>
    public class HSSFHeader : HeaderFooter,Header
    {

        HeaderRecord headerRecord;

        /// <summary>
        /// Creates a new header interface from a header record
        /// </summary>
        /// <param name="headerRecord">The Header record to Create the header with</param>
        public HSSFHeader(HeaderRecord headerRecord)
            : base(headerRecord.Header)
        {
            this.headerRecord = headerRecord;

        }

        /// <summary>
        /// Get the left side of the header.
        /// </summary>
        /// <value>The string representing the left side.</value>
        public override String Left
        {
            get
            {
                if (stripFields)
                    return StripFields(left);
                return left;
            }
            set
            {
                left = value;
                CreateHeaderString();
            }
        }

        /// <summary>
        /// Get the center of the header.
        /// </summary>
        /// <value>The string representing the center.</value>
        public override String Center
        {
            get
            {
                if (stripFields)
                    return StripFields(center);
                return center;
            }
            set
            {
                center = value;
                CreateHeaderString();
            }
        }


        /// <summary>
        /// Get the right side of the header.
        /// </summary>
        /// <value>The string representing the right side.</value>
        public override String Right
        {
            get
            {
                if (stripFields)
                    return StripFields(right);
                return right;
            }
            set
            {
                right = value;
                CreateHeaderString();
            }
        }
        /// <summary>
        /// Gets the raw header.
        /// </summary>
        /// <value>The raw header.</value>
        public String RawHeader
        {
            get
            {
                return headerRecord.Header;
            }
        }

        /// <summary>
        /// Creates the complete header string based on the left, center, and middle
        /// strings.
        /// </summary>
        private void CreateHeaderString()
        {
            headerRecord.Header = ("&C" + (center == null ? "" : center) +
                    "&L" + (left == null ? "" : left) +
                    "&R" + (right == null ? "" : right));
            headerRecord.HeaderLength = ((byte)headerRecord.Header.Length);
        }
    }
}