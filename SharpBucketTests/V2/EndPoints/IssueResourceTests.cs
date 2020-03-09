﻿using NUnit.Framework;
using SharpBucket.V2.EndPoints;
using SharpBucket.V2.Pocos;
using Shouldly;

namespace SharpBucketTests.V2.EndPoints
{
    [TestFixture]
    internal class IssueResourceTests
    {
        private IssueResource ExistingIssue { get; set; }

        private IssueResource NotExistingIssue { get; set; }

        [OneTimeSetUp]
        protected void Init()
        {
            ExistingIssue = SampleRepositories.BotTestRepository.IssuesResource().IssueResource(2);

            NotExistingIssue = SampleRepositories.BotTestRepository.IssuesResource().IssueResource(int.MaxValue);
        }

        [Test]
        public void GetPullRequest_ExistingPublicIssue_ReturnValidInfo()
        {
            var issue = ExistingIssue.GetIssue();
            issue.ShouldNotBeNull();
            issue.id.ShouldBe(2);
            issue.reporter?.nickname.ShouldBe("penev92");
            issue.title.ShouldBe("Some other test issue");
            issue.state.ShouldBe(IssueStatus.Resolved);
        }
    }
}
