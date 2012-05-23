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

namespace KalikoCMS.Core {
    using System;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;

    internal class PageProxy : RealProxy {
        private readonly object _target;

        protected internal PageProxy(Type type) : base(type) {
            _target = Activator.CreateInstance(type);
        }

        public static CmsPage CreatePageProxy(Type type) {
            var pageProxy = new PageProxy(type);
            return (CmsPage)pageProxy.GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage message) {
            var methodMessage = (IMethodCallMessage)message;
            var method = methodMessage.MethodBase;
            object returnValue;

            if (method.IsVirtual) {
                returnValue = HandleVirtualMethods(methodMessage, method);
            }
            else {
                try {
                    returnValue = method.Invoke(_target, methodMessage.Args);
                }
                catch (Exception exception) {
                    throw GetExceptionToRethrow(exception);
                }
            }

            var returnMessage = BuildReturnMessage(methodMessage, returnValue);
            return returnMessage;
        }

        private object HandleVirtualMethods(IMethodCallMessage methodMessage, MethodBase method) {
            string methodName = method.Name;

            if (methodName.StartsWith("get_")) {
                var currentPage = (CmsPage)_target;
                string propertyName = methodName.Substring(4);
                var propertyData = currentPage.Property[propertyName];

                return propertyData;
            }

            return null;
        }

        private static Exception GetExceptionToRethrow(Exception exception) {
            if (exception.InnerException != null) {
                return exception.InnerException;
            }

            return exception;
        }

        private static IMessage BuildReturnMessage(IMethodCallMessage methodMessage, object returnValue) {
            return new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
        }
    }
}
