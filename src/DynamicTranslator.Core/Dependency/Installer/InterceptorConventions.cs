﻿namespace DynamicTranslator.Core.Dependency.Installer
{
    #region using

    using System.Linq;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Facilities;
    using Interceptors;
    using Orchestrators.Detector;
    using Orchestrators.Finder;

    #endregion

    public class InterceptorConventions : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.ComponentRegistered += KernelOnComponentRegistered;
        }

        private void KernelOnComponentRegistered(string key, IHandler handler)
        {
            var isMeanFinder = handler.ComponentModel.Implementation.GetInterfaces().Contains(typeof (IMeanFinder));
            if (isMeanFinder)
            {
                handler.ComponentModel.Interceptors.AddFirst(new InterceptorReference(typeof (ExceptionInterceptor)));
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof (TextGuardInterceptor)));
            }

            var isDetector = handler.ComponentModel.Implementation.GetInterfaces().Contains(typeof (ILanguageDetector));
            if (isDetector)
            {
                handler.ComponentModel.Interceptors.AddFirst(new InterceptorReference(typeof (ExceptionInterceptor)));
            }
        }
    }
}