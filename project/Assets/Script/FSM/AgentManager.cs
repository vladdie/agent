using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FSM
{
    public class AgentManager : MonoBehaviour
    {

        public List<Agent> listOfAgents;
		private Agent defaultAgent;
        public int AddAgent(Agent agent)
        {
            listOfAgents.Add(agent);
            return listOfAgents.IndexOf(agent);
        }

		public Agent GetAgent(int id)
		{
			return listOfAgents[id];
		}


		public int GetCount(){
			return listOfAgents.Count;
		}

		public Agent GetAgent(string name)
		{
			//            var selectedAgent = (from agent in listOfAgents
			//                                 where agent.ID == id
			//                                 select agent).FirstOrDefault();
			//
			//            return selectedAgent;
			foreach (Agent agent in listOfAgents){
				if (agent.name == name){
					defaultAgent = agent;
					return defaultAgent;
				}
				else{
					continue;
				}
			}
			return defaultAgent;
		}

        public void RemoveAgent(Agent agent)
        {
            listOfAgents.Remove(agent);
        }
    }
}