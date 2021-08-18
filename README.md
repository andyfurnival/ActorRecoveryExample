# ActorRecoveryExample

Needs a local instance of Redis
1st load
The parent actor is created explictly by code, there is no persistence to recover
[INFO][08/18/2021 15:37:02][Thread 0033][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: Test Message 1
[INFO][08/18/2021 15:37:02][Thread 0055][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: Test Message 2
Parent actor receiving messages triggers the creation of child actors and send them the same message
[INFO][08/18/2021 15:37:02][Thread 0035][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Command: Test Message 1
[INFO][08/18/2021 15:37:02][Thread 0031][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: True
[INFO][08/18/2021 15:37:02][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent/2] [ChildActor] Command: Test Message 2
[INFO][08/18/2021 15:37:02][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent/2] [ChildActor] Command: False
[INFO][08/18/2021 15:37:02][Thread 0035][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Command: False
[INFO][08/18/2021 15:37:02][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: Test Message 3
[INFO][08/18/2021 15:37:02][Thread 0034][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Recovery: Test Message 1
[INFO][08/18/2021 15:37:02][Thread 0035][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Command: Test Message 3


2nd load

The parent actor is created explictly by code, which triggers the recovery
[INFO][08/18/2021 15:30:31][Thread 0031][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Recovery: Test Message 1
[INFO][08/18/2021 15:30:31][Thread 0031][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Recovery: Test Message 2
[INFO][08/18/2021 15:30:31][Thread 0031][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Recovery: True
[INFO][08/18/2021 15:30:31][Thread 0031][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Recovery: Test Message 3

The parent actor starts receiving messages (non-recovery)
[INFO][08/18/2021 15:30:31][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: Test Message 1
[INFO][08/18/2021 15:30:31][Thread 0016][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: Test Message 2

These commands to the parent actor, is what starts the child actors and triggers recovery
[INFO][08/18/2021 15:30:31][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Recovery: Test Message 1

THe parent then send the child actors messages
[INFO][08/18/2021 15:30:31][Thread 0055][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Command: Test Message 1
[INFO][08/18/2021 15:30:31][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent/2] [ChildActor] Recovery: Test Message 2
[INFO][08/18/2021 15:30:31][Thread 0016][akka.tcp://test@0.0.0.0:18000/user/parent/2] [ChildActor] Command: Test Message 2
[INFO][08/18/2021 15:30:31][Thread 0032][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: True
[INFO][08/18/2021 15:30:31][Thread 0015][akka.tcp://test@0.0.0.0:18000/user/parent/2] [ChildActor] Command: False
[INFO][08/18/2021 15:30:31][Thread 0016][akka.tcp://test@0.0.0.0:18000/user/parent/1] [ChildActor] Command: False
[INFO][08/18/2021 15:30:31][Thread 0032][akka.tcp://test@0.0.0.0:18000/user/parent] [ParentActor] Command: Test Message 3
[INFO][08/18/2021 15:30:31][Thread 0032][akka.tcp://test@0.0.0.0:18000/user/parent/3] [ChildActor] Recovery: Test Message 3
[INFO][08/18/2021 15:30:31][Thread 0032][akka.tcp://test@0.0.0.0:18000/user/parent/3] [ChildActor] Command: Test Message 3
