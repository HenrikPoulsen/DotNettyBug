// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace DotNetty.Tests.End2End
{
    using System;
    using System.Text;
    using DotNetty.Transport.Channels;
    using Message;

    public class FileTransferChannelHandler  : SimpleChannelInboundHandler<FileTransfer>
    {
        //public static bool Faulted = false;
        protected override void ChannelRead0(IChannelHandlerContext ctx, FileTransfer msg)
        {
            ctx.Channel.WriteAndFlushAsync(msg).Wait();
        }
    }
}