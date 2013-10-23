﻿#region License and copyright notice
/* 
 * Kaliko Content Management System
 * 
 * Copyright (c) Fredrik Schultz and Contributors
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

namespace KalikoCMS.Core.Collections {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using WebControls;

    public sealed class PageCollection : IEnumerable<CmsPage> {
        private static readonly object PadLock = new object();

        // TODO: Prepared for multilanguage
        // private readonly int _languageId;
        private readonly Collection<Guid> _pageIds;


        public PageCollection() {
            _pageIds = new Collection<Guid>();
        }

        public int Count {
            get { return _pageIds.Count; }
        }

        public Collection<Guid> PageIds {
            get { return _pageIds; }
        }

        public SortDirection SortDirection { get; set; }
        public SortOrder SortOrder { get; set; }
        public bool Sorted { get; set; }

        public void Add(Guid pageId) {
            _pageIds.Add(pageId);
        }

        public void Remove(Guid pageId) {
            _pageIds.Remove(pageId);
        }

        public bool Contains(Guid pageId) {
            return _pageIds.Contains(pageId);
        }

        public void Sort(SortOrder sortOrder, SortDirection sortDirection) {
            lock (PadLock) {
                if (IsAlreadySorted(sortOrder, sortDirection)) {
                    return;
                }

                Array values = new ArrayList(_pageIds).ToArray(typeof(Guid));
                Array keys = GetKeysFromParameters(sortOrder, values);

                Array.Sort(keys, values);
                _pageIds.Clear();

                if (sortDirection == SortDirection.Ascending) {
                    AddRange(values);
                }
                else {
                    AddRangeReversed(values);
                }

                Sorted = true;
                SortOrder = sortOrder;
                SortDirection = sortDirection;
            }
        }


        private void AddRange(ICollection items) {
            foreach (var item in items) {
                _pageIds.Add((Guid)item);
            }
        }


        private void AddRangeReversed(Array items) {
            for (int i = items.Length - 1; i >= 0; i--) {
                var item = items.GetValue(i);
                _pageIds.Add((Guid)item);
            }
        }


        private Array GetKeysFromParameters(SortOrder sortOrder, Array values) {
            Array keys = new ArrayList(values.Length).ToArray();

            //TODO: Add more sort orders including custom property names
            if (sortOrder == SortOrder.PageName) {
                keys = GetPageNameKeysForSort();
            }
            else if (sortOrder == SortOrder.StartPublishDate) {
                keys = GetStartPublishDateKeysForSort();
            }
            else if(sortOrder == SortOrder.SortIndex) {
                keys = GetSortIndexForSort();
            }
            else if (sortOrder == SortOrder.CreatedDate) {
                keys = GetCreatedDateForSort();
            }
            else if (sortOrder == SortOrder.UpdateDate) {
                keys = GetUpdateDateForSort();
            }

            return keys;
        }

        private Array GetUpdateDateForSort() {
            return _pageIds.Select(pageId => PageFactory.GetPage(pageId).UpdateDate).ToArray();
        }

        private Array GetCreatedDateForSort() {
            return _pageIds.Select(pageId => PageFactory.GetPage(pageId).CreatedDate).ToArray();
        }


        private Array GetSortIndexForSort() {
            return _pageIds.Select(pageId => PageFactory.GetPage(pageId).SortOrder).ToArray();
        }


        private Array GetPageNameKeysForSort() {
            return _pageIds.Select(pageId => PageFactory.GetPage(pageId).PageName).ToArray();
        }


        private Array GetStartPublishDateKeysForSort() {
            return _pageIds.Select(pageId => PageFactory.GetPage(pageId).StartPublish ?? DateTime.MinValue).ToArray();
        }


        private bool IsAlreadySorted(SortOrder sortOrder, SortDirection sortDirection) {
            return Sorted && SortOrder == sortOrder && SortDirection == sortDirection;
        }

        private static bool IsPageSourcesEqual(PageCollection pageSource1, PageCollection pageSource2) {
            if (ReferenceEquals(pageSource1, pageSource2)) {
                return true;
            }

            if (ReferenceEquals(pageSource1, null) || ReferenceEquals(pageSource2, null)) {
                return false;
            }

            if (pageSource1._pageIds.Count != pageSource2._pageIds.Count) {
                return false;
            }

            return pageSource1._pageIds.All(pageSource2._pageIds.Contains);
        }

        public override bool Equals(Object obj) {
            if (obj == null) {
                return false;
            }

            return this == obj as PageCollection;
        }

        public override int GetHashCode() {
            int hash = _pageIds.GetHashCode();
            return hash;
        }

        #region IEnumerable<CmsPage> Members

        IEnumerator<CmsPage> IEnumerable<CmsPage>.GetEnumerator() {
            return (IEnumerator<CmsPage>)GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return new PageCollectionEnumerator(_pageIds);
        }

        #endregion

        public static PageCollection operator +(PageCollection pageSource1, PageCollection pageSource2) {
            pageSource1.AddRange(pageSource2._pageIds);
            return pageSource1;
        }

        public static bool operator ==(PageCollection pageSource1, PageCollection pageSource2) {
            return IsPageSourcesEqual(pageSource1, pageSource2);
        }

        public static bool operator !=(PageCollection pageSource1, PageCollection pageSource2) {
            return !IsPageSourcesEqual(pageSource1, pageSource2);
        }
    }
}