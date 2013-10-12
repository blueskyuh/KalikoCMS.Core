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

namespace KalikoCMS.PropertyType {
    using System;
    using Attributes;
    using Core;
    using Serialization;

    [PropertyType("56A791FC-D480-40A9-B161-651B9C7D8AEA", "PageLink", "Page link property", "~/Admin/Content/PropertyType/PageLinkPropertyEditor.ascx")]
    public class PageLinkProperty : PropertyData {
        private int? _cachedHashCode;
        private CmsPage _page;

        public Guid PageId { get; set; }

        public int LanguageId { get; set; }

        protected override string StringValue {
            get {
                return Page.PageUrl.ToString();
            }
        }

        public CmsPage Page {
            get {
                return _page ?? (_page = PageFactory.GetPage(PageId, LanguageId));
            }
        }

        public bool IsValid {
            get {
                if (PageId == Guid.Empty) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }

        public PageLinkProperty() {
        }

        public PageLinkProperty(CmsPage page) {
            _page = page;
            PageId = page.PageId;
            LanguageId = page.LanguageId;
        }

        public PageLinkProperty(int languageId, Guid pageId) {
            LanguageId = languageId;
            PageId = pageId;
        }

        protected override PropertyData DeserializeFromJson(string data) {
            return JsonSerialization.DeserializeJson<PageLinkProperty>(data);
        }

        public override int GetHashCode() {
            return (int)(_cachedHashCode ?? (_cachedHashCode = CalculateHashCode()));
        }

        private int CalculateHashCode() {
            int hash = JsonSerialization.GetNewHash();
            hash = JsonSerialization.CombineHashCode(hash, PageId);
            hash = JsonSerialization.CombineHashCode(hash, LanguageId);

            return hash;
        }
    }

}
