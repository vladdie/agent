using UnityEngine;
using System.Collections;

namespace FSM
{

    public class WalkingTo : State<Miner>
    {

        public override void Enter(Miner miner)
        {
            var locManager = Object.FindObjectOfType<LocationManager>();

            //miner.Say(string.Format("Walkin' to {0}", e.Agent.TargetLocation));
            miner.ChangeLocation(locManager.Locations[miner.TargetLocation].position);

        }

        public override void Execute(Miner miner)
        {
            var locManager = Object.FindObjectOfType<LocationManager>();

            var target = locManager.Locations[miner.TargetLocation].position;


            target.y = 0;

            if (Vector3.Distance(target, miner.transform.position) <= 2.0f)
            {
                miner.Location = miner.TargetLocation;
                miner.StateMachine.RevertToPreviousState();
			}else{

			}



        }
        public override void Exit(Miner agent)
        {
            //throw new System.NotImplementedException();
        }

        public override bool OnMessage(Miner agent, Telegram telegram)
        {
            // throw new System.NotImplementedException();
            return true;
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }



    public class EnterMineAndDigForNugget : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Debug.Log(miner.name + " Walkin' to the goldmine");
            //var goldMinePosition = GameObject.FindGameObjectWithTag("Mine");
            miner.TargetLocation = Location.goldMine;

            if (miner.Location != miner.TargetLocation)
            {
                miner.StateMachine.ChangeState(new WalkingTo());
            }

        }

        public override void Execute(Miner miner)
        {
            miner.GoldCarrying += 1;
            miner.HowFatigued += 1;
            Debug.Log(miner.name + " Pickin' up a nugget");

            if (miner.PocketsFull())
            {
                miner.StateMachine.ChangeState(new VisitBankAndDepositGold());
            }

            if (miner.Thirsty())
            {
                miner.StateMachine.ChangeState(new QuenchThirst());
            }
        }

        public override void Exit(Miner miner)
        {
            if (miner.Location == miner.TargetLocation)
                Debug.Log(miner.name + "Ah'm leaving the gold mine with mah pockets full o' sweet gold");
        }

        public override bool OnMessage(Miner agent, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }

    // In this state, the miner goes to the bank and deposits gold
    public class VisitBankAndDepositGold : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Debug.Log(miner.name + " Goin' to the bank. Yes siree");

            miner.TargetLocation = Location.bank;

            if (miner.Location != miner.TargetLocation)
            {
                miner.StateMachine.ChangeState(new WalkingTo());
            }
        }

        public override void Execute(Miner miner)
        {
            miner.MoneyInBank += miner.GoldCarrying;
            miner.GoldCarrying = 0;
            Debug.Log(miner.name + " Depositing gold. Total savings now: " + miner.MoneyInBank);
            if (miner.Rich())
            {
                Debug.Log(miner.name + " WooHoo! Rich enough for now. Back home to mah li'lle lady");
                miner.StateMachine.ChangeState(new GoHomeAndSleepTillRested());
            }
            else
            {
                miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
            }
        }

        public override void Exit(Miner miner)
        {
            Debug.Log(miner.name + " : is leaving the Bank");
        }

        public override bool OnMessage(Miner agent, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }

    // In this state, the miner goes home and sleeps
    public class GoHomeAndSleepTillRested : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Debug.Log(miner.name + " is walking home");
            miner.TargetLocation = Location.shack;

            if (miner.Location != miner.TargetLocation)
            {
                miner.StateMachine.ChangeState(new WalkingTo());
            }
			else{
				Message.DispatchMessage(0, "Bob", "Elsa", MessageType.HiHoneyImHome);
				//Debug.Log("HiHoneyImhome");
			}
                
        }

        public override void Execute(Miner miner)
        {
            if (miner.HowFatigued < miner.TirednessThreshold)
            {
                Debug.Log(miner.name + " Feel good! Time to find more gold!");
                miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
            }
            else
            {
                miner.HowFatigued--;
                Debug.Log(miner.name + " zzzzzzzzzZZZZZZZZZZZ....");
            }
        }

        public override void Exit(Miner miner)
        {

        }

        public override bool OnMessage(Miner miner, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HiHoneyImHome:
                    return false;
                case MessageType.StewsReady:
                    Debug.Log("Message handled by " + miner.name + " at time ");
                    Debug.Log(miner.name + " Okay I'm coming to resturant'!");
                    miner.StateMachine.ChangeState(new EatStew());
                    return true;
                default:
                    return false;
            }
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }

    // In this state, the miner goes to the saloon to drink
    public class QuenchThirst : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            miner.TargetLocation = Location.saloon;

            if (miner.Location != miner.TargetLocation)
            {
                miner.StateMachine.ChangeState(new WalkingTo());
            }
        }

        public override void Execute(Miner miner)
        {
            // Buying whiskey costs 2 gold but quenches thirst altogether
            miner.HowThirsty = 0;
            miner.MoneyInBank -= 2;
            Debug.Log(miner.name + " I love beer!");
            miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
        }

        public override void Exit(Miner miner)
        {
            Debug.Log(miner.name + " Leaving the bar, feelin' good");
        }

        public override bool OnMessage(Miner agent, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }

    // In this state, the miner eats the food that Elsa has prepared
    public class EatStew : State<Miner>
    {
        public override void Enter(Miner miner)
        {
			Debug.Log(miner.name + " Smell good Elsa, Goin' to the resturant. ");
			
			miner.TargetLocation = Location.resturant;
			
			if (miner.Location != miner.TargetLocation)
			{
				miner.StateMachine.ChangeState(new WalkingTo());
			}

        }

        public override void Execute(Miner miner)
        {
            Debug.Log(miner.name + " Tastes real good too!");
			miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
		}
		
		public override void Exit(Miner miner)
        {
            Debug.Log(miner.name + " I'm full now, continue to do my stuff'");
        }

        public override bool OnMessage(Miner agent, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }

    // If the agent has a global state, then it is executed every Update() cycle
    public class MinerGlobalState : State<Miner>
    {
        public override void Enter(Miner miner)
        {

        }

        public override void Execute(Miner miner)
        {

        }

        public override void Exit(Miner miner)
        {

        }

        public override bool OnMessage(Miner agent, Telegram telegram)
        {
            return false;
        }
		public override bool OnSenseEvent(Miner agent, Sense sense)
		{
			return false;
		}
    }

}
