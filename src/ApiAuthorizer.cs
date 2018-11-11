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
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Touchjet.NetworkUtils
{
    public class ApiAuthorizer
    {
        string _accessToken;
        DateTime _tokenExpiry;
        string _authenticationUrl;
        string _api;
        string _secret;

        public ApiAuthorizer(string authenticationUrl, string api, string secret)
        {
            _authenticationUrl = authenticationUrl;
            _api = api;
            _secret = secret;
        }

        public async Task AddToken(HttpClient httpClient)
        {
            if (string.IsNullOrWhiteSpace(_accessToken) || (_tokenExpiry < DateTime.UtcNow))
            {
                await CredentialsAuthentication();
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        async Task CredentialsAuthentication()
        {
            using (HttpClient client = new HttpClient())
            {
                Log.Verbose($"Authentication with client_id: {_api} client_secret:{_secret} grant_type:client_credentials at {_authenticationUrl}");
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _api),
                    new KeyValuePair<string, string>("client_secret", _secret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });
                var response = await client.PostAsync(_authenticationUrl, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsJsonAsync<TokenResult>();
                    if (String.IsNullOrEmpty(result.Error))
                    {
                        _accessToken = result.AccessToken;
                        _tokenExpiry = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 120);
                        return;
                    }
                }
            }
            throw new Exception("Api Authorization Failed.");
        }

        public class TokenResult
        {
            [JsonProperty("error")]
            public string Error { get; set; }
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }
    }
}
