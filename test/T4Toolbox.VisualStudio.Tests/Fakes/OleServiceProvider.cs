// <copyright file="OleServiceProvider.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;

    internal class OleServiceProvider : IServiceProvider, Microsoft.VisualStudio.OLE.Interop.IServiceProvider, IDisposable
    {
        private static volatile object mutex = new object();
        private Dictionary<Guid, ServiceInstance> services = new Dictionary<Guid, ServiceInstance>();
        private bool isDisposed;

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider) == serviceType)
            {
                return this;
            }

            return this.services == null || !this.services.ContainsKey(serviceType.GUID) ? null : this.services[serviceType.GUID].Service;
        }

        public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
        {
            ppvObject = (IntPtr)0;
            var num = 0;
            ServiceInstance serviceInstance = null;
            if (this.services != null && this.services.ContainsKey(guidService))
            {
                serviceInstance = this.services[guidService];
            }

            if (serviceInstance == null)
            {
                return -2147467262;
            }

            if (riid.Equals(VSConstants.IID_IUnknown))
            {
                ppvObject = Marshal.GetIUnknownForObject(serviceInstance.Service);
            }
            else
            {
                var unk = IntPtr.Zero;
                try
                {
                    unk = Marshal.GetIUnknownForObject(serviceInstance.Service);
                    num = Marshal.QueryInterface(unk, ref riid, out ppvObject);
                }
                finally
                {
                    if (unk != IntPtr.Zero)
                    {
                        Marshal.Release(unk);
                    }
                }
            }

            return num;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddService(Type serviceType, object serviceInstance, bool shouldDisposeServiceInstance)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (serviceInstance == null)
            {
                throw new ArgumentNullException(nameof(serviceInstance));
            }

            if (this.services == null)
            {
                this.services = new Dictionary<Guid, ServiceInstance>();
            }

            if (this.services.ContainsKey(serviceType.GUID))
            {
                throw new InvalidOperationException();
            }

            this.services.Add(serviceType.GUID, new ServiceInstance(serviceInstance, shouldDisposeServiceInstance));
        }

        public void RemoveService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (!this.services.ContainsKey(serviceType.GUID))
            {
                return;
            }

            this.services.Remove(serviceType.GUID);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            lock (mutex)
            {
                if (disposing && this.services != null)
                {
                    while (this.services.Count > 0)
                    {
                        this.RemoveService(this.services.Keys.First());
                    }

                    this.services.Clear();
                    this.services = null;
                }

                this.isDisposed = true;
            }
        }

        private void RemoveService(Guid guid)
        {
            if (this.services == null)
            {
                return;
            }

            var service = this.services[guid];
            if (service == null)
            {
                return;
            }

            this.services.Remove(guid);
            if (!service.ShouldDispose || !(service.Service is IDisposable disposable))
            {
                return;
            }

            disposable.Dispose();
        }

        private class ServiceInstance
        {
            internal readonly object Service;
            internal readonly bool ShouldDispose;

            internal ServiceInstance(object service, bool shouldDispose)
            {
                this.Service = service;
                this.ShouldDispose = shouldDispose;
            }
        }
    }
}