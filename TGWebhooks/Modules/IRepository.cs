﻿using Octokit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TGWebhooks.Modules
{
	/// <summary>
	/// Represents the git repository being manager by the <see cref="TGWebhooks"/>
	/// </summary>
    public interface IRepository : IInitializable
    {
		/// <summary>
		/// The path to the <see cref="IRepository"/>
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Restricts usage of the repo to the current call stack
		/// </summary>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation</param>
		/// <returns>A <see cref="Task{TResult}"/> resulting in a <see cref="SemaphoreSlimContext"/> indicating ownership of the <see cref="IRepository"/></returns>
		Task<SemaphoreSlimContext> LockToCallStack(CancellationToken cancellationToken);

		/// <summary>
		/// Fetches the given <paramref name="pullRequest"/> into the <see cref="IRepository"/> and returns the proper HEAD commit. If the <see cref="PullRequest.Commits"/> count is more than one it will be squashed. Must be done in the returned <see cref="SemaphoreSlimContext"/> of <see cref="LockToCallStack(CancellationToken)"/>
		/// </summary>
		/// <param name="pullRequest">The <see cref="PullRequest"/> to fetch</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation</param>
		/// <returns>A <see cref="Task{TResult}"/> resulting in the see SHA to base further work off of</returns>
		Task<string> CreatePullRequestWorkingCommit(PullRequest pullRequest, CancellationToken cancellationToken);

		/// <summary>
		/// Commits changes to the current working directory. Must be done in the returned <see cref="SemaphoreSlimContext"/> of <see cref="LockToCallStack(CancellationToken)"/>
		/// </summary>
		/// <param name="pathsToStage">The paths in the <see cref="IRepository"/> to stage</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation</param>
		/// <returns>A <see cref="Task{TResult}"/> resulting in the created commit SHA</returns>
		Task<string> CommitChanges( IEnumerable<string> pathsToStage, CancellationToken cancellationToken);

		/// <summary>
		/// Pushes a <paramref name="commit"/> to a specified <paramref name="remote"/> <paramref name="branch"/>, optionally <paramref name="force"/>ing it. Must be done in the returned <see cref="SemaphoreSlimContext"/> of <see cref="LockToCallStack(CancellationToken)"/>
		/// </summary>
		/// <param name="remote">The git remote to push to</param>
		/// <param name="branch">The branch on <paramref name="remote"/> to push to</param>
		/// <param name="commit">The SHA of the commit to push</param>
		/// <param name="token">The token to use in place of a password for pushing</param>
		/// <param name="force">If <see langword="true"/> the push will be done as a force push</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation</param>
		/// <returns>A <see cref="Task"/> representing the running operation</returns>
		Task Push(string remote, string branch, string commit, string token, bool force, CancellationToken cancellationToken);
	}
}
