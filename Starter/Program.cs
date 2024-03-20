using System;

Random random = new Random();
Console.CursorVisible = false;
int height = Console.WindowHeight - 1;
int width = Console.WindowWidth - 5;
bool shouldExit = false;

// Console position of the player
int playerX = 0;
int playerY = 0;

// Console position of the food
int foodX = 0;
int foodY = 0;

// Available player and food strings
string[] states = {"('-')", "(^-^)", "(X_X)"};
string[] foods = {"@@@@@", "$$$$$", "#####"};

// Current player string displayed in the Console
string player = states[0];

// Index of the current food
int food = 0;

// Index of the frozen player state
const int frozenPlayerStateIndex = 2;
// Index of the speed player state
const int speedPlayerStateIndex = 1;
// Movement speed when player is in speed state
const int speedPlayerStateMovementSpeed = 3;

InitializeGame();
while (!shouldExit) 
{
    if (TerminalResized())
    {
        Console.Clear();
        Console.WriteLine("Console was resized. Program exiting.");
        shouldExit = true;
    }
    else if (playerShouldFreeze())
        FreezePlayer();
    else if (isPlayerSpeedState())
        Move(horizontalSpeed:speedPlayerStateMovementSpeed);
    else
        Move();

    if (isFoodConsumed())
    {
        ChangePlayer();
        ShowFood();
    }

}

// Returns true if the Terminal was resized 
bool TerminalResized() 
{
    return height != Console.WindowHeight - 1 || width != Console.WindowWidth - 5;
}

// Displays random food at a random location
void ShowFood() 
{
    // Update food to a random index
    food = random.Next(0, foods.Length);

    // Update food position to a random location
    foodX = random.Next(0, width - player.Length);
    foodY = random.Next(0, height - 1);

    // Display the food at the location
    Console.SetCursorPosition(foodX, foodY);
    Console.Write(foods[food]);
}

// Changes the player to match the food consumed
void ChangePlayer() 
{
    player = states[food];
    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);
}

// Temporarily stops the player from moving
void FreezePlayer() 
{
    System.Threading.Thread.Sleep(1000);
    player = states[0];
}

// Reads directional input from the Console and moves the player
void Move(bool exitOnNonDirectionalKeyInput = false, int horizontalSpeed = 1) 
{
    int lastX = playerX;
    int lastY = playerY;
    
    switch (Console.ReadKey(true).Key) 
    {
        case ConsoleKey.UpArrow:
            playerY--; 
            break;
		case ConsoleKey.DownArrow: 
            playerY++; 
            break;
		case ConsoleKey.LeftArrow:  
            playerX -= horizontalSpeed; 
            break;
		case ConsoleKey.RightArrow: 
            playerX += horizontalSpeed; 
            break;
		case ConsoleKey.Escape:
            shouldExit = true; 
            break;
        default:
            shouldExit = exitOnNonDirectionalKeyInput ? true : false;
            break;
    }

    // Clear the characters at the previous position
    Console.SetCursorPosition(lastX, lastY);
    for (int i = 0; i < player.Length; i++) 
    {
        Console.Write(" ");
    }

    // Keep player position within the bounds of the Terminal window
    playerX = (playerX < 0) ? 0 : (playerX >= width ? width : playerX);
    playerY = (playerY < 0) ? 0 : (playerY >= height ? height : playerY);

    // Draw the player at the new location
    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);
}

// Clears the console, displays the food and player
void InitializeGame() 
{
    Console.Clear();
    ShowFood();
    Console.SetCursorPosition(0, 0);
    Console.Write(player);
}

// Determines whether there is a collision between player and food coordinates
bool isFoodConsumed ()
{
    int playerSizeXOffset = player.Length - 1;
    int foodSizeXOffset = foods[food].Length - 1;

    return playerY == foodY && ((Math.Abs(playerX - foodX) >= 0 && Math.Abs(playerX - foodX) <= playerSizeXOffset) || (Math.Abs(foodX - playerX) >= 0 && Math.Abs(foodX - playerX) <= foodSizeXOffset));
}

// Determines if the player should freeze because he ate bad food
bool playerShouldFreeze()
{
    return player == states[frozenPlayerStateIndex];
}

// Determines if the player is in speed state because he ate good food
bool isPlayerSpeedState()
{
    return player == states[speedPlayerStateIndex];
}