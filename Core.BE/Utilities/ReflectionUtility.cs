using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Emeint.Core.BE.Utilities
{
    public static class ReflectionUtility
    {
        public static List<Type> GetInheritedTypes<T>(Assembly assembly) where T : class
        {
            return assembly.GetTypes()
            .Where(type => type.BaseType == typeof(T)).ToList();
        }
    }
}
