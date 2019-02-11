﻿//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;

namespace AppCoordinates
{
    public class AppCoordinates
    {
        public string ClientId { get; set; }
        public string Tenant { get; set; }
        public string Authority { get { return $"https://login.microsoftonline.com/{Tenant}/";  } }
        public Uri RedirectUri { get; set; }
    }

    public static class PreRegisteredApps
    {
        public static AppCoordinates GetV1App(bool useInMsal)
        {
            return new AppCoordinates()
            {
                ClientId = "f0e0429e-060c-42d3-9375-913eb7c7a62d",
                Tenant = useInMsal ? "organizations" : "common", // Multi-tenant: you can try it out in your AAD organization
                RedirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob")
            };
        }

        // Resources
        public static string MsGraph = "https://graph.microsoft.com";

        // As Scope
        public static string[] MsGraphWithUserReadScope = new string[] { "https://graph.microsoft.com/user.read" };
    }
}
