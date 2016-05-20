/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
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

namespace NPOI
{
    using System;
    using System.IO;
    using System.Collections;
    using NPOI.Util;
    using NPOI.POIFS.FileSystem;
    using NPOI.HPSF;


    /// <summary>
    /// This holds the common functionality for all POI
    /// Document classes.
    /// Currently, this relates to Document Information Properties
    /// </summary>
    /// <remarks>@author Nick Burch</remarks>
    [Serializable]
    public abstract class POIDocument:IDisposable
    {
        /** Holds metadata on our document */
        protected SummaryInformation sInf;
        /** Holds further metadata on our document */
        protected DocumentSummaryInformation dsInf;
        /** The open POIFS FileSystem that contains our document */
        protected POIFSFileSystem filesystem;
        /**	The directory that our document lives in */
        protected DirectoryNode directory;

        /** For our own logging use */
        //protected POILogger logger;

        /* Have the property streams been Read yet? (Only done on-demand) */
        protected bool initialized = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="POIDocument"/> class.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="fs">The fs.</param>
        public POIDocument(DirectoryNode dir, POIFSFileSystem fs)
        {
            this.filesystem = fs;
            this.directory = dir;
            //POILogFactory.GetLogger(this.GetType());
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="POIDocument"/> class.
        /// </summary>
        /// <param name="fs">The fs.</param>
        public POIDocument(POIFSFileSystem fs)
            : this(fs.Root, fs) 
        {
            
        }
        public virtual void Dispose()
        {
            this.directory = null;
            this.dsInf = null;
            this.filesystem = null;
        }
        /// <summary>
        /// Fetch the Document Summary Information of the document
        /// </summary>
        /// <value>The document summary information.</value>
        public DocumentSummaryInformation DocumentSummaryInformation
        {
            get
            {
                if (!initialized) ReadProperties();
                return dsInf;
            }
            set 
            {
                dsInf = value;
            }
        }

        /// <summary>
        /// Fetch the Summary Information of the document
        /// </summary>
        /// <value>The summary information.</value>
        public SummaryInformation SummaryInformation
        {
            get
            {
                if (!initialized) ReadProperties();
                return sInf;
            }
            set 
            {
                sInf = value;
            }
        }

        /// <summary>
        /// Find, and Create objects for, the standard
        /// Documment Information Properties (HPSF).
        /// If a given property Set is missing or corrupt,
        /// it will remain null;
        /// </summary>
        protected void ReadProperties()
        {
            PropertySet ps;

            // DocumentSummaryInformation
            ps = GetPropertySet(DocumentSummaryInformation.DEFAULT_STREAM_NAME);
            if (ps != null && ps is DocumentSummaryInformation)
            {
                dsInf = (DocumentSummaryInformation)ps;
            }
            else if (ps != null)
            {
                //logger.Log(POILogger.WARN, "DocumentSummaryInformation property Set came back with wrong class - ", ps.GetType());
            }

            // SummaryInformation
            ps = GetPropertySet(SummaryInformation.DEFAULT_STREAM_NAME);
            if (ps is SummaryInformation)
            {
                sInf = (SummaryInformation)ps;
            }
            else if (ps != null)
            {
                //logger.Log(POILogger.WARN, "SummaryInformation property Set came back with wrong class - ", ps.GetType());
            }

            // Mark the fact that we've now loaded up the properties
            initialized = true;
        }

        /// <summary>
        /// For a given named property entry, either return it or null if
        /// if it wasn't found
        /// </summary>
        /// <param name="SetName">Name of the set.</param>
        /// <returns></returns>
        protected PropertySet GetPropertySet(String SetName)
        {
            POIFSDocumentReader dis;
            try
            {
                // Find the entry, and Get an input stream for it
                dis = directory.CreatePOIFSDocumentReader(SetName);
            }
            catch (IOException ie)
            {
                // Oh well, doesn't exist
                //logger.Log(POILogger.WARN, "Error Getting property Set with name " + SetName + "\n" + ie);
                return null;
            }

            try
            {
                // Create the Property Set
                PropertySet Set = PropertySetFactory.Create(dis);
                return Set;
            }
            catch (IOException ie)
            {
                // Must be corrupt or something like that
                //logger.Log(POILogger.WARN, "Error creating property Set with name " + SetName + "\n" + ie);
            }
            catch (HPSFException he)
            {
                // Oh well, doesn't exist
                //logger.Log(POILogger.WARN, "Error creating property Set with name " + SetName + "\n" + he);
            }
            return null;
        }

        /// <summary>
        /// Writes out the standard Documment Information Properties (HPSF)
        /// </summary>
        /// <param name="outFS">the POIFSFileSystem to Write the properties into</param>
        protected void WriteProperties(POIFSFileSystem outFS)
        {
            WriteProperties(outFS, null);
        }
        /// <summary>
        /// Writes out the standard Documment Information Properties (HPSF)
        /// </summary>
        /// <param name="outFS">the POIFSFileSystem to Write the properties into.</param>
        /// <param name="writtenEntries">a list of POIFS entries to Add the property names too.</param>
        protected void WriteProperties(POIFSFileSystem outFS, IList writtenEntries)
        {
            if (sInf != null)
            {
                WritePropertySet(SummaryInformation.DEFAULT_STREAM_NAME, sInf, outFS);
                if (writtenEntries != null)
                {
                    writtenEntries.Add(SummaryInformation.DEFAULT_STREAM_NAME);
                }
            }
            if (dsInf != null)
            {
                WritePropertySet(DocumentSummaryInformation.DEFAULT_STREAM_NAME, dsInf, outFS);
                if (writtenEntries != null)
                {
                    writtenEntries.Add(DocumentSummaryInformation.DEFAULT_STREAM_NAME);
                }
            }
        }

        /// <summary>
        /// Writes out a given ProperySet
        /// </summary>
        /// <param name="name">the (POIFS Level) name of the property to Write.</param>
        /// <param name="Set">the PropertySet to Write out.</param>
        /// <param name="outFS">the POIFSFileSystem to Write the property into.</param>
        protected void WritePropertySet(String name, PropertySet Set, POIFSFileSystem outFS)
        {
            try
            {
                MutablePropertySet mSet = new MutablePropertySet(Set);
                MemoryStream bOut = new MemoryStream();

                mSet.Write(bOut);
                byte[] data = bOut.ToArray();
                MemoryStream bIn = new MemoryStream(data);
                outFS.CreateDocument(bIn, name);

                //logger.Log(POILogger.INFO, "Wrote property Set " + name + " of size " + data.Length);
            }
            catch (WritingNotSupportedException)
            {
                Console.Error.WriteLine("Couldn't Write property Set with name " + name + " as not supported by HPSF yet");
            }
        }

        /// <summary>
        /// Writes the document out to the specified output stream
        /// </summary>
        /// <param name="out1">The out1.</param>
        public abstract void Write(Stream out1);

        /// <summary>
        /// Copies nodes from one POIFS to the other minus the excepts
        /// </summary>
        /// <param name="source">the source POIFS to copy from.</param>
        /// <param name="target">the target POIFS to copy to</param>
        /// <param name="excepts">a list of Strings specifying what nodes NOT to copy</param>
        protected void CopyNodes(POIFSFileSystem source, POIFSFileSystem target,
                                  IList excepts)
        {
            //System.err.println("CopyNodes called");

            DirectoryEntry root = source.Root;
            DirectoryEntry newRoot = target.Root;

            IEnumerator entries = root.Entries;

            while (entries.MoveNext())
            {
                Entry entry = (Entry)entries.Current;
                if (!isInList(entry.Name, excepts))
                {
                    CopyNodeRecursively(entry, newRoot);
                }
            }
        }

        /// <summary>
        /// Checks to see if the String is in the list, used when copying
        /// nodes between one POIFS and another
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="list">The list.</param>
        /// <returns>
        /// 	<c>true</c> if [is in list] [the specified entry]; otherwise, <c>false</c>.
        /// </returns>
        private bool isInList(String entry, IList list)
        {
            for (int k = 0; k < list.Count; k++)
            {
                if (list[k].Equals(entry))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies an Entry into a target POIFS directory, recursively
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="target">The target.</param>
        private void CopyNodeRecursively(Entry entry, DirectoryEntry target)
        {
            //System.err.println("copyNodeRecursively called with "+entry.Name+
            //                   ","+target.Name);
            DirectoryEntry newtarget = null;
            if (entry.IsDirectoryEntry)
            {
                newtarget = target.CreateDirectory(entry.Name);
                IEnumerator entries = ((DirectoryEntry)entry).Entries;

                while (entries.MoveNext())
                {
                    CopyNodeRecursively((Entry)entries.Current, newtarget);
                }
            }
            else
            {
                DocumentEntry dentry = (DocumentEntry)entry;
                POIFSDocumentReader dstream = new POIFSDocumentReader(dentry);
                target.CreateDocument(dentry.Name, dstream);
                dstream.Close();
            }
        }
    }
}