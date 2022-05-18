// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyInternetConnectionLibrary
{
    // custom exception thrown when internet connection issues occur
    [Serializable]
    public class NoInternetConnectionException : Exception
    {
        public NoInternetConnectionException()
        {
        }

        public NoInternetConnectionException(string message) : base(message)
        {
        }

        public NoInternetConnectionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoInternetConnectionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}