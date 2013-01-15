#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod
{
    /// <summary>
    /// Generic class to get/add strongly typed Sequence Iods.
    /// <example>
    /// <code><![CDATA[
    /// public SequenceIodList<ScheduledProcedureStepSequenceIod> ScheduledProcedureStepSequenceList
    /// {
    ///     get 
    ///     {
    ///         return new SequenceIodList<ScheduledProcedureStepSequenceIod>(base.DicomAttributeCollection[DicomTags.ScheduledProcedureStepSequence] as DicomAttributeSQ); 
    ///     }
    ///  }]]>
    /// </code></example>
    /// </summary>
    /// <typeparam name="T">Type of SequenceIod</typeparam>
    public class SequenceIodList<T> : IList<T>
        where T:SequenceIodBase, new()
    {
        #region Private Variables
        /// <summary>
        /// 
        /// </summary>
        DicomAttributeSQ _dicomAttributeSQ;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceIodList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="dicomTag">The dicom tag.</param>
        public SequenceIodList(DicomTag dicomTag)
        {
            _dicomAttributeSQ = new DicomAttributeSQ(dicomTag);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceIodList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public SequenceIodList(uint tag)
        {
            _dicomAttributeSQ = new DicomAttributeSQ(tag);
        }

        /// <summary>
        /// Initializes a new instance of the SequenceIodList class.
        /// </summary>
        public SequenceIodList(DicomAttributeSQ dicomAttributeSQ)
        {
            if (dicomAttributeSQ == null)
                throw new ArgumentNullException("dicomAttributeSQ");

            _dicomAttributeSQ = dicomAttributeSQ;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the DicomAttributeSQ.
        /// </summary>
        /// <value>The dicom attribute SQ.</value>
        public DicomAttributeSQ dicomAttributeSQ
        {
            get { return _dicomAttributeSQ; }
            set { _dicomAttributeSQ = value; }
        }

        /// <summary>
        /// Gets the first sequence item, for binding purposes.
        /// </summary>
        /// <value>The first sequence item.</value>
        public T FirstSequenceItem
        {
            get { return this[0]; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the generic sequence iod from dicom sequence item.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        /// <returns></returns>
        /// <remarks>Can't specify a constructor parameter for a generic type so need to do it like this.</remarks>
        private T GetSequenceIodFromDicomSequenceItem(DicomSequenceItem dicomSequenceItem)
        {
            if (dicomSequenceItem == null)
                return null;
            else
            {
                T newT = new T();
                newT.DicomSequenceItem = dicomSequenceItem;
                return newT;
            }
        }
        #endregion

        #region IList<T> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            IEnumerator<T> e = this.GetEnumerator();
            int i = 0;
            while (e.MoveNext())
            {
                T obj = e.Current;
                if (obj.Equals(item))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Note: Not Yet Implemented.  Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
            //TODO: to implement, would have to implement these methods in the DicomAttributeSQ class, or perhaps turn DicomAttributeSQ to use a a List instead of an array?
        }

        /// <summary>
        /// Not Yet Implemented.  Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
            //TODO: to implement, would have to implement these methods in the DicomAttributeSQ class, or perhaps turn DicomAttributeSQ to use a a List instead of an array?
        }

        /// <summary>
        /// Getsthe <see cref="T"/> at the specified index.  Set is Not Yet Implemented.  
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                if (_dicomAttributeSQ.Count > index)
                {
                    return GetSequenceIodFromDicomSequenceItem(_dicomAttributeSQ[index]);
                }
                return null;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
                //TODO: to implement, would have to implement these methods in the DicomAttributeSQ class, or perhaps turn DicomAttributeSQ to use a a List instead of an array?

            }
        }


        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(T item)
        {
            _dicomAttributeSQ.AddSequenceItem(item.DicomSequenceItem);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _dicomAttributeSQ.ClearSequenceItems();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return (this.IndexOf(item) > -1);
        }

        /// <summary>
        /// Not Yet Implemented.  Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        /// 	<exception cref="NotImplementedException"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        public int Count
        {
            get { return (int) _dicomAttributeSQ.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Not Yet Implemented.  Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        /// <exception cref="NotImplementedException"/>
        public bool Remove(T item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _dicomAttributeSQ.Count; i++)
            {
                yield return GetSequenceIodFromDicomSequenceItem(_dicomAttributeSQ[i]);
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
