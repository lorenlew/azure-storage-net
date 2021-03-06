﻿// -----------------------------------------------------------------------------------------
// <copyright file="BlobTestBase.Common.cs" company="Microsoft">
//    Copyright 2013 Microsoft Corporation
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// -----------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.WindowsAzure.Storage.Blob
{
    public partial class BlobTestBase : TestBase
    {
        public static string GetRandomContainerName()
        {
            return string.Concat("testc", Guid.NewGuid().ToString("N"));
        }

        public static CloudBlobContainer GetRandomContainerReference(DelegatingHandler delegatingHandler = null)
        {
            CloudBlobClient blobClient = GenerateCloudBlobClient(delegatingHandler);
            string name = GetRandomContainerName();
            CloudBlobContainer container = blobClient.GetContainerReference(name);

            return container;
        }

        public static CloudBlobContainer GetRandomPremiumBlobContainerReference()
        {
            if (TestBase.PremiumBlobTenantConfig == null || TestBase.PremiumBlobStorageCredentials == null)
            {
                Assert.Inconclusive("A premium blob storage account must be specified to run this test.");
            }

            Uri baseAddressUri = new Uri(TestBase.PremiumBlobTenantConfig.BlobServiceEndpoint);
            CloudBlobClient blobClient = new CloudBlobClient(baseAddressUri, TestBase.PremiumBlobStorageCredentials);
            string name = GetRandomContainerName();
            CloudBlobContainer container = blobClient.GetContainerReference(name);

            return container;
        }

        public static CloudBlobContainer GenerateRandomWriteOnlyBlobContainer()
        {
            string blobContainerName = "n" + Guid.NewGuid().ToString("N");

            SharedAccessAccountPolicy sasAccountPolicy = new SharedAccessAccountPolicy()
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(30),
                Permissions = SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.Delete,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object | SharedAccessAccountResourceTypes.Container

            };

            CloudBlobClient blobClient = GenerateCloudBlobClient();
            CloudStorageAccount account = new CloudStorageAccount(blobClient.Credentials, false);
            string accountSASToken = account.GetSharedAccessSignature(sasAccountPolicy);
            StorageCredentials accountSAS = new StorageCredentials(accountSASToken);
            StorageUri storageUri = blobClient.StorageUri;
            CloudStorageAccount accountWithSAS = new CloudStorageAccount(accountSAS, storageUri, null, null, null);
            CloudBlobClient blobClientWithSAS = accountWithSAS.CreateCloudBlobClient();
            CloudBlobContainer containerWithSAS = blobClientWithSAS.GetContainerReference(blobContainerName);
            return containerWithSAS;
        }

        public static List<string> GetBlockIdList(int count)
        {
            List<string> blocks = new List<string>();
            for (int i = 0; i < count; i++)
            {
                blocks.Add(Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
            }
            return blocks;
        }

        public static void AssertAreEqual(CloudBlob expected, CloudBlob actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.BlobType, actual.BlobType);
                Assert.AreEqual(expected.Uri, actual.Uri);
                Assert.AreEqual(expected.StorageUri, actual.StorageUri);
                Assert.AreEqual(expected.SnapshotTime, actual.SnapshotTime);
                Assert.AreEqual(expected.IsSnapshot, actual.IsSnapshot);
                Assert.AreEqual(expected.SnapshotQualifiedUri, actual.SnapshotQualifiedUri);
                AssertAreEqual(expected.Properties, actual.Properties);
                AssertAreEqual(expected.CopyState, actual.CopyState);
            }
        }

        public static void AssertAreEqual(BlobProperties expected, BlobProperties actual)
        {
            AssertAreEqual(expected, actual, true);
        }

        public static void AssertAreEqual(BlobProperties expected, BlobProperties actual, bool checkContentMD5)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.CacheControl, actual.CacheControl);
                Assert.AreEqual(expected.ContentDisposition, actual.ContentDisposition);
                Assert.AreEqual(expected.ContentEncoding, actual.ContentEncoding);
                Assert.AreEqual(expected.ContentLanguage, actual.ContentLanguage);
                if (checkContentMD5)
                {
                    Assert.AreEqual(expected.ContentMD5, actual.ContentMD5);
                }
                Assert.AreEqual(expected.ContentType, actual.ContentType);
                Assert.AreEqual(expected.ETag, actual.ETag);
                Assert.AreEqual(expected.LastModified, actual.LastModified);
                Assert.AreEqual(expected.Length, actual.Length);
            }
        }

        public static void AssertAreEqual(CopyState expected, CopyState actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.BytesCopied, actual.BytesCopied);
                Assert.AreEqual(expected.CompletionTime, actual.CompletionTime);
                Assert.AreEqual(expected.CopyId, actual.CopyId);
                Assert.AreEqual(expected.Source, actual.Source);
                Assert.AreEqual(expected.Status, actual.Status);
                Assert.AreEqual(expected.TotalBytes, actual.TotalBytes);
            }
        }
    }
}
