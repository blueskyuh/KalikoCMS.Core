﻿#region License and copyright notice
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

namespace KalikoCMS.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Routing;
    using System.Web.Mvc;
    using Kaliko;
    using KalikoCMS.Core;
    using KalikoCMS.Modules;
    using KalikoCMS.Mvc.Framework;

    internal class RequestModule : RequestModuleBase, IStartupSequence {
        private static Dictionary<int, Type> _controllerList;

        public RequestModule() {
            RequestManager = new RequestManager();
        }

        protected override void RedirectToStartPage() {
            var page = PageFactory.GetPage(Configuration.SiteSettings.Instance.StartPageId);
            RedirectToController(page);
        }

        public static void RedirectToController(CmsPage page) {
            if (_controllerList.All(c => c.Key != page.PageTypeId)) {
                var exception = new Exception(string.Format("No controller is registered for pagetype of page '{0}'", page.PageName));
                Logger.Write(exception, Logger.Severity.Critical);
                throw exception;
            }

            var type = _controllerList[page.PageTypeId];
            var controller = (Controller)Activator.CreateInstance(type);
            var controllerName = StripEnd(type.Name.ToLowerInvariant(), "controller");

            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = "index";
            routeData.Values["currentPage"] = ((IPageController)controller).GetTypedPage(page);

            HttpContext.Current.Response.Clear();
            var requestContext = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
            ((IController)controller).Execute(requestContext);
            HttpContext.Current.Response.End();
        }

        private static string StripEnd(string text, string ending) {
            if (text.EndsWith(ending)) {
                return text.Substring(0, text.Length - ending.Length);
            }

            return text;
        }


        #region Startup sequence

        public void Startup() {
            _controllerList = BuildControllerList();
        }

        private static Dictionary<int, Type> BuildControllerList() {
            var controllerList = new Dictionary<int, Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                if (IsGenericAssembly(assembly)) {
                    continue;
                }

                foreach (var definedType in assembly.DefinedTypes) {
                    if (definedType.ImplementedInterfaces.All(i => i != typeof(IPageController))) {
                        continue;
                    }

                    if (definedType.BaseType == null) {
                        continue;
                    }

                    var pageType = PageType.GetPageType(definedType.BaseType.GenericTypeArguments.FirstOrDefault());

                    controllerList.Add(pageType.PageTypeId, definedType.AsType());
                }
            }

            return controllerList;
        }

        private static bool IsGenericAssembly(Assembly assembly) {
            var knownAssemblyNames = new[] { "System.", "Microsoft.", "KalikoCMS." };
            var isGenericAssembly = knownAssemblyNames.Any(knownAssemblyName => assembly.FullName.StartsWith(knownAssemblyName));

            return isGenericAssembly;
        }

        #endregion
    }
}
