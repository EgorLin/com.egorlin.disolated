using System;
using System.Linq;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Exceptions
{
    internal sealed class ExceptionConstructorInjector : Exception
    {
        public ExceptionConstructorInjector(Type type, Exception exception, InfoConstructorParameter[] constructorParameters) : base(BuildMessage(type, exception, constructorParameters))
        {
        }

        private static string BuildMessage(Type type, Exception exception, InfoConstructorParameter[] constructorParameters)
        {
            var constructorSignature = $"{type.Name} ({string.Join(", ", constructorParameters.Select(t => t.ParameterType.Name))})";
            return $"{exception.Message} occurred while instantiating object type '{type.GetFullName()}' using constructor {constructorSignature}";
        }
    }
}
