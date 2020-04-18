To use: 
1. go to database prefabs and drag and drop the GameScreen prefab into the scene that controls the visuals for the game
2. go to database prefabs and drag and drop the SubmissionScreen prefab into the scene that controls the individual player screens for the game.
3. add library "using database" to the scripts you want to interface with the database.
	methods in DatabaseInteractionMobile:
		void SendTurnToDatabase(string move)
			sends the players turn to the database as a string
		async Task<bool> EnterGameAsync(string newGameID)
			returns t/f whether player was able to enter the game
			newGameID should be entered in by the user in order to join the same session
	methods in DatabaseInteractionDesktop:
		 Dictionary<int, string> GetTurnFromDatabase()
			gets both player turns from firebase storing as:	0, "player turn"
																1, "player turn"


		