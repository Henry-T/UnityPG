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

namespace NPOI.HSSF.Model
{
    using System;
    using System.Collections;
    using NPOI.DDF;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Record;

    /// <summary>
    /// An abstract shape Is the lowlevel model for a shape.
    /// @author Glen Stampoultzis (glens at apache.org)
    /// </summary>
    public abstract class AbstractShape
    {
        /// <summary>
        /// Create a new shape object used to Create the escher records.
        /// </summary>
        /// <param name="hssfShape">The simple shape this Is based on.</param>
        /// <param name="shapeId">The shape id.</param>
        /// <returns></returns>
        public static AbstractShape CreateShape(HSSFShape hssfShape, int shapeId)
        {
            AbstractShape shape;
            if (hssfShape is HSSFComment)
            {
                shape = new CommentShape((HSSFComment)hssfShape, shapeId);
            }
            else if (hssfShape is HSSFTextbox)
            {
                shape = new TextboxShape((HSSFTextbox)hssfShape, shapeId);
            }
            else if (hssfShape is HSSFPolygon)
            {
                shape = new PolygonShape((HSSFPolygon)hssfShape, shapeId);
            }
            else if (hssfShape is HSSFSimpleShape)
            {
                HSSFSimpleShape simpleShape = (HSSFSimpleShape)hssfShape;
                switch (simpleShape.ShapeType)
                {
                    case HSSFSimpleShape.OBJECT_TYPE_PICTURE:
                        shape = new PictureShape(simpleShape, shapeId);
                        break;
                    case HSSFSimpleShape.OBJECT_TYPE_LINE:
                        shape = new LineShape(simpleShape, shapeId);
                        break;
                    case HSSFSimpleShape.OBJECT_TYPE_OVAL:
                    case HSSFSimpleShape.OBJECT_TYPE_RECTANGLE:
                        shape = new SimpleFilledShape(simpleShape, shapeId);
                        break;
                    default:
                        throw new ArgumentException("Do not know how to handle this type of shape");
                }
            }
            else
            {
                throw new ArgumentException("Unknown shape type");
            }
            EscherSpRecord sp = shape.SpContainer.GetChildById(EscherSpRecord.RECORD_ID);
            if (hssfShape.Parent!= null)
                sp.Flags=sp.Flags | EscherSpRecord.FLAG_CHILD;
            return shape;
        }

        protected AbstractShape()
        {
        }

        /// <summary>
        /// The shape container and it's children that can represent this
        /// shape.
        /// </summary>
        /// <value>The sp container.</value>
        public abstract EscherContainerRecord SpContainer { get; }

        /// <summary>
        /// The object record that Is associated with this shape.
        /// </summary>
        /// <value>The obj record.</value>
        public abstract ObjRecord ObjRecord { get; }

        /// <summary>
        /// Creates an escher anchor record from a HSSFAnchor.
        /// </summary>
        /// <param name="userAnchor">The high level anchor to Convert.</param>
        /// <returns>An escher anchor record.</returns>
        protected virtual EscherRecord CreateAnchor(HSSFAnchor userAnchor)
        {
            return ConvertAnchor.CreateAnchor(userAnchor);
        }

        /// <summary>
        /// Add standard properties to the opt record.  These properties effect
        /// all records.
        /// </summary>
        /// <param name="shape">The user model shape.</param>
        /// <param name="opt">The opt record to Add the properties to.</param>
        /// <returns>The number of options Added.</returns>
        protected virtual int AddStandardOptions(HSSFShape shape, EscherOptRecord opt)
        {
            opt.AddEscherProperty(new EscherBoolProperty(EscherProperties.TEXT__SIZE_TEXT_TO_FIT_SHAPE, 0x080000));
            //        opt.AddEscherProperty( new EscherBoolProperty( EscherProperties.TEXT__SIZE_TEXT_TO_FIT_SHAPE, 0x080008 ) );
            if (shape.IsNoFill)
            {
                // Wonderful... none of the spec's give any clue as to what these constants mean.
                opt.AddEscherProperty(new EscherBoolProperty(EscherProperties.Fill__NOFillHITTEST, 0x00110000));
            }
            else
            {
                opt.AddEscherProperty(new EscherBoolProperty(EscherProperties.Fill__NOFillHITTEST, 0x00010000));
            }
            opt.AddEscherProperty(new EscherRGBProperty(EscherProperties.Fill__FillCOLOR, shape.FillColor));
            opt.AddEscherProperty(new EscherBoolProperty(EscherProperties.GROUPSHAPE__PRINT, 0x080000));
            opt.AddEscherProperty(new EscherRGBProperty(EscherProperties.LINESTYLE__COLOR, shape.LineStyleColor));
            int options = 5;
            if (shape.LineWidth != HSSFShape.LINEWIDTH_DEFAULT)
            {
                opt.AddEscherProperty(new EscherSimpleProperty(EscherProperties.LINESTYLE__LINEWIDTH, shape.LineWidth));
                options++;
            }
            if (shape.LineStyle != HSSFShape.LINESTYLE_SOLID)
            {
                opt.AddEscherProperty(new EscherSimpleProperty(EscherProperties.LINESTYLE__LINEDASHING, shape.LineStyle));
                opt.AddEscherProperty(new EscherSimpleProperty(EscherProperties.LINESTYLE__LINEENDCAPSTYLE, 0));
                if (shape.LineStyle == HSSFShape.LINESTYLE_NONE)
                    opt.AddEscherProperty(new EscherBoolProperty(EscherProperties.LINESTYLE__NOLINEDRAWDASH, 0x00080000));
                else
                    opt.AddEscherProperty(new EscherBoolProperty(EscherProperties.LINESTYLE__NOLINEDRAWDASH, 0x00080008));
                options += 3;
            }
            opt.SortProperties();
            return options;   // # options Added
        }

    }
}