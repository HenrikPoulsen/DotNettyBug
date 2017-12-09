// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Protobuf;
using DotNetty.Common.Concurrency;
using DotNetty.Tests.Common;
using DotNetty.Tests.End2End;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Embedded;
using DotNetty.Transport.Channels.Sockets;
using Google.Protobuf;
using Message;
using Xunit;
using Xunit.Abstractions;

namespace DotNettyBug
{
    public class End2EndTests
    {
        private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        private readonly ITestOutputHelper _output;

        public End2EndTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        private static readonly Random Random = new Random(0);

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static IEnumerable<object[]> GetMessageLength()
        {
            //32 * 1024 * 1024
            var length = 32;
            int port = 8100;
            for (var i = 0; i < 6; ++i)
            {
                yield return new object[] {length, port + i};
                length = length * 2;
            }
        }
        
        [Theory]
        [MemberData(nameof(GetMessageLength))]
        public async Task LargeProtobufMessage_TcpChannel(int messageLength, int port)
        {
            var closeServerFunc = await StartServerAsync(true, port,ch =>
            {
                ch.Pipeline.AddLast(new ProtobufVarint32FrameDecoder());
                ch.Pipeline.AddLast(new ProtobufDecoder(FileTransfer.Parser));
                ch.Pipeline.AddLast(new ProtobufVarint32LengthFieldPrepender());
                ch.Pipeline.AddLast(new ProtobufEncoder());

                ch.Pipeline.AddLast(new FileTransferChannelHandler());
            });

            var group = new MultithreadEventLoopGroup();
            var readListener = new ReadListeningHandler(DefaultTimeout);
            Bootstrap b = new Bootstrap()
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(ch =>
                {
                    ch.Pipeline.AddLast(new ProtobufVarint32FrameDecoder());
                    ch.Pipeline.AddLast(new ProtobufDecoder(FileTransfer.Parser));
                    ch.Pipeline.AddLast(new ProtobufVarint32LengthFieldPrepender());
                    ch.Pipeline.AddLast(new ProtobufEncoder());
                    ch.Pipeline.AddLast(readListener);
                }));

            _output.WriteLine("Configured Bootstrap: {0}", b);
            
            var message = RandomString(messageLength * 1024);

            string hash;
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                hash = BitConverter.ToString(
                    md5.ComputeHash(Encoding.UTF8.GetBytes(message))
                ).Replace("-", string.Empty);
            }
            var fileTransferMessage = new FileTransfer
            {
                FileChecksum = hash,
                Payload = message
            };

            FileTransfer responseMessage = null;

            IChannel clientChannel = null;
            try
            {
                clientChannel = await b.ConnectAsync(IPAddress.Loopback, port);

                _output.WriteLine("Connected channel: {0}", clientChannel);

                
                await clientChannel.WriteAndFlushAsync(fileTransferMessage).WithTimeout(DefaultTimeout);

                responseMessage = Assert.IsAssignableFrom<FileTransfer>(await readListener.ReceiveAsync());
            }
            catch (Exception e)
            {
                _output.WriteLine($"{e}");
            }
            finally
            {
                var serverCloseTask = closeServerFunc();
                clientChannel?.CloseAsync().Wait(TimeSpan.FromSeconds(5));
                await group.ShutdownGracefullyAsync();
                if (!serverCloseTask.Wait(ShutdownTimeout))
                {
                    _output.WriteLine("Didn't stop in time.");
                }
            }
            
            Assert.Equal(fileTransferMessage.FileChecksum, responseMessage?.FileChecksum);
            Assert.Equal(fileTransferMessage.Payload, responseMessage?.Payload);
        }
        
        /// <summary>
        ///     Starts Echo server.
        /// </summary>
        /// <returns>function to trigger closure of the server.</returns>
        async Task<Func<Task>> StartServerAsync(bool tcpNoDelay, int port, Action<IChannel> childHandlerSetupAction)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var started = false;
            try
            {
                var b = new ServerBootstrap()
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(childHandlerSetupAction))
                    .ChildOption(ChannelOption.TcpNodelay, tcpNoDelay);

                _output.WriteLine("Configured ServerBootstrap: {0}", b);

                var serverChannel = await b.BindAsync(port);

                _output.WriteLine("Bound server channel: {0}", serverChannel);

                started = true;

                return async () =>
                {
                    try
                    {
                        await serverChannel.CloseAsync();
                    }
                    catch (Exception e)
                    {
                        _output.WriteLine($"{e}");
                    }
                    finally
                    {
                        await bossGroup.ShutdownGracefullyAsync();
                        await workerGroup.ShutdownGracefullyAsync();
                    }
                };
            }
            catch (Exception e)
            {
                _output.WriteLine($"{e}");
                throw;
            }
            finally
            {
                if (!started)
                {
                    await bossGroup.ShutdownGracefullyAsync();
                    await workerGroup.ShutdownGracefullyAsync();
                }
            }
        }
        
        [Theory]
        [MemberData(nameof(GetMessageLength))]
        public void LargeProtobufMessage_EmbeddedChannel(int messageLength, int dummy)
        {
            var channel = new EmbeddedChannel(
                new ProtobufVarint32FrameDecoder(),
                new ProtobufDecoder(FileTransfer.Parser),
                new ProtobufVarint32LengthFieldPrepender(),
                new ProtobufEncoder());
            var message = RandomString(messageLength);
            string hash;
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                hash = BitConverter.ToString(
                    md5.ComputeHash(Encoding.UTF8.GetBytes(message))
                ).Replace("-", string.Empty);
            }
            var fileTransfer = new FileTransfer
            {
                FileChecksum = hash,
                Payload = message
            };
            Assert.True(channel.WriteOutbound(fileTransfer));
            var buffer = channel.ReadOutbound<IByteBuffer>();
            Assert.NotNull(buffer);
            Assert.True(buffer.ReadableBytes > 0);

            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);

            var inputBuffer = Unpooled.WrappedBuffer(data);
            

            Assert.True(channel.WriteInbound(inputBuffer));

            var messageInbound = channel.ReadInbound<IMessage>();
            Assert.NotNull(message);
            Assert.IsType<FileTransfer>(messageInbound);
            var roundTripped = (FileTransfer)messageInbound;

            Assert.Equal(fileTransfer.FileChecksum, roundTripped.FileChecksum);
            Assert.Equal(fileTransfer.Payload, roundTripped.Payload);

            Assert.False(channel.Finish());
        }
    }
}