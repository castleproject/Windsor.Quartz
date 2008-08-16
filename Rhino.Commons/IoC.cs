#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using Castle.Windsor;

namespace Rhino.Commons
{
    public static class IoC
    {
        private static IWindsorContainer container;
        private static readonly object LocalContainerKey = new object();

        public static void Initialize(IWindsorContainer windsorContainer)
        {
            GlobalContainer = windsorContainer;
        }

		public static object Resolve(Type serviceType)
		{
			   return Container.Resolve(serviceType);
		}

		public static object Resolve(string serviceName)
		{
			return Container.Resolve(serviceName);
		}
		
		public static object Resolve(Type serviceType, string serviceName)
		{
			   return Container.Resolve(serviceName, serviceType);
        }

        /// <summary>
        /// Tries to resolve the component, but return null
        /// instead of throwing if it is not there.
        /// Useful for optional dependencies.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TryResolve<T>()
        {
            return TryResolve<T>(default(T));
        }

        /// <summary>
        /// Tries to resolve the compoennt, but return the default 
        /// value if could not find it, instead of throwing.
        /// Useful for optional dependencies.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T TryResolve<T>(T defaultValue)
        {
            if (Container.Kernel.HasComponent(typeof(T)) == false)
                return defaultValue;
            return Container.Resolve<T>();
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return Container.Resolve<T>(name);
        }

        public static IWindsorContainer Container
        {
            get
            {
                IWindsorContainer result = LocalContainer ?? GlobalContainer;
                if (result == null)
                    throw new InvalidOperationException("The container has not been initialized!");
                return result;
            }
        }

        private static IWindsorContainer LocalContainer
        {
            get
            {
                if (LocalContainerStack.Count==0)
                    return null;
                return LocalContainerStack.Peek();
            }
        }

        private static Stack<IWindsorContainer> LocalContainerStack
        {
            get
            {
                Stack<IWindsorContainer> stack = Local.Data[LocalContainerKey] as Stack<IWindsorContainer>;
                if(stack==null)
                {
                    Local.Data[LocalContainerKey] = stack = new Stack<IWindsorContainer>();
                }
                return stack;
            }
        }

        public static bool IsInitialized
        {
            get { return GlobalContainer != null; }
        }

        internal static IWindsorContainer GlobalContainer
        {
            get
            {
                return container;
            }
            set
            {
                container = value;
            }
        }

        /// <summary>
        /// This allows you to override the global container locally
        /// Useful for scenarios where you are replacing the global container
        /// but needs to do some initializing that relies on it.
        /// </summary>
        /// <param name="localContainer"></param>
        /// <returns></returns>
        public static IDisposable UseLocalContainer(IWindsorContainer localContainer)
        {
            LocalContainerStack.Push(localContainer);
            return new DisposableAction(delegate
            {
                Reset(localContainer);
            });
        }

        public static void Reset(IWindsorContainer containerToReset)
        {
			if(containerToReset==null)
				return;
			if (ReferenceEquals(LocalContainer, containerToReset))
			{
			    LocalContainerStack.Pop();
                if(LocalContainerStack.Count==0)
                    Local.Data[LocalContainerKey] = null;
                return;
			}
            if (ReferenceEquals(GlobalContainer, containerToReset))
            {
                GlobalContainer = null;
            }
        }

		public static void Reset()
		{
			IWindsorContainer windsorContainer = LocalContainer ?? GlobalContainer;
			Reset(windsorContainer);
		}
    }
}