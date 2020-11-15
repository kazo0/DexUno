using System;

namespace Dex.Uwp.Services
{
    public class NavigationServiceException : Exception
    {
        public NavigationServiceException(string message) : base(message)
        {
        }
    }
}