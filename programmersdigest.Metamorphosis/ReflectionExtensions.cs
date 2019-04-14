using System;
using System.Linq;
using System.Reflection;

namespace programmersdigest.Metamorphosis
{
    internal static class ReflectionExtensions
    {
        public static bool IsMethodDefinitionCompatible(this MethodInfo methodA, MethodInfo methodB)
        {
            if (methodA.IsGenericMethod != methodB.IsGenericMethod)
            {
                return false;
            }

            // Check generic parameters.
            var genericParametersA = methodA.GetGenericArguments();
            var genericParametersB = methodB.GetGenericArguments();

            if (genericParametersA.Length != genericParametersB.Length)
            {
                return false;
            }

            for (var i = 0; i < genericParametersA.Length; i++)
            {
                if (!IsParameterCompatible(genericParametersA[i], genericParametersB[i]))
                {
                    return false;
                }
            }

            // Check return type.
            if (!IsParameterCompatible(methodA.ReturnType, methodB.ReturnType))
            {
                return false;
            }

            // Check parameters.
            var parametersA = methodA.GetParameters();
            var parametersB = methodB.GetParameters();

            if (parametersA.Length != parametersB.Length)
            {
                return false;
            }

            for (var i = 0; i < parametersA.Length; i++)
            {
                if (!IsParameterCompatible(parametersA[i].ParameterType, parametersB[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsParameterCompatible(Type typeA, Type typeB)
        {
            if (typeA.IsGenericParameter && typeB.IsGenericParameter)
            {
                if (!Enumerable.SequenceEqual(typeA.GetGenericParameterConstraints(), typeB.GetGenericParameterConstraints()))
                {
                    return false;
                }
            }
            else if (!typeA.IsGenericParameter && !typeB.IsGenericParameter)
            {
                if (typeA != typeB)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
