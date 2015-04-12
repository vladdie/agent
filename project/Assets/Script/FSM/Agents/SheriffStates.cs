using UnityEngine;
using System.Collections;


namespace FSM
{
    public class PatrolRandomLocation : State<Sheriff>
    {
       // static Random rand = new Random();

        public override void Enter(Sheriff sheriff)
        {
			//Debug.Log(sheriff.name+ ": is Arrived!");
        }

        public override void Execute(Sheriff sheriff)
        {
			Debug.Log(sheriff.name+ ": is Patrolling in " + sheriff.getLocationName(sheriff.TargetLocation) + ".");

            if (!sheriff.OutlawSpotted)
            {
				sheriff.TargetLocation = sheriff.ChooseNextLocation();
				Debug.Log (sheriff.name+": Patrol over. Next location is "+ sheriff.getLocationName(sheriff.TargetLocation));
				sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(sheriff.TargetLocation, new PatrolRandomLocation()));

			}
		}

        public override void Exit(Sheriff sheriff)
        {
			//Debug.Log(sheriff.name+ ": is Leaving " + sheriff.getLocationName(sheriff.TargetLocation)  + ".");
        }

        public override bool OnMessage(Sheriff agent, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Sheriff sheriff, Sense sense)
        {
            return false;
        }
    }

    // In this state, the sheriff goes to the bank and deposits gold
    public class StopByBankAndDepositGold : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
            Debug.Log(sheriff.name+"Arrived in bank.");
        }

        public override void Execute(Sheriff sheriff)
        {
            sheriff.MoneyInBank += sheriff.goldCarrying;
            sheriff.goldCarrying = 0;
            Debug.Log(sheriff.name+"Depositing gold. Total savings now: " + sheriff.MoneyInBank);

            sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(Location.saloon, new CelebrateTheDayInSaloon()));
        }

        public override void Exit(Sheriff sheriff)
        {
           // Debug.Log(sheriff.name+"Leaving the Bank, time to celebrate!");
        }

        public override bool OnMessage(Sheriff agent, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Sheriff agent, Sense sense)
        {
            return false;
        }
    }

    // In this state, the sheriff goes to the bank and deposits gold
    public class CelebrateTheDayInSaloon : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
            Debug.Log(sheriff.name+"Arrived in the saloon!");
        }

        public override void Execute(Sheriff sheriff)
        {
            Debug.Log(sheriff.name+"All drinks on me today! Kidding!! Haha");

            sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(sheriff.ChooseNextLocation(), new PatrolRandomLocation()));
        }

        public override void Exit(Sheriff sheriff)
        {
           // Debug.Log(sheriff.name+"Leaving the saloon.");
        }

        public override bool OnMessage(Sheriff agent, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Sheriff agent, Sense sense)
        {
            return false;
        }
    }


    public class DropDeadSheriff : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
            Debug.Log(sheriff.name+"Goodbye, cruel world!");
            sheriff.IsDead = true;
        }

        public override void Execute(Sheriff sheriff)
        {
        }

        public override void Exit(Sheriff sheriff)
        {
            sheriff.IsDead = false;
            sheriff.Location = Location.sheriffsOffice;

            Debug.Log(sheriff.name+"It's a miracle, I am alive!");
        }

        public override bool OnMessage(Sheriff agent, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Sheriff agent, Sense sense)
        {
            return false;
        }
    }

    public class SheriffTravelToTarget : TravelToTarget<Sheriff>
    {
		public SheriffTravelToTarget(Location loc,State<Sheriff> state)
        {
            targetState = state;
			targetLoc = loc;
        }

        public override void Enter(Sheriff sheriff)
        {
			sheriff.TargetLocation = targetLoc;
			var locManager = Object.FindObjectOfType<LocationManager>();

			Debug.Log(sheriff.name+ ": Walkin' to " + sheriff.getLocationName(sheriff.TargetLocation)  + ".");
			sheriff.ChangeLocation(locManager.Locations[sheriff.TargetLocation].position);
			Debug.Log("*************************************");
        }

        public override void Execute(Sheriff sheriff)
        {
			var locManager = Object.FindObjectOfType<LocationManager>();
			var target = locManager.Locations[sheriff.TargetLocation].position;
			target.y = 0;
			Debug.Log("##################################################");
			if (Vector3.Distance(target, sheriff.transform.position) <= 5.0f)
			{
				sheriff.Location = sheriff.TargetLocation;
				sheriff.StateMachine.ChangeState(targetState);
			}
        }

        public override void Exit(Sheriff sheriff)
        {
			Debug.Log(sheriff.name+": reached the target location!");
        }

        public override bool OnMessage(Sheriff agent, Telegram telegram)
        {
            return false;
        }

        public override bool OnSenseEvent(Sheriff agent, Sense sense)
        {
            return false;
        }
    }

    // If the agent has a global state, then it is executed every Update() cycle
    public class SheriffGlobalState : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
        }

        public override void Execute(Sheriff sheriff)
        {
        }

        public override void Exit(Sheriff sheriff)
        {

        }

        public override bool OnMessage(Sheriff sheriff, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.Gunfight:
                    // Notify the undertaker
					Message.DispatchMessage(0, sheriff.name, "OutLaw", MessageType.Gunfight);
					var agentManager = Object.FindObjectOfType<AgentManager>();
                    // Gunfight
					var outlaw = agentManager.GetAgent("OutLaw");

					if (Random.Range(0,10) == 1) // sheriff dies
						{
						
							outlaw.goldCarrying += sheriff.GoldCarrying;
							sheriff.GoldCarrying = 0;
							
							Message.DispatchMessage(0, sheriff.name, sheriff.name, MessageType.Dead);
						}
						else // outlaw dies
						{
							Debug.Log(sheriff.name+"I am not coward, but I am so strong. It is hard to die.");
						
							sheriff.goldCarrying += outlaw.goldCarrying;
							outlaw.goldCarrying = 0;
						
							Message.DispatchMessage(0, sheriff.name, outlaw.name, MessageType.Dead);
						
							sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(Location.bank, new StopByBankAndDepositGold()));
						}

                    sheriff.OutlawSpotted = false;

                    return true;
                case MessageType.Dead:
                    sheriff.StateMachine.ChangeState(new DropDeadSheriff());
                    return true;
                case MessageType.Respawn:
                    sheriff.StateMachine.ChangeState(new PatrolRandomLocation());
                    return true;
                default:
                    return false;
            }
        }

        public override bool OnSenseEvent(Sheriff sheriff, Sense sense)
        {
			var agentManager = Object.FindObjectOfType<AgentManager>();
            if (!sheriff.IsDead)
            {
				if ("OutLaw" == agentManager.GetAgent(sense.Sender).name) // outlaw spotted
                {
					if (!sheriff.OutlawSpotted && !agentManager.GetAgent(sense.Sender).IsDead)
                    {
                        Debug.Log(sheriff.name+"Sure glad to see you bandit, but hand me those guns.");
                        sheriff.OutlawSpotted = true;
                        Message.DispatchMessage(0, sheriff.name, sense.Sender, MessageType.SheriffEncountered);

                        return true;
                    }
                }
                else // greetings
                {
                    Debug.Log(sheriff.name+"Good day, townie!");
                    Message.DispatchMessage(0, sheriff.name, sense.Sender, MessageType.SheriffEncountered);

                    return true;
                }
            }

            return false;
        }
    }
}
