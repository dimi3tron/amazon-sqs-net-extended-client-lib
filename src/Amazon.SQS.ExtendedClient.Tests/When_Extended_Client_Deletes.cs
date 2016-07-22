﻿namespace Amazon.SQS.ExtendedClient.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class When_Extended_Client_Deletes : ExtendedClientTestBase
    {
        [Test]
        public void Long_Message_It_Is_Deleted_From_s3()
        {
            var handleTail = "_handle_";
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, handleTail);
            client.DeleteMessage(new DeleteMessageRequest(SQS_QUEUE_NAME, longReceiptHandle));
            s3Mock.Verify(m => m.DeleteObject(It.Is<string>(s => s.Equals(S3_BUCKET_NAME)), It.Is<string>(s => s.Equals(s3Key))));
            sqsMock.Verify(m => m.DeleteMessage(It.Is<DeleteMessageRequest>(r => r.QueueUrl.Equals(SQS_QUEUE_NAME) && r.ReceiptHandle.Equals(handleTail))));
        }

        [Test]
        public async Task Long_Message_Async_It_Is_Deleted_From_s3()
        {
            var handleTail = "_handle_";
            var s3Key = Guid.NewGuid().ToString("N");
            var longReceiptHandle = GenerateReceiptHandle(S3_BUCKET_NAME, s3Key, handleTail);
            await client.DeleteMessageAsync(new DeleteMessageRequest(SQS_QUEUE_NAME, longReceiptHandle));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.Is<string>(s => s.Equals(S3_BUCKET_NAME)), It.Is<string>(s => s.Equals(s3Key)), It.IsAny<CancellationToken>()));
            sqsMock.Verify(m => m.DeleteMessageAsync(It.Is<DeleteMessageRequest>(r => r.QueueUrl.Equals(SQS_QUEUE_NAME) && r.ReceiptHandle.Equals(handleTail)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void Short_Message_It_Is_Deleted_Only_From_Queue()
        {
            client.DeleteMessage(new DeleteMessageRequest(SQS_QUEUE_NAME, "handle"));
            s3Mock.Verify(m => m.DeleteObject(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            sqsMock.Verify(m => m.DeleteMessage(It.Is<DeleteMessageRequest>(r => r.QueueUrl.Equals(SQS_QUEUE_NAME) && r.ReceiptHandle.Equals("handle"))));
        }

        [Test]
        public async Task Short_Message_Async_It_Is_Deleted_Only_From_Queue()
        {
            await client.DeleteMessageAsync(new DeleteMessageRequest(SQS_QUEUE_NAME, "handle"));
            s3Mock.Verify(m => m.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            sqsMock.Verify(m => m.DeleteMessageAsync(It.Is<DeleteMessageRequest>(r => r.QueueUrl.Equals(SQS_QUEUE_NAME) && r.ReceiptHandle.Equals("handle")), It.IsAny<CancellationToken>()));
        }
    }
}