# Scratch

This a scratch repo to store some of my work in no particular order.

ConnectGamePieces is a simple game engine to mimic the hasbrow board game connect four. 
I didnt call it connect four because I want it to be a more robust engine that will work for multiple columns, rows and number of connections to win.

The code is broken out between console game, engine and models - to seperate out each concern and isolate the game engine as the domain.

I have a lot of todo's, it is not yet complete for the engine and console game:
- complete buisness logic around game board creation (Guard from creating game boards with which can never be complete: one example: you shouldn't be able to create a 2 column and 2 row game with a 3 game pieces match requirment to win).
- implement diagnal connection detection algorithm
- more to come
- create proper .net library
- Unit tests

How to I compile the code & run (where you have your .net framework might difer): 

in a terminal:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ConnectGamePieces.cs 
.\ConnectGamePieces.exe
