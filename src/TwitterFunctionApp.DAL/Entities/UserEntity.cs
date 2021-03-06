﻿using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TwitterFunctionApp.DAL.Entities
{
    public class UserEntity : TableEntity
    {
        public UserEntity() : base() { }

        public UserEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public DateTime UpdatedDate { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string OAuthVerifier { get; set; }

        public override string ToString()
        {
            return string.Format("User - P:{0} R:{1} OAT:{2} OAV:{3}", PartitionKey, RowKey, OAuthToken, OAuthVerifier);
        }
    }
}
