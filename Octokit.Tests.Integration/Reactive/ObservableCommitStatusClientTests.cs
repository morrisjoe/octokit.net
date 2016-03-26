﻿using System.Reactive.Linq;
using System.Threading.Tasks;
using Octokit.Reactive;
using Xunit;

namespace Octokit.Tests.Integration.Reactive
{
    public class ObservableCommitStatusClientTests
    {
        public class TheGetAllMethod
        {
            readonly ObservableCommitStatusClient _commitStatusClient;
            const string owner = "octokit";
            const string name = "octokit.net";
            const string reference = "master";

            public TheGetAllMethod()
            {
                var github = Helper.GetAuthenticatedClient();
                _commitStatusClient = new ObservableCommitStatusClient(github);
            }

            [IntegrationTest]
            public async Task ReturnCommitStatus()
            {
                var commitStatus = await _commitStatusClient.GetAll(owner, name, reference).ToList();

                Assert.NotEmpty(commitStatus);
            }

            [IntegrationTest]
            public async Task ReturnsCorrectCountOfCommitStatusWithoutStart()
            {
                var options = new ApiOptions
                {
                    PageSize = 5,
                    PageCount = 1
                };

                var commitStatus = await _commitStatusClient.GetAll(owner, name ,reference , options).ToList();

                Assert.Equal(5, commitStatus.Count);
            }

            [IntegrationTest]
            public async Task ReturnsCorrectCountOfCommitStatusWithStart()
            {
                var options = new ApiOptions
                {
                    PageSize = 5,
                    PageCount = 1,
                    StartPage = 1
                };

                var commitStatus = await _commitStatusClient.GetAll(owner, name, reference, options).ToList();

                Assert.Equal(5, commitStatus.Count);
            }

            [IntegrationTest]
            public async Task ReturnsDistinctResultsBasedOnStartPage()
            {
                var startOptions = new ApiOptions
                {
                    PageSize = 5,
                    PageCount = 1
                };

                var firstPage = await _commitStatusClient.GetAll(owner, name, reference, startOptions).ToList();

                var skipStartOptions = new ApiOptions
                {
                    PageSize = 5,
                    PageCount = 1,
                    StartPage = 2
                };

                var secondPage = await _commitStatusClient.GetAll(owner, name, reference,skipStartOptions).ToList();             

                Assert.NotEqual(firstPage[0].Id, secondPage[0].Id);
                Assert.NotEqual(firstPage[1].Id, secondPage[1].Id);
                Assert.NotEqual(firstPage[2].Id, secondPage[2].Id);
                Assert.NotEqual(firstPage[3].Id, secondPage[3].Id);
                Assert.NotEqual(firstPage[4].Id, secondPage[4].Id);
            }
        }
    }
}
