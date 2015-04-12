using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FSM
{

    public class LocationManager : MonoBehaviour
    {

        public Dictionary<Location, Transform> Locations = new Dictionary<Location, Transform>();
		public Dictionary<Location, int> obstancleDictionary = new Dictionary<int, int>();
        void Awake()
        {
            var saloon = GameObject.FindGameObjectWithTag("saloon");
            if (saloon == null) throw new NullReferenceException("Saloon game object is not available!");
			obstancleDictionary.Add (Location.saloon, 5 );
            Locations.Add(Location.saloon, saloon.transform);

            var home = GameObject.FindGameObjectWithTag("shack");
            if (home == null) throw new NullReferenceException("Home game object is not available!");
			obstancleDictionary.Add (Location.shack, 2 );
            Locations.Add(Location.shack, home.transform);

			var mine = GameObject.FindGameObjectWithTag("goldMine");
			if (mine == null) throw new NullReferenceException("Mine game object is not available!");
			obstancleDictionary.Add (Location.goldMine, 20 );
			Locations.Add(Location.goldMine, mine.transform);

            var bank = GameObject.FindGameObjectWithTag("bank");
            if (bank == null) throw new NullReferenceException("Bank game object is not available!");
			Locations.Add(Location.bank, bank.transform);
			obstancleDictionary.Add (Location.bank, 3 );

			var resturant = GameObject.FindGameObjectWithTag("resturant");
			if (resturant == null) throw new NullReferenceException("Resturant game object is not available!");
			Locations.Add(Location.resturant, resturant.transform);
			obstancleDictionary.Add (Location.resturant, 4 );

			var toilet = GameObject.FindGameObjectWithTag("toilet");
			if (toilet == null) throw new NullReferenceException("Toilet game object is not available!");
			obstancleDictionary.Add (Location.toilet, 3 );
			Locations.Add(Location.toilet, toilet.transform);

			var cemetery = GameObject.FindGameObjectWithTag("cemetery");
			if (cemetery == null) throw new NullReferenceException("cemetery game object is not available!");
			obstancleDictionary.Add (Location.cemetery, 0 );
			Locations.Add(Location.cemetery, cemetery.transform);

			var outlawCamp = GameObject.FindGameObjectWithTag("outlawCamp");
			if (outlawCamp == null) throw new NullReferenceException("outlawCamp game object is not available!");
			obstancleDictionary.Add (Location.outlawCamp, 2 );
			Locations.Add(Location.outlawCamp, outlawCamp.transform);

			var sheriffsOffice = GameObject.FindGameObjectWithTag("sheriffsOffice");
			if (sheriffsOffice == null) throw new NullReferenceException("sheriffsOffice game object is not available!");
			obstancleDictionary.Add (Location.sheriffsOffice, 3 );
			Locations.Add(Location.sheriffsOffice, sheriffsOffice.transform);
			
			var undertakers = GameObject.FindGameObjectWithTag("undertakers");
			if (undertakers == null) throw new NullReferenceException("undertakers game object is not available!");
			//obstancleDictionary.Add (Location.undertakers, 3 );
			Locations.Add(Location.undertakers, undertakers.transform);
        }

    }
}
