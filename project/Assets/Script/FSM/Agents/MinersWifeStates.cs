using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FSM
{
	public class MovingTo : State<MinersWife>
	{
		
		public override void Enter(MinersWife MinersWife)
		{
			var locManager = Object.FindObjectOfType<LocationManager>();
			
			//miner.Say(string.Format("Walkin' to {0}", e.Agent.TargetLocation));
			MinersWife.ChangeLocation(locManager.Locations[MinersWife.TargetLocation].position);
			
		}
		
		public override void Execute(MinersWife MinersWife)
		{
			var locManager = Object.FindObjectOfType<LocationManager>();
			
			var target = locManager.Locations[MinersWife.TargetLocation].position;
			
			
			target.y = 0;
			
			if (Vector3.Distance(target, MinersWife.transform.position) <= 3.0f)
			{
				MinersWife.Location = MinersWife.TargetLocation;
				MinersWife.StateMachine.RevertToPreviousState();
			}
			
			
			
		}
		public override void Exit(MinersWife MinersWife)
		{
			//throw new System.NotImplementedException();
		}
		
		public override bool OnMessage(MinersWife MinersWife, Telegram telegram)
		{
			// throw new System.NotImplementedException();
			return true;
		}
		public override bool OnSenseEvent(MinersWife agent, Sense sense)
		{
			return false;
		}
	}


    public class DoHouseWork : State<MinersWife>
    {

        public override void Enter(MinersWife minersWife)
        {
//			minersWife.TargetLocation = Location.shack;
//			
//			if (minersWife.Location != minersWife.TargetLocation)
//			{
//				minersWife.StateMachine.ChangeState(new MovingTo());
//			}
            Debug.Log(minersWife.name + " Time to do some more housework!");
        }

        public override void Execute(MinersWife minersWife)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
				Debug.Log(minersWife.name + " : is cleaning the floor");
                    break;
                case 1:
				Debug.Log(minersWife.name + " : is washing the dishes");
                    break;
                case 2:
				Debug.Log(minersWife.name + " is making the bed");
                    break;
                default:
                    break;
            }
        }

        public override void Exit(MinersWife minersWife)
        {
			Debug.Log(minersWife.name + " stop doing the housework");
        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(MinersWife agent, Sense sense)
		{
			return false;
		}
    }

    public class VisitBathroom : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
//			minersWife.TargetLocation = Location.toilet;
//			
//			if (minersWife.Location != minersWife.TargetLocation)
//			{
//				minersWife.StateMachine.ChangeState(new MovingTo());
//			}
			Debug.Log(minersWife.name + " :is walking to the toilet");
        }

        public override void Execute(MinersWife minersWife)
        {
			Debug.Log(minersWife.name + " Ahhhhhh! Sweet relief!");
            minersWife.StateMachine.RevertToPreviousState();  // this completes the state blip

        }

        public override void Exit(MinersWife minersWife)
        {
			Debug.Log(minersWife.name + " :is leavin' the toilet");
        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(MinersWife agent, Sense sense)
		{
			return false;
		}
    }

    public class CookStew : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
            if (!minersWife.Cooking)
            {
                // MinersWife sends a delayed message to herself to arrive when the food is ready
                Debug.Log(minersWife.name + " :is going to the kitchen");
				Debug.Log(minersWife.name + " :Putting the stew in the oven");
				Message.DispatchMessage(1.5, "Elsa", "Elsa", MessageType.StewsReady);
                minersWife.Cooking = true;
            }
        }

        public override void Execute(MinersWife minersWife)
        {
			Debug.Log(minersWife.name + " : is cooking the stew");
        }

        public override void Exit(MinersWife minersWife)
        {
			Debug.Log(minersWife.name + " Leaving the kitchen");
//			minersWife.Cooking = false;
//			minersWife.StateMachine.ChangeState(new DoHouseWork());
        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HiHoneyImHome:
                    return false;
                case MessageType.StewsReady:
                    // Tell bob that the stew is ready now 
					Debug.Log("Message handled by " + minersWife.name + " at time " + System.DateTime.Now);
					Debug.Log(minersWife.name + " StewReady! Bob come to eat!");
                    Message.DispatchMessage(0, "Elsa", "Bob", MessageType.StewsReady);
					minersWife.Cooking = false;
					minersWife.StateMachine.ChangeState(new DoHouseWork());
				return true;
                default:
                    return false;
            }
        }
		public override bool OnSenseEvent(MinersWife agent, Sense sense)
		{
			return false;
		}

    }

    public class WifesGlobalState : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {

        }

        public override void Execute(MinersWife minersWife)
        {
            // There's always a 10% chance of a state blip in which MinersWife goes to the bathroom
            if (Random.Range(0, 9) == 1 && !minersWife.StateMachine.IsInState(new VisitBathroom()))
            {
                minersWife.StateMachine.ChangeState(new VisitBathroom());
            }
        }

        public override void Exit(MinersWife minersWife)
        {

        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HiHoneyImHome:
					Debug.Log("Message handled by " + minersWife.name + " at time ");
					Debug.Log(minersWife.name + "Hi honey. Let me make you some of mah fine country stew");
                    minersWife.StateMachine.ChangeState(new CookStew());
                    return true;
                case MessageType.StewsReady:
                    return false;
                default:
                    return false;
            }
        }
		public override bool OnSenseEvent(MinersWife agent, Sense sense)
		{
			return false;
		}
    }
}
