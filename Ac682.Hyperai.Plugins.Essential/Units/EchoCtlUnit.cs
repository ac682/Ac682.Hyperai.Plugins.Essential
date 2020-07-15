﻿using Ac682.Hyperai.Plugins.Essential.Services;
using Hyperai.Events;
using Hyperai.Messages;
using Hyperai.Relations;
using Hyperai.Units;
using Hyperai.Units.Attributes;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Extensions;
using HyperaiShell.Foundation.Plugins;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;

namespace Ac682.Hyperai.Plugins.Essential.Units
{
    public class EchoCtlUnit : UnitBase
    {
        private readonly EchoService _service;

        public EchoCtlUnit(EchoService service)
        {
            _service = service;
        }

        [Receive(MessageEventType.Friend)]
        [Extract("!echo.on")]
        public async Task EchoOn(Friend friend)
        {
            _service.On(friend.Identity);
            await friend.SendAsync("Echo on".MakeMessageChain());
        }

        [Receive(MessageEventType.Group)]
        [Extract("!echo.on")]
        public async Task EchoOn(Group group)
        {
            _service.On(group.Identity);
            await group.SendAsync("Echo on".MakeMessageChain());
        }

        [Receive(MessageEventType.Friend)]
        [Extract("!echo.off")]
        public async Task EchoOff(Friend friend)
        {
            _service.Off(friend.Identity);
            await friend.SendAsync("Echo off".MakeMessageChain());
        }

        [Receive(MessageEventType.Group)]
        [Extract("!echo.off")]
        public async Task EchoOff(Group group)
        {
            _service.Off(group.Identity);
            await group.SendAsync("Echo off".MakeMessageChain());
        }

        [Receive(MessageEventType.Group)]
        [Extract("!image")]
        public async Task Image(Group group)
        {
            var builder = new MessageChainBuilder().AddImage(new Uri(@"E:\Pictures\DOAX-VenusVacation\DOAX-VenusVacation_200207_191722.jpg"));
            await group.SendAsync(builder.Build());
        }
    }
}
