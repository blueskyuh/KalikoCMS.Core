#region License and copyright notice
/* 
 * Kaliko Content Management System
 * 
 * Copyright (c) Fredrik Schultz
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 * http://www.gnu.org/licenses/lgpl-3.0.html
 */
#endregion

namespace KalikoCMS.Events {
    using System;
    using System.Linq;
    using System.Web.UI;

    public class SendFormEventArgs : EventArgs {
        private readonly ControlCollection _formContainer;

        public SendFormEventArgs(ControlCollection formContainer) {
            FormWasSentCorrectly = true;
            _formContainer = formContainer;
        }

        public bool FormWasSentCorrectly { get; set; }

        public Control this[string name] {
            get { return _formContainer.Cast<Control>().FirstOrDefault(control => control.ID == name); }
        }
    }
}