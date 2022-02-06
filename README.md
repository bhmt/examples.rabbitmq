# .NET5 RabbitMQ basic working example

## Description

The solution contains of a .NET5 Hosted Service and RabbitMQ.

The `bhmt.mq.consumer` project represents a messaging queue.
It is run as a BackgroundService. It will read a recieved message and print it to console.

The `bhmt.mq.producer` project is used to send messages when calling an endpoint at `/api/Producer/Send`.


## Usage

Use makefile to run the Consumer, and then Producer in another terminal

```bash
$ make consumer
$ make producer
```

After the producer is running use curl to call

```bash
$ curl -X POST localhost:5200/api/Producer/Send -d ''
```

Or open browser at `localhost:5200/swagger` to try the endpoint call.


## BackgroundService

The .NET5 Background service implements IHostedService with three methods - `StartAsync, ExecuteAsync, StopAsync`.

The `StartAsync` is used to create and set the connection and channel.

The `ExecuteAsync` defines the work that needs to be done when recieveing a message.
A CancellationToken should signal that the work related to a message is done or cancelled, and the next message should be used.
In the service implementation, a while loop is used to check if the this signal is sent.

The `StopAsync` can be uset to gracefully shutdown the service.
