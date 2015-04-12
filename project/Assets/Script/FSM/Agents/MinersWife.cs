using UnityEngine;
using System.Collections;

namespace FSM
{
    public class MinersWife : Agent
    {
        private StateMachine<MinersWife> stateMachine;
        public int HusbandId;
		public Location location;
		public bool Cooking;
		public string name = "Elsa";

        public StateMachine<MinersWife> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }
//        public int HusbandId
//        {
//            get { return husbandId; }
//            set { husbandId = value; }
//        }
//
//        public Location MinerLocation
//        {
//            get { return location; }
//            set { location = value; }
//        }
//
//        public bool Cooking
//        {
//            get { return cooking; }
//            set { cooking = value; }
//        }

        public MinersWife()
            : base()
		//void Awake()
        {
            stateMachine = new StateMachine<MinersWife>(this);
            stateMachine.CurrentState = new DoHouseWork();
            stateMachine.GlobalState = new WifesGlobalState();
			ID = 2;
			HusbandId = this.ID - 1;  // hack hack

        }

//		void Update()
//		{
//			Message.SendDelayedMessages();
//		}
//		
		// Update is called once per frame
		void Start()
        {
            //stateMachine.Update();
			StartCoroutine(PerformUpdate());
        }

        IEnumerator PerformUpdate()
        {
            while (true)
            {
                stateMachine.Update();
                yield return new WaitForSeconds(0.8f);
            }
        }

        // This method is invoked when the agent receives a message
        public override bool HandleMessage(Telegram telegram)
        {
            return stateMachine.HandleMessage(telegram);
        }

		public override bool HandleSenseEvent(Sense sense)
		{
			return stateMachine.HandleSenseEvent(sense);
		}
    }

}
