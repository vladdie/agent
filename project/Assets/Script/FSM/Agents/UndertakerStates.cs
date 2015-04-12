using UnityEngine;
using System.Collections;


namespace FSM
{
    public class HoverInTheOffice : State<Undertaker>
    {

        public override void Enter(Undertaker undertaker)
        {
            Debug.Log(undertaker.name+ "Arrived in the office!");
        }

        public override void Execute(Undertaker undertaker)
        {
            Debug.Log(undertaker.name+ "Hovering in the office.");
        }

        public override void Exit(Undertaker undertaker)
        {
            Debug.Log(undertaker.name+ "Leaving the office.");
        }

        public override bool OnMessage(Undertaker undertaker, Telegram telegram)
        {
			var agentManager = Object.FindObjectOfType<AgentManager>();
            switch (telegram.messageType)
            {
                case MessageType.Gunfight:
                    Debug.Log(undertaker.name+ "Let's get down to business!");
					undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(agentManager.GetAgent(telegram.Sender).transform.position, new LookForDeadBodies()));

                    return true;
                default:
                    return false;
            }
        }

        public override bool OnSenseEvent(Undertaker agent, Sense sense)
        {
            return false;
        }
    }

    public class LookForDeadBodies : State<Undertaker>
    {
        public override void Enter(Undertaker undertaker)
        {
			Debug.Log(undertaker.name+ "Arrived in " + undertaker.getLocationName(undertaker.TargetLocation) + ".");
        }

        public override void Execute(Undertaker undertaker)
        {
			var agentManager = Object.FindObjectOfType<AgentManager>();
			for (int i = 0; i < agentManager.listOfAgents.Count; ++i)
            {
				if (agentManager.GetAgent(i).IsDead)
                {
					if (undertaker.transform.position != agentManager.GetAgent(i).transform.position)
                    {
						undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(agentManager.GetAgent(i).transform.position, new LookForDeadBodies()));
                        return;
                    }
                    undertaker.CorpseID = i;
                }
            }

			Debug.Log(undertaker.name+ "Found the corpse of " + agentManager.GetAgent(undertaker.CorpseID).name + ".");

            if (undertaker.CorpseID >= 0)
            {
				var locManager = Object.FindObjectOfType<LocationManager>();
				undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(locManager.Locations[Location.cemetery].position, new DragOffTheBody()));
            }
        }

        public override void Exit(Undertaker undertaker)
        {
            if (undertaker.Location != Location.cemetery)
            {
				Debug.Log(undertaker.name+ "Leaving " + undertaker.getLocationName(undertaker.TargetLocation) + ".");
            }
        }

        public override bool OnMessage(Undertaker undertaker, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Undertaker agent, Sense sense)
        {
            return false;
        }
    }

    public class DragOffTheBody : State<Undertaker>
    {
        public override void Enter(Undertaker undertaker)
        {
            Debug.Log(undertaker.name+ "Carrying the body to the tombs in the cemetery!");
        }

        public override void Execute(Undertaker undertaker)
        {
            Debug.Log(undertaker.name+ "Dragging the body off. . . R.I.P.");
			var agentManager = Object.FindObjectOfType<AgentManager>();
			string deadBody = agentManager.GetAgent(undertaker.CorpseID).name;
			Message.DispatchMessage(10, undertaker.name, deadBody, MessageType.Respawn);
            undertaker.CorpseID = -1;
			var locManager = Object.FindObjectOfType<LocationManager>();
			undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(locManager.Locations[Location.cemetery].position, new HoverInTheOffice()));
        }

        public override void Exit(Undertaker undertaker)
        {
            Debug.Log(undertaker.name+ "Leaving the cemetery");
        }

        public override bool OnMessage(Undertaker undertaker, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Undertaker agent, Sense sense)
        {
            return false;
        }
    }

    public class UndertakerTravelToTarget : TravelToTarget<Undertaker>
    {
        
		public UndertakerTravelToTarget(Vector3 target,State<Undertaker> state)
		{
			State<Undertaker> targetState = state;
			targetPos = target;
		}

        public override void Enter(Undertaker undertaker)
        {
			undertaker.ChangeLocation(targetPos);
			Debug.Log(undertaker.name+ ": Walkin' to " + undertaker.getLocationName(undertaker.TargetLocation) + ".");
        }

        public override void Execute(Undertaker undertaker)
        {
			targetPos.y = 0;
			
			if (Vector3.Distance(targetPos, undertaker.transform.position) <= 3.0f)
			{
				undertaker.Location = undertaker.TargetLocation;
				undertaker.StateMachine.ChangeState(targetState);
			}

            if (undertaker.CorpseID >= 0)//there is dead body
            {
				var agentManager = Object.FindObjectOfType<AgentManager>();
				agentManager.GetAgent(undertaker.CorpseID).transform.position = undertaker.transform.position;
            }
        }

        public override void Exit(Undertaker undertaker)
        {
        }

        public override bool OnMessage(Undertaker agent, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Undertaker agent, Sense sense)
        {
            return false;
        }
    }


    // If the agent has a global state, then it is executed every Update() cycle
    public class UndertakerGlobalState : State<Undertaker>
    {

        public override void Enter(Undertaker undertaker)
        {
        }

        public override void Execute(Undertaker undertaker)
        {
        }

        public override void Exit(Undertaker undertaker)
        {

        }

        public override bool OnMessage(Undertaker undertaker, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.SheriffEncountered:
                    Debug.Log(undertaker.name+ "Thank you sheriff, any 'sad' news?");
                    return true;
                default:
                    return false;
            }
        }

        public override bool OnSenseEvent(Undertaker agent, Sense sense)
        {
            return false;
        }
    }
}
