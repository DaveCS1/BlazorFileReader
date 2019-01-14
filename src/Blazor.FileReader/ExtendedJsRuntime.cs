﻿using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System;
using System.Threading.Tasks;

namespace Blazor.FileReader
{
    public interface IExtendedJSRuntime: IJSRuntime
    {
        TReturn InvokeUnmarshalled<T1, T2, TReturn>(string functionName, T1 arg1, T2 arg2);
    }

    public static class ExtendedJSRuntime
    {
        private static readonly Lazy<IExtendedJSRuntime> extendedJSRuntimeWrapper = 
            new Lazy<IExtendedJSRuntime>(() => new ExtendedJSRuntimeWrapper(JSRuntime.Current));

        public static IExtendedJSRuntime Current => extendedJSRuntimeWrapper.Value;

        public static bool IsAvailable { get; } = JSRuntime.Current is MonoWebAssemblyJSRuntime;

        private class ExtendedJSRuntimeWrapper : IExtendedJSRuntime
        {
            private MonoWebAssemblyJSRuntime current;

            public ExtendedJSRuntimeWrapper(IJSRuntime current)
            {
                this.current = current as MonoWebAssemblyJSRuntime
                    ?? throw new PlatformNotSupportedException($"Requires {nameof(MonoWebAssemblyJSRuntime)} as the JSRuntime");
            }

            public Task<T> InvokeAsync<T>(string identifier, params object[] args)
            {
                return current.InvokeAsync<T>(identifier, args);
            }

            public TRes InvokeUnmarshalled<T1, T2, TRes>(string identifier, T1 arg1, T2 arg2)
            {
                return current.InvokeUnmarshalled<T1, T2, TRes>(identifier, arg1, arg2);
            }

            public void UntrackObjectRef(DotNetObjectRef dotNetObjectRef)
            {
                current.UntrackObjectRef(dotNetObjectRef);
            }
        }
    }
}
