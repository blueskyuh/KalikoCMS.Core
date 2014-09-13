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

namespace KalikoCMS.Modules {
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Web;
    using Core;
    using Data;
    using Data.Entities;
    using Kaliko;

    public class InitModule : IHttpModule {
        private static bool _isRunning;
        private static bool _firstRun = true;
        private static readonly object Padlock = new object();

        private InitModule() { }

        public void Init(HttpApplication context) {
            BreakIfAlreadyRunning();

            lock (Padlock) {
                if (_firstRun) {
                    InitializeApplication();
                }
            }
        }

        private static void InitializeApplication() {
            _isRunning = true;

            try {
                RunInitializingSteps();
            }
            catch (Exception exception) {
                _firstRun = true;
                _isRunning = false;
                Logger.Write(exception, Logger.Severity.Critical);
                throw;
            }
            finally {
                _isRunning = false;
            }

            _firstRun = false;
        }

        private static void RunInitializingSteps() {
            AutoMapperConfiguration.Configure();
            KeepDatabaseUpToDate();
            PageType.LoadPageTypes();
            PageFactory.IndexSite();
            RunStartupSequence();
        }

        private static void KeepDatabaseUpToDate() {
            // TODO: Version database to prevent downgrade + read languages from web.config (i.e. don't hard code 'English')
            using (var context = new DataContext()) {
                // Keep schema up to date
                context.UpdateSchema();

                // Ensure that at least one language is available
                if (context.SiteLanguages.Count() == 0) {
                    context.Add(new SiteLanguageEntity {
                        ShortName = "en",
                        LongName = "English"
                    });
                    context.SaveChanges();
                }
            }
        }

        private static void RunStartupSequence() {
            var types = InterfaceReader.GetTypesWithInterface(typeof(IStartupSequence));
            
            foreach (var type in types) {
                if(type.IsInterface) {
                    continue;
                }

                var startupSequence = Activator.CreateInstance(type) as IStartupSequence;
                if (startupSequence != null) {
                    startupSequence.Startup();
                }
            }
        }

        private static void BreakIfAlreadyRunning() {
            if (!_isRunning) {
                return;
            }

            var httpResponse = HttpContext.Current.Response;
            httpResponse.StatusCode = 503;
            httpResponse.Write("System is starting up.. Please check back in a few seconds.");
            httpResponse.End();
        }

        public void Dispose() {
        }
    }
}
