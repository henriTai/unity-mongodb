Version 2

New features summary:
	* Corrected admin authorization (see below)
	* Authorization is activated.
	* Model validation for new score entries are activated.
	* Banned players are sorted out from score lists.
	* ?slice=[int] query is added for high score list.
	* Added an option to get a array of all the score entries of one player.

Configurations:

*database path is defined in MongoDB.cs and is "mongodb://localhost:27017"
*Authorization key for Unity client ("UnityKey") is set in appsettings.json and is "unity1234"
||||||||||||||*** CORRECTION *** ||||||||||||||||>>>
*Authorization key for admin ("UnityKey" also) is set in AuthenticationMiddleware.cs and is "admin123"
*PUT operations (banning) is only allowed with admin authorization (DELETE is not implemented yet) using 'players'-path.
*Right now authorization IS hooked up.
*model validation for score entry is implemented.

**************************************************************************************************

API paths:

The path for Unity client is: "http://localhost:5000/api/scores"
The path for admin is: "http://localhost:5000/api/players"

**************************************************************************************************
Working features (Unity client):

GET: Get returns top10-list of high scores. The response message in json-format looks like this:

[
    
	{
        
		"_id": "91619d8c-8346-45cd-8329-9d2f2a2db984",
        
		"name": "Aake",
        
		"score": 71,
        
		"date": "2018-09-28T08:59:46.284Z"
    
	},
    
	{
        
		"_id": "66b58b91-cbd0-40ed-9410-0e4fc6b3f95c",
        
		"name": "Jamppa",
        
		"score": 50,
        
		"date": "2018-09-27T20:35:00.701Z"
    
	},
    
	{
        
		"_id": "6e83e4eb-c801-4663-8653-6faf69bbd608",
        
		"name": "Jeppe",
        
		"score": 50,
        
		"date": "2018-09-28T08:58:58.924Z"
    
	},
    
	{
        
		"_id": "37935a6b-41f2-4cb4-a5a0-b88f7f000bac",
        
		"name": "Aake",
        
		"score": 34,
        
		"date": "2018-09-28T08:59:27.415Z"
    
	},
    
	{
        
		"_id": "07f271ca-23ef-450e-b753-85872471bea9",
        
		"name": "Erkki",
        
		"score": 25,
        
		"date": "2018-09-27T20:30:40.156Z"
    
	},
    
	{
        
		"_id": "1cd3dd12-397b-4fe3-bba4-1cb37153945d",
        
		"name": "Aake",
        
		"score": 14,
        
		"date": "2018-09-28T08:59:12.646Z"
    
	},
    
	{
        
		"_id": "d81f7fda-13c9-4b9d-a771-d92c6fa1c583",
        
		"name": "Repe",
        
		"score": 8,
        
		"date": "2018-09-27T20:41:37.626Z"
    
	},
    
	{
        
		"_id": "1dfb300f-e3b4-4cb0-ab5e-c571beedaedf",
        
		"name": "Jamppa",
        
		"score": 6,
        
		"date": "2018-09-27T20:35:13.89Z"
    
	},
    
	{
        
		"_id": "636fb7e1-3464-48a1-8580-e87208b35ac4",
        
		"name": "Jamppa",
        
		"score": 3,
        
		"date": "2018-09-27T20:34:39.278Z"
    
	},
    
	{
        
		"_id": "399d4ea8-34ee-4078-ac03-54c538d06379",
        
		"name": "Repe",
        
		"score": 3,
        
		"date": "2018-09-27T20:41:54.855Z"
    
	}

]
*Banned players are sorted out from the list.

GET +?slice=[int]
	* returns high score array STARTING FROM THE GIVEN RANK.

GET .../[Name]
	* returns an array of all the scores of one player

POST: Post takes new score entry information from the body and is in following format:

{
	
	"Name": "Jamppa",

	"Score":32

}

If the player is [banned], the score is not added and the result looks like this:

{
    
	"name": "Jamppa",
    
	"banned": true,
    
	"score": 0,
    
	"ranking": 0,
    
	"bestScore": 0,
    
	"bestRanking": 0

}

If the player is NOT [banned], it will look like this:

{
    
	"name": "Jamppa",
    
	"banned": false,
    
	"score": 32,
    
	"ranking": 5,
    
	"bestScore": 50,
    
	"bestRanking": 2

}

PUT and DELETE are not allowed operations for Unity client.

**************************************************************************************************

Working features (Admin):

GET:
	*Returns list of ALL players
+query ?banned=true
	*Returns list of all players that are not banned (and vice versa, if set false).
+/[Name]
	*Returns a player with given name (string search)
+/[id]
	*returns a player with given id (guid search)

POST:
	*Not in use

PUT:
	*4 methods: string and guid search versions for both 1) set a given banned status (bool) or without parameter change
	status to opposite:
http://localhost:5000/api/players/Jamppa?banned=true (sets banned status to true)
http://localhost:5000/api/players/Jamppa (changes banned status)
http://localhost:5000/api/players/1a0a0cc1-dc57-4612-8330-f8137a151593?banned=true (sets banned status to true)
http://localhost:5000/api/players/1a0a0cc1-dc57-4612-8330-f8137a151593 (changes banned status)

DELETE:
	This is not implemented yet. If player is removed, all the associated scores should also be removed. I guess this should be done atomically in both collections.

