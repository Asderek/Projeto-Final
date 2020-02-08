# Projeto-Final
Projeto Final de Computação - PUC-Rio

  A video demonstration of the project can be found at <https://bit.ly/2ZYVDsx>

-Games, like society, are always changing. Their importance, influence, cultural significance and meaning change over time. Following that new perception of video games, companies changed their way of creating games as well.
-Games that were made only to distract players for a couple hours gave way to deeper games, with well-developed characters, mechanics and storylines. The way developers told stories changed as well. Where once games had simple pre-programmed linear storylines, we now have hugely complex games, with several plot points that branch the story in different directions based on the players choices and decisions along the game. This storytelling technique is called branching and is widely used and considered one of the best ways to adapt the story to a particular player.
-However, the level of adaptability of a storyline can be improved. This project intends to develop a game that, based on the players’ psychological profile and the relationship between players, decides the best quest, from a pool of available quests. To do that, we utilize a combination of questionnaires meant to assess the players’ gaming interests, as well as the players’ psychological and behavioural characteristics, based on the well-known Big Five model.
-With these pieces of information, along with information about the relationship between players, we choose which available next-quest provides the best gaming experience to that particular group of players.

4.1. System Architecture
-The unity project is comprised of a main game manager, GameManager. This manager controls the main aspects of the game (enemy velocity, power up duration, etc.). It also possesses the models (prefabs) for the players, enemies, victims, rescue zones, objectives and items.
-Besides the main GameManager, the project is comprised also of:
- - UI Manager - Controls all the information that are displayed on the screen (player empathy, ocean values, etc)
- - BFGI Manager - Big Five Game Inventory Manager, controls all the information regarding the players OCEAN values.
- - Player Behaviour Manager - Keeps statistics of each player (average enemy kill time, average time between attacks, etc). Also keeps the players empathy information.
- - Scene Manager - Controls the creation, processing and clean up of the scenarios and quests. Instantiates enemies, objectives, victims, etc.
- - Quest Manager - Holds the list of existing quests, as well as the current quest being shrubles
- - Baffa Alg - Called once the current quest is complete. Runs through the list of following quests, and finds the quest that best pleases all players.
- - Player Type Manager - Handles the questionnaire and store the values of the player types (preference for violent games, rescuer games or collector games).
