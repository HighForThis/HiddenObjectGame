using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//*************************************************************************
//@header       HOGGameController
//@abstract     Control the game.
//@discussion   Add to the object as a component.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
	/// <summary>
	/// This is the script that controls the entire game. You set attributes for the time, score bar, object list, object area, etc. Each level has its own GameController prefab with those attributes customized as needed.
	/// </summary>
	public class HOGGameController:MonoBehaviour 
	{
        public ObjectType HOGObjectType;
		//A list of all the possible objects in the game. The hidden object for each level will be randomly selected from this list
		//public Transform[] objectList;
        public HOGObjectContainer ObjContainer;
		private int hiddenObjectIndex;
		
		//The edges of the area which the objects are scattered within
		public Transform edgeTop;
		public Transform edgeBottom;
		public Transform edgeLeft;
		public Transform edgeRight;
		
		//These attributes define how a level looks, the smaller the gap between objects the more objects are placed within the object area. You can also set how
		//much the gap changes after each level, and the minimum gap ( A gap of less than 0.2 may cause performance issues ).
		public float objectGap = 1;
		public float gapChange = 0.05f;
		public float gapMinimum = 0.2f;
        //public float extraGap = 2;
		//private float gapMargin = 0;
		internal float areaWidth;
		//internal float areaWidthCurrent;
		internal float areaHeight;
		//internal float areaHeightCurrent;
		
		//The click area of a hidden object
		public float objectClickRadius = 1;
		
		//How many objects we need to find to win this level
		public Vector2 hiddenObjectsRange = new Vector2(1,3);
		internal int numberOfHiddenObjects = 1;
		
		//How many objects we found so far this level
		internal int foundObjects = 0;
		
		//Should we show the icons of the hidden objects at the top bar?
		public bool showIconsOnTop = false;
		
		//The gap between the icons in the top bar
		public float iconsGap = 1.5f;
        public float iconsScale = 1f;

		//The top bar object which contains the timer icon and text, as well as the "find object" text.
		public Transform topBar;
		
		//The score bar which contains the level score, and total score
		public Transform scoreBar;
		private int roundScore = 0; //Our score for the current round
		//private int totalScore = 0; //Our total score in the game
		
		//The options screen shows options including Restart, Main Menu, Music, and Sound. When the options screen appears, the game is paused.
		//public Transform optionsScreen;
		
		//This screen shows when the timer reaches 0. After a delay, the game over screen is shown
		public TextMesh TimeUpText;
        public TextMesh TargetText;
		
		//This screen is shown when the game is over. It displays the total score as well as Restart and Main Menu buttons
		//public Transform gameOverScreen;
		
		//Attributes for the timer, how much time we have left, how much time increases after each level, and how many seconds we lose when misclikcing. 
		//Also how many seconds to wait before showing a hint, and how many points to earn for each second on the timer at the end of a level
		public float timeLeft = 30;
		public float timeChange = 5;
		public float timeLoss = 1;
		public float hintTime = 10;
		private float hintTimeCount = 0;
		//public int timeBonus = 100; //How many extra points we get for each second in a round
		public int timeBonusChange = 100;
		
		//A list of messages that randomly appear when winning a level
		public string[] victoryMessage;
		
		//How many seconds to wait before starting the next level
		public float nextRoundDelay = 4;
		
		//Various sounds
		public AudioClip audioFind;
		public AudioClip audioError;
		public AudioClip audioWin;
		public AudioClip audioTimeup;
		public AudioClip audioHint;
		
		//The speed at which objects fall at the end of a level
		public Vector2 objectFallSpeedX = new Vector2(-2,2);
		public Vector2 objectFallSpeedY = new Vector2(3,6);
		public Vector2 objectRotateSpeed = new Vector2(-5,5);
		
		//The animation of the top and score bars
		public AnimationClip screenIntroAnimation;
		public AnimationClip screenOutroAnimation;
		
		//Is the game paused now?
		public bool isPaused = true;

        //Is the game finished
        private bool _isGameFinished;
		
		//A genral use index
		private int index = 0;
		private Transform newObject;

        // The number of creating objects and destroy time
        private int _createNum;
        private float _destroyTimeNum;
        private bool _canOpenDestroyTimer;

        // Game category
        //private int _gameCategory;
        // Stress level
        //private int _stressLevel;
        // Player Level
        //private int _playerLevel;
        // Player Experience
        //private int _playerExp;
        // Count time without finding hidden object
        private int _timerWithoutFound;
        public Transform PlayerTransform;
        CircleCollider2D _playerCollider;
        float _totalDistance;

        //HOGGameSetting _gameSetting;

        #region Properties
        /// <summary>
        /// Game level
        /// </summary>
        private int GameLevel
        {
            get { return GameStart.Instance.CurrentGameRecorder.GameLevel; }
            set { GameStart.Instance.CurrentGameRecorder.GameLevel = value; }
        }

        /// <summary>
        /// The type of training
        /// </summary>
        private int TrainType
        {
            get { return GameStart.Instance.CurrentGameRecorder.TrainType; }
        }

        /// <summary>
        /// Player Experience
        /// </summary>
        private int PlayerExp
        {
            get { return GameStart.Instance.CurrentGameRecorder.PlayerExp; }
            set { GameStart.Instance.CurrentGameRecorder.PlayerExp = value; }
        }

        /// <summary>
        /// Player Level
        /// </summary>
        private int PlayerLevel
        {
            get { return GameStart.Instance.CurrentGameRecorder.PlayerLevel; }
            set
            {
                GameStart.Instance.CurrentGameRecorder.PlayerLevel = value;
            }
        }
        #endregion

        private void Awake()
        {
            _playerCollider = PlayerTransform.GetComponent<CircleCollider2D>();
            EventManager.Instance.AddListener(EventTypeSet.GameStart, EventGameStart);
            EventManager.Instance.AddListener(EventTypeSet.FinishGame, EventFinishGame);
        }

        /// <summary>
        /// Start is only called once in the lifetime of the behaviour.
        /// The difference between Awake and Start is that Start is only called if the script instance is enabled.
        /// This allows you to delay any initialization code, until it is really needed.
        /// Awake is always called before any Start functions.
        /// This allows you to order initialization of scripts
        /// </summary>
        void Start() 
		{            

            //Deactivate various game screens if they are assigned
            if ( TimeUpText)
            {
                TimeUpText.text = GameStart.Instance.TimeUpMessage;
                TimeUpText.transform.FindChild("Shadow").GetComponent<TextMesh>().text = GameStart.Instance.TimeUpMessage;
                TimeUpText.transform.parent.gameObject.SetActive(false);
            }
            if (TargetText)
            {
                TargetText.text = GameStart.Instance.TargetString;
            }    
			//if ( optionsScreen )    optionsScreen.gameObject.SetActive(false);
			//if ( gameOverScreen )    gameOverScreen.gameObject.SetActive(false);
			if ( scoreBar )    scoreBar.gameObject.SetActive(false);			
			
			//If we are using text instead of icons, hide the looking glass icon
			if ( showIconsOnTop == false )
			{
				topBar.Find("Base/GlassIcon").gameObject.SetActive(false);
			}

            //if(GameObject.Find("Follower") != null)
            //    PlayerTransform = GameObject.Find("Follower").GetComponent<Transform>();

            // Get information from setting
            GetDateFromSettting();

            ////Create the first level
            //CreateLevel(GameLevel);
            isPaused = false;
            _isGameFinished = false;
		}

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventTypeSet.GameStart, EventGameStart);
            EventManager.Instance.RemoveListener(EventTypeSet.FinishGame, EventFinishGame);
        }

        // Update is called once per frame
        void Update() 
		{
			//Check if we have a topBar assigned, which contains the timer
			if ( topBar )
			{
				//Count down the time left until the level ends
				if ( !isPaused && GameStart.Instance.CanPlayGame)    
				{
					//Reduce the timer
					timeLeft -= Time.deltaTime;
					
					//Update the timer text
					UpdateTimer();

                    // Update the timer of shaking and destroying
                    UpdateDestroyTime(GameLevel, hintTime);

                    // Update the distance
                    CalculateCurrentDistance();
				}
                //_playerCollider.enabled = GameStart.Instance.CanPlayGame;

            }
		}

		/// <summary>
		/// This function creates a level, placing objects evenly within the object area, and then adding the hidden objects over them.
		/// </summary>
		void CreateLevel(int levelNum)
		{
			//Check if we have the edge points assigned. These points define the area in which the objects are placed
			if ( edgeLeft && edgeTop && edgeRight && edgeBottom )
			{
				//Reset the hint timer at the start of a level
				hintTimeCount = 0;
				
				//Reset the number of objects we found to 0
				foundObjects = 0;

                //Choose a random number of objects to be hidden
                numberOfHiddenObjects = Mathf.RoundToInt(Random.Range( hiddenObjectsRange.x, hiddenObjectsRange.y));
				
				//Choose the type of object that will be hidden from the list of objects we set
				//hiddenObjectIndex = Mathf.FloorToInt(Random.value * (ObjContainer.ObjectList[(int)HOGObjectType].ObjItems.Length));
                hiddenObjectIndex = Random.Range(0, (ObjContainer.ObjectList[(int)HOGObjectType].ObjItems.Length));

                CreateObjectsDependOnLevel(levelNum);

                GameStart.Instance.SendCmdWhenAssist();

				////Calculate the margin based on the gap and area width
				//gapMargin = (areaWidth - Mathf.FloorToInt(areaWidth/objectGap) * objectGap)/2;
				
				////Start at the top of the area
				//areaHeightCurrent = 0;
				
				////Place objects in a row until you reach the width of the area, then move down by the value of gapMargin and palce another row of objects.
				////Repeat until the entire area is filled with objects.
				//while ( areaHeightCurrent <= areaHeight )
				//{
				//	areaWidthCurrent = gapMargin;
					
				//	while ( areaWidthCurrent <= areaWidth )
				//	{
				//		//Choose a random object from the list
				//		int randomIndex = Mathf.FloorToInt(Random.value * objectList.Length);
						
				//		//If the random object happens to be the same as the hidden object, choose another
				//		if ( randomIndex == hiddenObjectIndex )
				//		{
				//			if ( randomIndex != 0 )
				//			{
				//				randomIndex--;
				//			}
				//			else
				//			{
				//				randomIndex++;
				//			}
				//		}
						
				//		//Create the object, scale it and give it a random rotation. Then put it in the edgeTop object for easier access later
				//		newObject = Instantiate( objectList[randomIndex], new Vector3(edgeLeft.position.x + areaWidthCurrent, edgeBottom.position.y + areaHeightCurrent, Random.Range(-3, 3)), Quaternion.identity) as Transform;
						
				//		newObject.localScale *= objectGap;
						
				//		newObject.eulerAngles = new Vector3( newObject.eulerAngles.x, newObject.eulerAngles.y, Random.Range(0,360));
						
				//		newObject.parent = edgeTop;
						
				//		newObject.SendMessage("DelayAnimation", Random.Range(0,0.1f));
						
				//		areaWidthCurrent += (objectGap + extraGap);
				//	}
					
				//	areaHeightCurrent += (objectGap + extraGap);
				//}

    //            //Now we start placing the hidden objects
    //            CreateHiddenObjects(numberOfHiddenObjects);
				
				
			}
			else
			{
				Debug.LogWarning("You must assign the Top/Bottom/Left/Right edges in the inspector");
			}
            // Show collider
            _playerCollider.enabled = true;
		}

		/// <summary>
		/// This function add 1 to the number of found objects, and then checks if we found all the hidden objects to win the level. 
		/// The function is called from the hidden object itself when you click it. The hidden object has a HOGHiddenObject script attached to it which detects the click.
		/// </summary>
		IEnumerator UpdateFoundObjects()
		{
            //Debug.Log("<color=orange>UpdateFoundObjects!!</color>");
			//Remove one of the hidden object icons, if they exist
			if ( showIconsOnTop == true )
			{
				//if ( topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1)) )    Destroy(topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1)).gameObject);    
				
				if ( topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1)) )    
				{
					//Play the object's find animation
					topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1)).SendMessage("ObjectIcon");
					
					//Wait a default time of 0.1 second
					yield return new WaitForSeconds(0.1f);
					
					//Remove the object icon
					Destroy(topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1)).gameObject); 
				}
			}
            			
            //Increase the number of found objects
			foundObjects++;

            roundScore =  CalculateUnity.FinalScore(GameStart.Instance.MinTargetScore,GameStart.Instance.MaxTargetScore);
			
			//Reset hint timer
			hintTimeCount = 0;

			//If we find all the hidden objects we win the level
			if ( foundObjects < numberOfHiddenObjects )
			{
				if ( GetComponent<AudioSource>() )    GetComponent<AudioSource>().PlayOneShot(audioFind);
			}
			else
			{
				if ( GetComponent<AudioSource>() )    GetComponent<AudioSource>().PlayOneShot(audioWin);
                _timerWithoutFound = 0;
				Win(roundScore);
			}
		}
		
		/// <summary>
		/// This function wins a level. It pauses the game and applies the score based on the level we are at and the number of seconds left on the timer. Then it creates the next level.
		/// </summary>
		void Win(int scoreNum)
		{
			PauseTimer();
			
			//Activate the score bar which contains the level score and totalScore
			scoreBar.gameObject.SetActive(true);
			
			//Play the score bar intro animation
			scoreBar.GetComponent<Animation>().Play(screenIntroAnimation.name);

            // Set multiple of score
            int multipleOfScore = 1;
			
			//If we have a top bar assigned, show the victory message and add bonus time to the timer
			if ( topBar )    
			{
				//If we assigned victory messages, choose one of them randomly and display it in the top bar
				if ( victoryMessage.Length > 0)
                {
                    if (scoreNum != 0)
                    {
                        topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = victoryMessage[Mathf.FloorToInt(Random.Range(0, victoryMessage.Length -1))];
                        multipleOfScore = CalculateUnity.GetPreciousValue();
                        AddExp(scoreNum * multipleOfScore);
                    }
                    else
                    {
                        topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = victoryMessage[victoryMessage.Length-1];

                        int leftNum = numberOfHiddenObjects - foundObjects;
                        for (int i = leftNum; i > 0; i--)
                        {
                            //Remove the object icon
                            Destroy(topBar.Find("HiddenIcon" + (i - 1)).gameObject);
                        }
                    }
                }    

                ////Show the extra time we got on the timer
                //topBar.Find("Base/TimerText").GetComponent<TextMesh>().text = topBar.Find("Base/TimerText").Find("Shadow").GetComponent<TextMesh>().text = timeLeft.ToString("00");// + " +" + timeChange.ToString();
			}
			
			//If we have a score bar, show the level score and the total score
			if ( scoreBar )
			{
                //Show the level score
                //scoreBar.Find("Base/RoundScore").GetComponent<TextMesh>().text = scoreBar.Find("Base/RoundScore").Find("Shadow").GetComponent<TextMesh>().text = "+" + (timeBonus * Mathf.RoundToInt(timeLeft)).ToString();
                string textShow = string.Empty;
                if (scoreNum==0 || multipleOfScore==1)
                {
                    textShow = string.Format("+ {0}", scoreNum);
                }
                else
                {
                    textShow = string.Format(GameStart.Instance.RewardString + scoreNum*multipleOfScore, multipleOfScore);
                }
                scoreBar.Find("Base/RoundScore").GetComponent<TextMesh>().text = scoreBar.Find("Base/RoundScore").Find("Shadow").GetComponent<TextMesh>().text = textShow;

                //Add the level score to the total score
                //totalScore += timeBonus * Mathf.RoundToInt(timeLeft);
                //totalScore += scoreNum;

                ////Show the total score
                //scoreBar.Find("Base/TotalScore").GetComponent<TextMesh>().text = scoreBar.Find("Base/TotalScore").Find("Shadow").GetComponent<TextMesh>().text = (scoreNum*multipleOfScore).ToString();

                ////Increas the time bonus for the next level
                //timeBonus += timeBonusChange;
            }
			
			//Clear the objects from the area
			ClearObjects(false);
		
			Invoke ("NextRound", nextRoundDelay);
		}

		void NextRound()
		{
            //scoreBar.animation.Play("BarOutro");
            scoreBar.GetComponent<Animation>().Play(screenOutroAnimation.name);

            //Deactivate the score bar which contains the level score and totalScore
            scoreBar.gameObject.SetActive(false);

            //Add to the timer
            //timeLeft += timeChange;
            if (_isGameFinished)
                return;

			UpdateTimer();						

            //objectGap -= gapChange;
            //extraGap -= gapChange;
            //if (extraGap < 0) extraGap = 0;
			
			if ( objectGap < gapMinimum )    objectGap = gapMinimum;

            // TODO:Calculate the level of game
            CalculateNextGameLevel(TrainType, _timerWithoutFound);
			
			CreateLevel(GameLevel);
			
			StartTimer();            

        }

        /// <summary>
        /// This function clears all objects from the level, making them fall down and then removing them
        /// </summary>
        void ClearObjects(bool isEndGame)
		{
			//Go through all the objects in the edgeTop. If you check out the creation of the objects ( CreateLevel() ) you'll notice that we placed all the objects in 
			//edgeTop for easier access.
			foreach ( Transform fallingObject in edgeTop )
			{
                if (fallingObject.gameObject.activeSelf)
                {
                    //Add a rigid body to the object
                    if (fallingObject.transform.GetComponent<Rigidbody2D>() == null)
                        fallingObject.gameObject.AddComponent<Rigidbody2D>();

                    //if (fallingObject.GetComponent<Rigidbody2D>())
                    //{
                    //    fallingObject.GetComponent<Rigidbody2D>().simulated = true;
                    //    //Throw it in a random direction and rotation
                    //    fallingObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(objectFallSpeedX.x, objectFallSpeedX.y), Random.Range(objectFallSpeedY.x, objectFallSpeedY.y));

                    //    fallingObject.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(objectRotateSpeed.x, objectRotateSpeed.y);

                    //}

                    //Throw it in a random direction and rotation
                    fallingObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(objectFallSpeedX.x, objectFallSpeedX.y), Random.Range(objectFallSpeedY.x, objectFallSpeedY.y));

                    fallingObject.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(objectRotateSpeed.x, objectRotateSpeed.y);

                    //Destroy the object after a few seconds
                    //Destroy(fallingObject.gameObject, 3);
                    if(!isEndGame)
                        StartCoroutine(WaitToRecycle(fallingObject, 2.5f));
                }

                if (isEndGame)
                {
                    HOGTargetPool.Instacne.RemoveObject(fallingObject);
                    Destroy(fallingObject.gameObject, 2.5f);
                }
            }
		}

        IEnumerator WaitToRecycle(Transform trans, float timeNum)
        {
            yield return new WaitForSeconds(timeNum);
            if (trans.GetComponent<HOGPooledTarget>())
            {
                trans.GetComponent<HOGPooledTarget>().Recycle();
            }
        }

        /// <summary>
        /// This function updates the timer text and checks if time is up.
        /// </summary>
        void UpdateTimer()
		{
            //Assign the time in the text mesh
            //int timeLeftInt = int.Parse(timeLeft.ToString("00"));
            GameStart.Instance.TimeLeftSecond = int.Parse(timeLeft.ToString("00"));
            GameStart.Instance.TimeDestroySecond = int.Parse(_destroyTimeNum.ToString("00"));
            topBar.Find("Base/TimerText").GetComponent<TextMesh>().text = topBar.Find("Base/TimerText").Find("Shadow").GetComponent<TextMesh>().text = _destroyTimeNum.ToString("00");

            //If we have less than 5 seconds left on the timer, start animating the color of the timer icon
            //if ( timeLeft > 0 && timeLeft <= 5 )
            //{
            //	topBar.Find("Base/TimerClock").SendMessage("Pause", false);
            //}
            //else
            //{
            //	topBar.Find("Base/TimerClock").GetComponent<SpriteRenderer>().color = topBar.Find("Base/TimerClock").GetComponent<HOGAnimateColors>().colorList[0];

            //	topBar.Find("Base/TimerClock").SendMessage("Pause", true);
            //}

            //If there is no more time left, it's game over
            if ( timeLeft <= 0 )
			{
				//Show the TimeUp screen
				if ( TimeUpText )    TimeUpText.transform.parent.gameObject.SetActive(true);
                //topBar.Find("Base/TimerText").GetComponent<TextMesh>().text = topBar.Find("Base/TimerText").Find("Shadow").GetComponent<TextMesh>().text = timeLeft.ToString("00");

                if (GetComponent<AudioSource>()) GetComponent<AudioSource>().PlayOneShot(audioTimeup);

                timeLeft = 0;



                ////Show the game over screen after 2 seconds
                //Invoke("GameOver", 2);
                EndTheGame(true);
                FlowMediator.Instance.Next();
			}
		}
		
		void PauseTimer()
		{
			isPaused = true;
            // Hide collider
            _playerCollider.enabled = false;
        }
		
		void StartTimer()
		{
			isPaused = false;
		}
		
		/// <summary>
		/// This function updates the time.
		/// </summary>
		/// <param name=""> changeValue </param>
		void ChangeTimer( float changeValue )
		{
			timeLeft += changeValue;
			
			UpdateTimer();
		}
		
		/// <summary>
		/// This function reduces from the timer and shakes the screen
		/// </summary>
		void Error()
		{
			ChangeTimer(-timeLoss);
			
			transform.SendMessage("StartShake");
		}
		
		/// <summary>
		/// This function shows a hint, shaking a hidden object
		/// </summary>
		void ShowHint()
		{
			if ( audioHint )
			{
				if ( GetComponent<AudioSource>() )
				{
					GetComponent<AudioSource>().PlayOneShot(audioHint);
				}
				else
				{
					Debug.LogWarning("You must add an AudioSource component to this object in order to play sounds");
				}
			}

            //Find a hidden object and run the object hint frunction from it
            foreach (Transform item in edgeTop)
            {
                if (item.gameObject.activeSelf)
                {
                    if (item.name == ObjContainer.ObjectList[(int)HOGObjectType].ObjItems[hiddenObjectIndex].name)
                    {
                        item.SendMessage("ObjectHint");
                    }   
                }
            }
			//if ( edgeTop.Find(ObjContainer.ObjectList[(int)HOGObjectType].ObjItems[hiddenObjectIndex].name))    
   //             edgeTop.Find(ObjContainer.ObjectList[(int)HOGObjectType].ObjItems[hiddenObjectIndex].name).SendMessage("ObjectHint");
		}


        /*
		/// <summary>
		/// This function pauses the game and shows the options screen
		/// </summary>
		//IEnumerator ToggleOptions()
		//{
		//	isPaused = !isPaused;
			
		//	if ( optionsScreen )
		//	{
		//		//If the options screen is not centered, center it
		//		optionsScreen.localPosition = new Vector3( 0, 0, optionsScreen.localPosition.z);
				
		//		if ( isPaused == true )
		//		{
		//			optionsScreen.gameObject.SetActive(true);
		//			optionsScreen.GetComponent<Animation>().Play(screenIntroAnimation.name);
		//		}
		//		else
		//		{
		//			optionsScreen.GetComponent<Animation>().Play(screenOutroAnimation.name);
					
		//			yield return new WaitForSeconds(screenOutroAnimation.length);

		//			optionsScreen.gameObject.SetActive(false);
		//		}
				
				
		//	}
		//	else    Debug.LogWarning("You must assign a game options screen in the component");
		//}
		
		/// <summary>
		/// This function pauses the timer and shows the game over screen
		/// </summary>
		/// <param name="">.</param>
		//void GameOver()
		//{
  //          //if ( gameOverScreen )    
  //          //{
  //          //	gameOverScreen.gameObject.SetActive(true);

  //          //	gameOverScreen.Find("Base/TotalScore").GetComponent<TextMesh>().text = gameOverScreen.Find("Base/TotalScore/Shadow").GetComponent<TextMesh>().text = totalScore.ToString();
  //          //}
  //          GameStart.Instance.OpenOrCloseMessagePage(true, HiddenObjectPage.HOGUIGameOver);
		//}

        
        /// <summary>
        /// Placing the hidden objects
        /// </summary>
        /// <param name="hiddenNum"></param>
        void CreateHiddenObjects(int hiddenNum)
        {
            index = 0;

            while (index < hiddenNum)
            {
                //Create a hidden object, scale it and give it a random rotation. Then put it in the edgeTop object for easier access later
                newObject = Instantiate(objectList[hiddenObjectIndex], new Vector3(Random.Range(edgeLeft.position.x, edgeRight.position.x), Random.Range(edgeBottom.position.y, edgeTop.position.y), -3), Quaternion.identity) as Transform;

                newObject.localScale *= objectGap;

                newObject.eulerAngles = new Vector3(newObject.eulerAngles.x, newObject.eulerAngles.y, Random.Range(0, 360));

                newObject.parent = edgeTop;

                newObject.SendMessage("DelayAnimation", Random.Range(0, 0.1f));

                //Also add a collider so we can click it, and make sure it shows in front of the regular objects.
                newObject.gameObject.AddComponent<CircleCollider2D>();

                newObject.GetComponent<CircleCollider2D>().radius = objectClickRadius;// * objectGap;

                newObject.GetComponent<CircleCollider2D>().isTrigger = true;    //Add on 2017/12/25********************************************

                newObject.SendMessage("Hidden", true);

                newObject.position = new Vector3(newObject.position.x, newObject.position.y, newObject.position.z - 1);

                //Create the icons of the hidden object in the top bar
                if (showIconsOnTop == true)
                {
                    Transform newIcon = Instantiate(objectList[hiddenObjectIndex], new Vector3(Random.Range(edgeLeft.position.x, edgeRight.position.x), Random.Range(edgeBottom.position.y, edgeTop.position.y), 0), Quaternion.identity) as Transform;

                    newIcon.localScale *= iconsScale;

                    newIcon.parent = topBar;

                    newIcon.position = topBar.Find("Base/GlassIcon").position;

                    newIcon.position = new Vector3(newIcon.position.x + (index+1) * iconsGap, newIcon.position.y, newIcon.position.z + 0.5f);

                    newIcon.SendMessage("SetIconRotation");

                    newIcon.SendMessage("ObjectIntro");

                    newIcon.name = "HiddenIcon" + index;
                }

                index++;
            }

            if (showIconsOnTop == false)
            {
                //Write the find message based on the word, the article(a,an), the number of hidden objects (more than 1), etc
                if (hiddenNum > 1)
                {
                    topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = "FIND " + hiddenNum.ToString() + " " + newObject.GetComponent<HOGHiddenObject>().namePlural;
                }
                else
                {
                    topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = "FIND " + newObject.GetComponent<HOGHiddenObject>().nameArticle + " " + newObject.GetComponent<HOGHiddenObject>().objectName;
                }
            }
            else
            {
                topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = "";
            }
        }

        */

        #region ExtraMethods
        /// <summary>
        /// Calculate all positions of prefabs which can create new objects.
        /// </summary>
        /// <param name="width">Max width of game</param>
        /// <param name="height">Max height of game</param>
        /// <returns></returns>
        List<Vector2> CalculatePrefabPosition(float width, float height)
        {
            List<Vector2> resultPos = new List<Vector2>();
            float currentWidth = 0;
            float currentHeight = 0;
            //Debug.Log("<color=yellow>Width:" + width + ">>>Height:" + height + "</color>");
            int indexX = 0;
            while (currentWidth < width - 2)
            {
                int indexY = 0;
                currentWidth = indexX * 3 + 1;

                while (currentHeight < height - 2)
                {
                    currentHeight = indexY * 2;
                    resultPos.Add(new Vector2(currentWidth, currentHeight));
                    indexY++;
                    //Debug.Log("<color=yellow>X:" + currentWidth + ">>>Y:" + currentHeight + "</color>");
                }

                currentHeight = 0;
                indexX++;
            }

            return resultPos;
        }

        void CreateObjectsDependOnLevel(int levelNum)
        {
            List<Vector2> prefabPos = new List<Vector2>();
            prefabPos = CalculatePrefabPosition(areaWidth, areaHeight);

            #region SwitchLevel
            switch (levelNum)
            {
                case 1:
                    SetCreateNumAndTimeNum(3, 60);
                    break;
                case 2:
                    SetCreateNumAndTimeNum(5, 60);
                    break;
                case 3:
                    SetCreateNumAndTimeNum(10, 60);
                    break;
                case 4:
                    SetCreateNumAndTimeNum(15, 60);
                    break;
                case 5:
                    SetCreateNumAndTimeNum(20, 60);
                    break;
                case 6:
                    SetCreateNumAndTimeNum(25, 60);
                    break;
                case 7:
                    SetCreateNumAndTimeNum(30, 60);
                    break;
                case 8:
                    SetCreateNumAndTimeNum(30, 45);
                    break;
                case 9:
                    SetCreateNumAndTimeNum(30, 40);
                    break;
                case 10:
                    SetCreateNumAndTimeNum(30, 35);
                    break;
                case 11:
                    SetCreateNumAndTimeNum(30, 30);
                    break;
                case 12:
                    SetCreateNumAndTimeNum(40, 30);
                    break;
                case 13:
                    SetCreateNumAndTimeNum(50, 30);
                    break;
                case 14:
                    SetCreateNumAndTimeNum(60, 30);
                    break;
                case 15:
                    SetCreateNumAndTimeNum(60, 25);
                    break;
                case 16:
                    SetCreateNumAndTimeNum(60, 20);
                    break;
                case 17:
                    SetCreateNumAndTimeNum(60, 15);
                    break;
                case 18:
                    SetCreateNumAndTimeNum(70, 15);
                    break;
                default:
                    break;
            }
            #endregion

            if (_createNum < numberOfHiddenObjects)
                _createNum = numberOfHiddenObjects;
            Debug.Log("<color=blue>Prefabs:" + prefabPos.Count + "</color>");
            for (int i = 0; i < numberOfHiddenObjects; i++)
            {
                int hiddenIndex = Random.Range(0, prefabPos.Count);
                CreateHiddenObjects(prefabPos[hiddenIndex]);
                prefabPos.RemoveAt(hiddenIndex);
            }

            int numberOfShowObjects = _createNum - numberOfHiddenObjects;

            if (numberOfShowObjects > prefabPos.Count)
            {
                // Out of range
                numberOfShowObjects = prefabPos.Count;
                Debug.Log("<color=red>NumberOfShowObjects:" + numberOfShowObjects + "</color>");
            }

            for (int i = 0; i < numberOfShowObjects; i++)
            {
                int showIndex = Random.Range(0, prefabPos.Count);
                //Debug.Log("<color=yellow>Count:" + prefabPos.Count + ">>>Index:" + showIndex + "</color>");
                CreateShowObjects(prefabPos[showIndex]);
                prefabPos.RemoveAt(showIndex);
            }
            Debug.Log("<color=green>Num:"+_createNum + ">>>Time:"+_destroyTimeNum+"</color>");
            _canOpenDestroyTimer = true;
        }

        void SetCreateNumAndTimeNum(int createNum,float timeNum)
        {
            _createNum = createNum;
            _destroyTimeNum = timeNum;
        }

        void CreateHiddenObjects(Vector2 createPos)
        {
            //Create a hidden object, scale it and give it a random rotation. Then put it in the edgeTop object for easier access later
            //newObject = Instantiate(objectList[hiddenObjectIndex], new Vector3(edgeLeft.position.x + createPos.x,edgeBottom.position.y + createPos.y, -3), Quaternion.identity) as Transform;
            newObject = HOGTargetPool.Instacne.GetObject(HOGObjectType, ObjContainer.ObjectList, hiddenObjectIndex, new Vector3(edgeLeft.position.x + createPos.x, edgeBottom.position.y + createPos.y, -3));

            newObject.localScale *= objectGap;

            //newObject.eulerAngles = new Vector3(newObject.eulerAngles.x, newObject.eulerAngles.y, Random.Range(0, 360));
            newObject.eulerAngles = new Vector3(newObject.eulerAngles.x, newObject.eulerAngles.y, 0);

            newObject.parent = edgeTop;

            newObject.SendMessage("DelayAnimation", Random.Range(0, 0.1f));

            //Also add a collider so we can click it, and make sure it shows in front of the regular objects.
            //newObject.gameObject.AddComponent<CircleCollider2D>();

            //newObject.GetComponent<CircleCollider2D>().radius = objectClickRadius;// * objectGap;

            //newObject.GetComponent<CircleCollider2D>().isTrigger = true;    //Add on 2017/12/25********************************************

            if (newObject.GetComponent<CircleCollider2D>())
            {
                newObject.GetComponent<CircleCollider2D>().enabled = true;

                newObject.GetComponent<CircleCollider2D>().radius = objectClickRadius;// * objectGap;

                newObject.GetComponent<CircleCollider2D>().isTrigger = true;
            }

            newObject.SendMessage("Hidden", true);

            newObject.position = new Vector3(newObject.position.x, newObject.position.y, newObject.position.z - 1);

            // Record the position of target
            GameStart.Instance.LastPos = GameStart.Instance.TargetPos;
            GameStart.Instance.TargetPos = newObject.position;
            _totalDistance = Vector3.Distance(newObject.position, PlayerTransform.position);

            //Create the icons of the hidden object in the top bar
            if (showIconsOnTop == true)
            {
                //Transform newIcon = Instantiate(objectList[hiddenObjectIndex], new Vector3(Random.Range(edgeLeft.position.x, edgeRight.position.x), Random.Range(edgeBottom.position.y, edgeTop.position.y), 0), Quaternion.identity) as Transform;

                Transform newIcon = Instantiate(ObjContainer.ObjectList[(int)HOGObjectType].ObjItems[hiddenObjectIndex], new Vector3(Random.Range(edgeLeft.position.x, edgeRight.position.x), Random.Range(edgeBottom.position.y, edgeTop.position.y), 0), Quaternion.identity) as Transform;

                newIcon.localScale *= iconsScale;

                newIcon.parent = topBar;

                newIcon.position = topBar.Find("Base/GlassIcon").position;

                newIcon.position = new Vector3(newIcon.position.x + (index + 1) * iconsGap, newIcon.position.y, newIcon.position.z + 0.5f);

                newIcon.SendMessage("SetIconRotation");

                newIcon.SendMessage("ObjectIntro");

                newIcon.name = "HiddenIcon" + index;
            }

            topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = "";

        }

        void CreateShowObjects(Vector2 createPos)
        {
            //Choose a random object from the list
            //int randomIndex = Mathf.FloorToInt(Random.value * ObjContainer.ObjectList[(int)HOGObjectType].ObjItems.Length);
            int randomIndex = Random.Range(0, ObjContainer.ObjectList[(int)HOGObjectType].ObjItems.Length);

            //If the random object happens to be the same as the hidden object, choose another
            if (randomIndex == hiddenObjectIndex)
            {
                if (randomIndex != 0)
                {
                    randomIndex--;
                }
                else
                {
                    randomIndex++;
                }
            }

            //Create the object, scale it and give it a random rotation. Then put it in the edgeTop object for easier access later
            //newObject = Instantiate(objectList[randomIndex], new Vector3(edgeLeft.position.x + createPos.x, edgeBottom.position.y + createPos.y, Random.Range(-3, 3)), Quaternion.identity) as Transform;

            newObject = HOGTargetPool.Instacne.GetObject(HOGObjectType, ObjContainer.ObjectList, randomIndex, new Vector3(edgeLeft.position.x + createPos.x, edgeBottom.position.y + createPos.y, Random.Range(-3, 3)));

            newObject.localScale *= objectGap;

            //newObject.eulerAngles = new Vector3(newObject.eulerAngles.x, newObject.eulerAngles.y, Random.Range(0, 360));
            newObject.eulerAngles = new Vector3(newObject.eulerAngles.x, newObject.eulerAngles.y, 0);

            newObject.parent = edgeTop;

            newObject.SendMessage("DelayAnimation", Random.Range(0, 0.1f));
        }

        void GetDateFromSettting()
        {
            //timeLeft = GameStart.Instance.TrainTime;
            //_gameSetting = HOGGameSettingGenerator.Instance.CreateGameSetting();
            timeLeft = HOGGameSettingGenerator.Instance.GameSetting.TrainTime;
            _timerWithoutFound = 0;
            victoryMessage = GameStart.Instance.VictoryMessage;
            //// Get level from file
            //GameLevel = 1;
            //// Get training type from json
            //_trainType = 2;
            //// Get game category
            //_gameCategory = 1;
            // Player's level
            //_playerLevel = 1;
            //// Player's experience
            //_playerExp = 0;
            //// Stress level
            //_stressLevel = 1;
        }

        void CalculateNextGameLevel(int trainType, int failNum)
        {
            if (failNum >= 3)
            {
                GameLevel--;
                _timerWithoutFound = 0;
            }
            else if (failNum == 0)
            {
                GameLevel++;
            }
                

            if (trainType == 2 && GameLevel > 10)
                GameLevel = 10;

            if (GameLevel < 1)
                GameLevel = 1;

            if (GameLevel > 18)
                GameLevel = 18;
        }

        /// <summary>
        /// Add score to experience and calculate the level of player.
        /// </summary>
        /// <param name="givenScore"></param>
        void AddExp(int givenScore)
        {
            PlayerExp += givenScore;
            GameStart.Instance.CurrentGameRecorder.Score += givenScore;
            int tempLevel = PlayerLevel;
            if (CalculateUnity.LevelUpCalculator(ref tempLevel, PlayerExp, GameStart.Instance.NextLevelExp))
            {
                GameStart.Instance.GetNextLevelExp(tempLevel);
                PlayerLevel = tempLevel;
                GameStart.Instance.OpenOrCloseMessagePage(true, HiddenObjectPage.HOGUILevelUp);
            }
            // Add number of movement
            MainCore.Instance.AddMovementCount();
        }

        /// <summary>
        /// Update the time of destroying the target
        /// </summary>
        /// <param name="levelNum"></param>
        /// <param name="shakeTime"></param>
        void UpdateDestroyTime(int levelNum, float shakeTime)
        {
            if (levelNum <= 10)
            {
                //Count down to the next hint
                if (hintTimeCount < shakeTime)
                {
                    hintTimeCount += Time.deltaTime;
                }
                else
                {
                    //Reset the hint timer
                    hintTimeCount = 0;
                    Debug.Log("<color=yellow>Hit!</color>");
                    EventManager.Instance.PublishEvent(EventTypeSet.TargetCreated, new EventData(this, GameStart.Instance.TargetPos));
                    //Show the hint
                    ShowHint();
                }
            }

            _destroyTimeNum -= Time.deltaTime;

            if (_canOpenDestroyTimer && _destroyTimeNum <= 0)
            {
                Win(0);
                GameStart.Instance.CurrentGameRecorder.LostTime++;
                _timerWithoutFound++;
                _canOpenDestroyTimer = false;
            }
        }
        
        /// <summary>
        /// Calculate the distance between player and target on update.
        /// </summary>
        void CalculateCurrentDistance()
        {
            float distance = Vector3.Distance(GameStart.Instance.TargetPos,PlayerTransform.position) - objectClickRadius / 2;

            distance = distance > _totalDistance ? _totalDistance : distance;

            GameStart.Instance.TargetDistanceProportion = 1 - distance / _totalDistance;
        }        

        /// <summary>
        /// End the game without the time or within the time.
        /// </summary>
        /// <param name="isNormal"></param>
        void EndTheGame(bool isNormal)
        {
            // Within the time
            //if (isNormal)
            //{

            //}

            //remove all the objects from the screen
            ClearObjects(true);

            //Remove all the hidden object icons, if they exist ( The timer ran out so we are clearing everything onscreen )
            if (showIconsOnTop == true)
            {
                while (foundObjects < numberOfHiddenObjects)
                {
                    if (topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1))) Destroy(topBar.Find("HiddenIcon" + (numberOfHiddenObjects - foundObjects - 1)).gameObject);

                    foundObjects++;
                }
            }

            // Pause the game timer
            PauseTimer();

            // Close the top bar
            if (topBar) topBar.GetComponent<Animation>().Play(screenOutroAnimation.name);

            //// Send event of GameOver
            //EventManager.Instance.PublishEvent(EventTypeSet.FinishGame, new EventData(this, isNormal));
            // Record the data of game.
            GameStart.Instance.CurrentGameRecorder.RecordTheDataInGame();
        }

        void EventGameStart(EventData data)
        {
            // Add on 2017/12/27************************************************
            if (!MainCore.Instance.IsMachineDisabled)
            {
                float widthScale = HOGGameSettingGenerator.Instance.GameSetting.WidthScale;
                float heightScale = HOGGameSettingGenerator.Instance.GameSetting.HeightScale;
                if (DLMotion.DynaLinkHS.MechType == 2)
                {
                    heightScale = 0.5f;
                }
                edgeTop.localPosition = new Vector3(edgeTop.localPosition.x, edgeBottom.localPosition.y + MachinePara.OrpHeight * heightScale, edgeTop.localPosition.z);
                edgeLeft.localPosition = new Vector3(edgeLeft.localPosition.x * widthScale, edgeLeft.localPosition.y, edgeLeft.localPosition.z);
                edgeRight.localPosition = new Vector3(edgeRight.localPosition.x * widthScale, edgeRight.localPosition.y, edgeRight.localPosition.z);
                Debug.LogWarning("<color=orange>"+heightScale+"</color>" + MachinePara.OrpHeight);
            }

            //Calculate the size of the area in which objects are placed
            areaWidth = Vector3.Distance(edgeLeft.position, edgeRight.position);
            areaHeight = Vector3.Distance(edgeTop.position, edgeBottom.position);

            CreateLevel(GameLevel);

            topBar.Find("Base/FindObjectText").GetComponent<TextMesh>().text = topBar.Find("Base/FindObjectText/Shadow").GetComponent<TextMesh>().text = GameStart.Instance.TopBarTipMessage;

        }

        void EventFinishGame(EventData data)
        {
            //Show the game over screen after 2 seconds
            //Invoke("GameOver", 1.5f);
            _isGameFinished = true;
            EndTheGame(true);
        }
        #endregion

    }
}








