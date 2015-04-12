using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSM
{

	public class LurkInOutlawCamp : State<Outlaw>
	{
		//static Random rand = new Random();
		
		public override void Enter(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Back home, sweet home!");
			
			outlaw.BoredomCountdown = Random.Range(1, 10);
		}
		
		public override void Execute(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + "Chilling in " + outlaw.getLocationName(outlaw.TargetLocation) + ".");
			
			if (outlaw.Bored())
			{
				outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(Location.cemetery,new LurkInCemetery()));
			}
		}
		
		public override void Exit(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Leaving the camp.");
		}
		
		public override bool OnMessage(Outlaw agent, Telegram telegram)
		{
			return false;
		}
		
		public override bool OnSenseEvent(Outlaw agent, Sense sense)
		{
			return false;
		}
	}

	public class LurkInCemetery : State<Outlaw>
	{
		static Random rand = new Random();
		
		public override void Enter(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Arrived in the cemetery!");
			outlaw.BoredomCountdown = Random.Range(1, 10);
		}
		
		public override void Execute(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Lurking in the cemetery.");
			if (outlaw.Bored())
			{
				outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(Location.outlawCamp, new LurkInOutlawCamp()));
			}
		}
		
		public override void Exit(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Leaving the cemetery.");
		}
		
		public override bool OnMessage(Outlaw agent, Telegram telegram)
		{
			return false;
		}
		
		public override bool OnSenseEvent(Outlaw agent, Sense sense)
		{
			return false;
		}
	}

	public class AttemptToRobBank : State<Outlaw>
	{
		static Random rand = new Random();
		
		public override void Enter(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Arrived in bank, ROB MONEY!");
		}
		
		public override void Execute(Outlaw outlaw)
		{
			outlaw.GoldCarrying += 5;
			Debug.Log(outlaw.name + ": Money owned now: " + outlaw.GoldCarrying);
			outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(outlaw.StateMachine.PreviousState.GetType() == typeof(LurkInOutlawCamp) ? Location.outlawCamp : Location.cemetery, outlaw.StateMachine.PreviousState));
		}
		
		public override void Exit(Outlaw outlaw)
		{
			Debug.Log(outlaw.name + ": Run away from the bank");
		}
		
		public override bool OnMessage(Outlaw agent, Telegram telegram)
		{
			return false;
		}
		
		public override bool OnSenseEvent(Outlaw agent, Sense sense)
		{
			return false;
		}
	}

	public class DropDeadOutlaw : State<Outlaw>
	{
		
		public override void Enter(Outlaw outlaw)
		{
			outlaw.IsDead = true;
		}
		
		public override void Execute(Outlaw outlaw)
		{
		}
		
		public override void Exit(Outlaw outlaw)
		{
			outlaw.IsDead = false;
			outlaw.Location = Location.outlawCamp;
			
			Debug.Log(outlaw.name + ": Rescued from dead!");
		}
		
		public override bool OnMessage(Outlaw agent, Telegram telegram)
		{
			return false;
		}
		
		public override bool OnSenseEvent(Outlaw agent, Sense sense)
		{
			return false;
		}
	}

//	public class WalkingTo : State<Outlaw>
//	{
//		
//		public override void Enter(Outlaw outlaw)
//		{
//			var locManager = Object.FindObjectOfType<LocationManager>();
//			
//			//miner.Say(string.Format("Walkin' to {0}", e.Agent.TargetLocation));
//			outlaw.ChangeLocation(locManager.Locations[outlaw.TargetLocation].position);
//			
//		}
//		
//		public override void Execute(Outlaw outlaw)
//		{
//			var locManager = Object.FindObjectOfType<LocationManager>();
//			
//			var target = locManager.Locations[outlaw.TargetLocation].position;
//
//			target.y = 0;
//			
//			if (Vector3.Distance(target, outlaw.transform.position) <= 3.0f)
//			{
//				outlaw.Location = outlaw.TargetLocation;
//				outlaw.StateMachine.RevertToPreviousState();
//			}
//		}
//		public override void Exit(Outlaw outlaw)
//		{
//		}
//		
//		public override bool OnMessage(Miner agent, Telegram telegram)
//		{
//			return false;
//		}
//		
//	}

	public class OutlawTravelToTarget : TravelToTarget<Outlaw>
	{
		public OutlawTravelToTarget(Location loc, State<Outlaw> state)
		{
			State<Outlaw> targetState = state;
			targetLoc = loc;
		}

		public override void Enter(Outlaw outlaw)
		{
			outlaw.TargetLocation = targetLoc;
			var locManager = Object.FindObjectOfType<LocationManager>();
			outlaw.ChangeLocation(locManager.Locations[outlaw.TargetLocation].position);
			Debug.Log(outlaw.name+ ": Walkin' to " + outlaw.getLocationName(outlaw.TargetLocation)  + ".");
		}
		
		public override void Execute(Outlaw outlaw)
		{

			var locManager = Object.FindObjectOfType<LocationManager>();
			var target = locManager.Locations[outlaw.TargetLocation].position;
			target.y = 0;
			
			if (Vector3.Distance(target, outlaw.transform.position) <= 3.0f)
			{
				outlaw.Location = outlaw.TargetLocation;
				outlaw.StateMachine.ChangeState(targetState);
			}
		}
		
		public override void Exit(Outlaw outlaw)
		{
		}
		
		public override bool OnMessage(Outlaw agent, Telegram telegram)
		{
			return false;
		}
		
		public override bool OnSenseEvent(Outlaw agent, Sense sense)
		{
			return false;
		}
	}

	public class OutlawGlobalState : State<Outlaw>
	{
		static Random rand = new Random();
		
		public override void Enter(Outlaw outlaw)
		{
		}
		
		public override void Execute(Outlaw outlaw)
		{
			if (!outlaw.IsDead)
			{
				if (outlaw.StateMachine.CurrentState.GetType() != typeof(OutlawTravelToTarget))
				{
					if (Random.Range(0,20) == 1 && !outlaw.StateMachine.IsInState(new AttemptToRobBank()))
					{
						outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(Location.bank, new AttemptToRobBank()));
					}
				}
			}
		}
		
		public override void Exit(Outlaw outlaw)
		{
		}
		
		public override bool OnMessage(Outlaw outlaw, Telegram telegram)
		{
			
			switch (telegram.messageType)
			{
			case MessageType.SheriffEncountered:
				Debug.Log(outlaw.name+ "Prepare to die instead, sheriff!");
				Message.DispatchMessage(0, outlaw.name, telegram.Sender, MessageType.Gunfight);
				return true;
			case MessageType.Dead:
				outlaw.StateMachine.ChangeState(new DropDeadOutlaw());
				return true;
			case MessageType.Respawn:
				outlaw.StateMachine.ChangeState(new LurkInOutlawCamp());
				return true;
			}
			return false;
		}
		
		public override bool OnSenseEvent(Outlaw agent, Sense sense)
		{
			return false;
		}
	}
}
