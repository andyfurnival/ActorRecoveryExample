using System;
using System.IO;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using Akka.Event;
using Akka.Persistence;
using Akka.Util.Internal;

namespace ActorRecoveryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ConfigurationFactory.ParseString(File.ReadAllText("HOCON"))
          
                .WithFallback(Persistence.DefaultConfig());
            
            var sys = ActorSystem.Create("test", cfg);
            var cluster1 = Cluster.Get(sys);

            Console.WriteLine("akka://test@{0}:{1}",cluster1.SelfAddress.Host, cluster1.SelfAddress.Port);
            
            cluster1.Join(Address.Parse($"akka.tcp://test@{cluster1.SelfAddress.Host}:18000"));
            cluster1.RegisterOnMemberUp(()=> 
            {
                var actor = sys.ActorOf(Props.Create(()=> new ParentActor()), "parent");
            
                actor.Tell("Test Message 1");
                actor.Tell("Test Message 2");
                actor.Tell(true);
                Thread.Sleep(500);
            
                actor.Tell("Test Message 3");
            });
           
            Console.Read();
        }
    }

    public class ParentActor : ReceivePersistentActor
    {
        public ParentActor()
        {
            PersistenceId = "parent";
            Recover<string>(msg =>
            {
                Context.GetLogger().Info("[{1}] Recovery: {0}", msg, nameof(ParentActor));
                CreateChildActor();

            });
            Recover<bool>(msg =>
            {
                Context.GetLogger().Info("[{1}] Recovery: {0}", msg, nameof(ParentActor));

            });
            Recover<RecoveryCompleted>(_ =>
            {
                Become(ProcessMessages);
            });
        }

       
        private void ProcessMessages()
        {
            Command<string>(msg =>
            {
                Persist(msg, persisted =>
                {
                    Context.GetLogger().Info("[{1}] Command: {0}", msg, nameof(ParentActor));
                    var child = CreateChildActor();
                    child.Tell(msg);
                });
            });
            Command<bool>(msg =>
            {
                Persist(msg, persisted =>
                {
                    Context.GetLogger().Info("[{1}] Command: {0}", msg, nameof(ParentActor));
                    Context.GetChildren().ForEach(child => child.Tell(false));
                    Context.GetChildren().ForEach(child => child.Tell(PoisonPill.Instance));
                });
            });
        }

        private static IActorRef CreateChildActor()
        {
            var name = (Context.GetChildren().Count() + 1).ToString();

            var child = Context.ActorOf(Props.Create(() =>
                new ChildActor(name)), name);
            return child;
        }


        public override string PersistenceId { get; }
    }

    public class ChildActor : ReceivePersistentActor
    {
        public ChildActor(string ID)
        {
            PersistenceId = "child" + ID;
            Recover<string>(msg =>
            {
                Context.GetLogger().Info("[{1}] Recovery: {0}", msg, nameof(ChildActor));
               
            });
            
            Recover<RecoveryCompleted>(_ =>
            {
                Become(ProcessMessages);
            });
            
            
        }

        private void ProcessMessages()
        {
            Command<string>(msg =>
            {
                Persist(msg, persisted =>
                {
                    Context.GetLogger().Info("[{1}] Command: {0}", msg, nameof(ChildActor));
                });
                
            });
            
            Command<bool>(msg =>
            {
                Context.GetLogger().Info("[{1}] Command: {0}", msg, nameof(ChildActor));
                Context.GetChildren().ForEach(child => child.Tell(PoisonPill.Instance));
            });
        }
       
        
        public override string PersistenceId { get; }
    }
}