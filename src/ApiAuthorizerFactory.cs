/*
 * Copyright (C) 2018 Touchjet Limited.
 * 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using System.Collections.Generic;

namespace Touchjet.NetworkUtils
{
    public static class ApiAuthorizerFactory
    {
        static Dictionary<string, ApiAuthorizer> _authorizers;
        static object accessLock = new object();

        public static ApiAuthorizer GetAuthorizer(string apiUrl, string authUrl, string api, string secret)
        {
            lock (accessLock)
            {
                if (_authorizers == null)
                {
                    _authorizers = new Dictionary<string, ApiAuthorizer>();
                }
                if (_authorizers.ContainsKey(apiUrl))
                {
                    return _authorizers[apiUrl];
                }
                else
                {
                    var authorizer = new ApiAuthorizer(authUrl, api, secret);
                    _authorizers.Add(apiUrl, authorizer);
                    return authorizer;
                }
            }
        }
    }
}
