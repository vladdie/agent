using UnityEngine;

using System.Collections;

namespace FSM
{
    public class Sheriff : Agent
    {
        public bool OutlawSpotted = false;
		//public int goldCarrying;
		public bool IsDead;
		public string name = "Sheriff";
        // Here is the StateMachine that the Sheriff uses to drive the agent's behaviour
        private StateMachine<Sheriff> stateMachine;
        public StateMachine<Sheriff> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }
		public int GoldCarrying
		{
			get { return goldCarrying; }
			set { goldCarrying = value; }
		}
        // And it knows its bank balance at any point in time
        private int moneyInBank;
        public int MoneyInBank
        {
            get { return moneyInBank; }
            set { moneyInBank = value; }
        }

        public Sheriff()
            : base()
        {
            stateMachine = new StateMachine<Sheriff>(this);
            stateMachine.CurrentState = new PatrolRandomLocation();
            stateMachine.GlobalState = new SheriffGlobalState();
			//name = "Sheriff";
			ID = 4;
            Location = Location.sheriffsOffice;
        }

        // This method is invoked by the Game object as a result of XNA updates 
        void Update()
        {
            stateMachine.Update();
        }
//		void Start()
//		{
//			StartCoroutine(PerformUpdate());
//		}
//		
//		// Update is called once per frame
//		IEnumerator PerformUpdate()
//		{
//			while (true)
//			{
//				stateMachine.Update();
//				yield return new WaitForSeconds(0.8f);	
//			}
//			
//		}
        // This method is invoked when the agent receives a message
        public override bool HandleMessage(Telegram telegram)
        {
            return stateMachine.HandleMessage(telegram);
        }       
        
        // This method is invoked when the agent senses
        public override bool HandleSenseEvent(Sense sense)
        {
            return stateMachine.HandleSenseEvent(sense);
        }

        public Location ChooseNextLocation()
        {
			var locManager = Object.FindObjectOfType<LocationManager>();

			Location nextLocation = locManager.ChooseRandLocation(Random.Range(0,7));
            if (nextLocation == Location.outlawCamp)
				nextLocation = Location.saloon;

            return nextLocation;
        }
    }
}
