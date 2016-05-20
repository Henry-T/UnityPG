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

    /// <summary>
    /// Class to Read and manipulate the footer.
    /// The footer works by having a left, center, and right side.  The total cannot
    /// be more that 255 bytes long.  One uses this class by Getting the HSSFFooter
    /// from HSSFSheet and then Getting or Setting the left, center, and right side.
    /// For special things (such as page numbers and date), one can use a the methods
    /// that return the Chars used to represent these.  One can also Change the
    /// fonts by using similar methods.
    /// @author Shawn Laubach (slaubach at apache dot org)
    /// </summary>
    public class HSSFFooter : HeaderFooter,NPOI.SS.UserModel.Footer
    {

        FooterRecord footerRecord;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSSFFooter"/> class.
        /// </summary>
        /// <param name="footerRecord">Footer record to Create the footer with</param>
        public HSSFFooter(FooterRecord footerRecord)
            : base(footerRecord.Footer)
        {
            this.footerRecord = footerRecord;
        }

        /// <summary>
        /// Get the left side of the footer.
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
                CreateFooterString();
            }
        }

        /// <summary>
        /// Get the center of the footer.
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
                CreateFooterString();
            }
        }


        /// <summary>
        /// Get the right side of the footer.
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
                CreateFooterString();
            }
        }
        /// <summary>
        /// Gets the raw footer.
        /// </summary>
        /// <value>The raw footer.</value>
        public String RawFooter
        {
            get
            {
                return footerRecord.Footer;
            }
        }
        /// <summary>
        /// Creates the complete footer string based on the left, center, and middle
        /// strings.
        /// </summary>
        private void CreateFooterString()
        {
            footerRecord.Footer = (
                           "&C" + (center == null ? "" : center) +
                           "&L" + (left == null ? "" : left) +
                           "&R" + (right == null ? "" : right));
            footerRecord.FooterLength = ((byte)footerRecord.Footer.Length);
        }
    }
}