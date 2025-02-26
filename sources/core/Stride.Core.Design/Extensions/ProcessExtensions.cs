// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using Stride.Core.Annotations;

namespace Stride.Core.Extensions;

public static class ProcessExtensions
{
    /// <summary>
    /// Waits asynchronously for the process to exit.
    /// </summary>
    /// <param name="process">The process to wait for cancellation.</param>
    /// <param name="cancellationToken">A cancellation token. If invoked, the task will return
    /// immediately as cancelled.</param>
    /// <returns>A Task representing waiting for the process to end.</returns>
    public static Task WaitForExitAsync([NotNull] this Process process, CancellationToken cancellationToken = default)
    {
        // Source: https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why/39872058#39872058
        process.EnableRaisingEvents = true;

        var taskCompletionSource = new TaskCompletionSource();

        process.Exited += handler;
        if (cancellationToken != default)
        {
            cancellationToken.Register(() =>
            {
                process.Exited -= handler;
                taskCompletionSource.TrySetCanceled();
            });
        }

        return taskCompletionSource.Task;

        void handler(object? sender, EventArgs args)
        {
            process.Exited -= handler;
            taskCompletionSource.TrySetResult();
        }
    }
}
