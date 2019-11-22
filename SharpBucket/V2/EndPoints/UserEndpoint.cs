﻿using SharpBucket.V2.Pocos;
using System.Threading.Tasks;

namespace SharpBucket.V2.EndPoints
{
    public class UserEndpoint : EndPoint
    {
        public UserEndpoint(ISharpBucketRequesterV2 sharpBucketV2)
            : base(sharpBucketV2, "user/")
        {
        }

        public User GetUser()
        {
            return _sharpBucketV2.Get<User>(_baseUrl);
        }

        public async Task<User> GetUserAsync()
        {
            return await _sharpBucketV2.GetAsync<User>(_baseUrl);
        }
    }
}