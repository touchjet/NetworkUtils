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
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace Touchjet.NetworkUtils
{
    public class BaseServiceClient
    {
        readonly HttpClient _client;
        readonly ApiAuthorizer _authorizer;
        readonly string _baseUrl;
        protected bool _debugOutput;

        public BaseServiceClient(string baseUrl, ApiAuthorizer authorizer = null)
        {
            _baseUrl = baseUrl;
            _client = new HttpClient();
            _authorizer = authorizer;
        }

        protected async Task<T> Get<T>(string path)
        {
            Log.Debug($"HTTP GET {path}");
            if (_authorizer != null)
            {
                await _authorizer.AddToken(_client);
            }
            using (var response = await _client.GetAsync(_baseUrl+path))
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsJsonAsync<T>();
                return result;
            }
        }

        protected async Task<T> Post<I, T>(string path, I request)
        {
            Log.Debug($"HTTP POST {path}");
            if (_authorizer != null)
            {
                await _authorizer.AddToken(_client);
            }
            using (var response = await _client.PostAsJsonAsync<I>(_baseUrl + path, request))
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsJsonAsync<T>();
                return result;
            }
        }

    }
}
