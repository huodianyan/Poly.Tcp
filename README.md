# Poly.Tcp
a lightweight Tcp Server/Client for Unity and any C# (or .Net) project.

## Features
- Zero dependencies
- Minimal core (< 1000 lines)
- Lightweight and fast
- Recieve/Send thread for each connection
- Simple fast byte array pool
- Adapted to all C# game engine

## Installation

## Overview

```csharp

var server = new PolyTcpServer();
server.OnConnectEvent += (connId) => Console.WriteLine($"OnServerConnect: {connId}");
server.OnDisconnectEvent += (connId) => Console.WriteLine($"OnServerDisconnect: {connId}");
server.OnRecieveEvent += (connId, segment) =>
{
    segment.Array[2] = 0x47;
    server.Send(connId, segment);
};
var ok = server.Start(port);

client = new PolyTcpClient();
client.OnConnectEvent += (connId) => Console.WriteLine($"OnClientConnect: {connId}");
client.OnDisconnectEvent += (connId) => Console.WriteLine($"OnClientDisconnect: {connId}");
client.OnRecieveEvent += (connId, segment) => Console.WriteLine($"OnClientRecieve: {connId}, {segment.Count}");

ok = client.Connect(address, port);

byte[] message = new byte[] { 0x42, 0x13, 0x37 };
ok = client.Send(new ArraySegment<byte>(message));

client.Disconnect();

server.Stop();

```

## License
The software is released under the terms of the [MIT license](./LICENSE.md).

## FAQ

## References

### Documents

### Projects
- [vis2k/Telepathy](https://github.com/vis2k/Telepathy)
- [RevenantX/LiteNetLib](https://github.com/RevenantX/LiteNetLib)
- [vis2k/Mirror](https://github.com/vis2k/Mirror)

### Benchmarks
