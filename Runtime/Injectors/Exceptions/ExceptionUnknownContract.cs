using System;
using EgorLin.DIsolated.Core.Extensions;

namespace EgorLin.DIsolated.Injectors.Exceptions
{
    public sealed class ExceptionUnknownContract : Exception
    {
        public Type UnknownContract { get; }
        
        public ExceptionUnknownContract(Type unknownContract) : base(GenerateMessage(unknownContract))
        {
            UnknownContract = unknownContract;
        }

        private static string GenerateMessage(Type unknownContract)
        {
            return $"Cannot resolve contract '{unknownContract.GetFullName()}'.";
        }
    }
}
