﻿using System;
using System.IO;
using System.Collections.Generic;

class Scene 
{
    static Vector2 Resolution = Game.Resolution;
    static int scroll = 0;

    static Boolean win = false; // tells whether player passed the level
    static Boolean dead = true;
    static Boolean menuOpen = false; // tells us if menu is open or not
    static Boolean doorOpen = false; //tells whether door is open

    //textures for screens
    static Texture start = Engine.LoadTexture("start.png");
    static Texture menu = Engine.LoadTexture("menu screen.png");
    static Texture gameover = Engine.LoadTexture("game over.png");
    static Texture done = Engine.LoadTexture("level done.png");
    static Texture instruction1 = Engine.LoadTexture("instruction 1.png");
    static Texture instruction2 = Engine.LoadTexture("instruction 2.png");
    static Texture credits = Engine.LoadTexture("credits.png");

    static Texture healthSheet = Engine.LoadTexture("Health.png");

    //textures for basic buttons
    static Texture back = Engine.LoadTexture("back_next.png");
    static TextureMirror next = TextureMirror.Horizontal;
    static Texture menuButton = Engine.LoadTexture("menu.png");

    //textures for menu options
    static Texture resume = Engine.LoadTexture("Resume Button.png");
    static Texture infoButton = Engine.LoadTexture("Instructions Button.png");
    static Texture quit = Engine.LoadTexture("Quit Button.png");

    // door
    static Texture door = Engine.LoadTexture("door.png");

    static readonly Texture background = Engine.LoadTexture("Kirby red level background - Grayscale.png");
    static readonly Texture background2 = Engine.LoadTexture("Kirby red level background.png");
    static Font font = Engine.LoadFont("font.ttf", 20);
    static int numBlocksLevel1 = 202;

    static Block[] blocks;
    static Player player = new Player(blocks);
    static EnemyManager enemyManager;

    static int screen = 0;

    public Scene()
    {
        Setup();
    }

    public void Setup()
    {
        reload();
    }

    public static void Update()
    {
        //start screen
        if (screen == 0)
        {
            Engine.DrawTexture(start, Vector2.Zero);
            if (((Engine.GetMouseButtonDown(MouseButton.Left)) || Engine.GetMouseButtonDown(MouseButton.Right)) && (!menuOpen))
            {
                screen++;
            }

            doorOpen = false;
            player.points = 0;
        }

        //1st instructions screen
        else if (screen == 1)
        {

            Engine.DrawTexture(instruction1, Vector2.Zero);
            arrowButtons();
            menuButtons();
        }

        //2nd instructions screen
        else if (screen == 2)
        {
            Engine.DrawTexture(instruction2, Vector2.Zero);
            arrowButtons();
            menuButtons();
        }

        //level 1
        else if (screen == 3)
        {
            if (dead)
            {
                dead = false;
                scroll = 0;
                player = new Player(blocks);
                player.kPos.X = 260;
                player.kPos.Y = Resolution.Y / 2;
                enemyManager = new EnemyManager(player);
                enemyManager.initializeEnemies();
            }

            if (Engine.GetKeyDown(Key.R))
            {
                reload();
            }

            if (player.points >= 100)
            {

                Engine.DrawTexture(background2, Vector2.Zero);
                Engine.DrawTexture(door, new Vector2(8300 + scroll, 575), source: new Bounds2(75, 0, 75, 100));
                doorOpen = true;
            }
            else
            {
                Engine.DrawTexture(background, Vector2.Zero);
                Engine.DrawTexture(door, new Vector2(8300 + scroll, 575), source: new Bounds2(0, 0, 75, 100));
            }
            player.Update(scroll);
            enemyManager.Update(scroll);

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].draw(scroll);
            }

            int speed = 5;

            //adjust scroll
            if (Engine.GetKeyHeld(Key.Right) && player.getKPosition().X >= 940 && scroll >= -7425 && player.getMoveRight())
            {
                scroll -= speed;
            }
            if (Engine.GetKeyHeld(Key.Left) && player.getKPosition().X <= 255 && scroll <= 0 && player.getMoveLeft())
            {
                scroll += speed;
            }
            // draw the blocks
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].draw(scroll);
            }

            menuButtons();
            Engine.DrawString("Current Score: " + player.points.ToString(),
                new Vector2(1000, 50), Color.White, font);
            Bounds2 hFrameBounds = new Bounds2(((int)(player.health / 100)) * 110, 0, 110, 20);
            Engine.DrawTexture(healthSheet, new Vector2(1000, 80), source: hFrameBounds, size: new Vector2(220, 40));

            if (player.getKPosition().Y >= 1000)
            {
                dead = true;
            }

            if (dead == true)
            {
                win = false;
                screen++;
            }

            if ((player.kPos.X >= 8300 + scroll) &&
                (player.kPos.X <= 8375 + scroll) &&
                (player.kPos.Y >= 575) &&
                (player.kPos.Y <= 675) &&
                (doorOpen))
            {
                win = true;
                dead = true;
                screen++;
            }

        }

        //game over screen
        else if (screen == 4)
        {
            if (win)
            {
                screen++;
            }
            else
            {
                menuButtons();
                Engine.DrawTexture(gameover, Vector2.Zero);
                Engine.DrawString("High Score: " + player.highScore(), new Vector2(550, 100), Color.White, font);
                if ((Engine.GetMouseButtonDown(MouseButton.Left)) && (!menuOpen))
                {
                    screen = 6;
                }
            }

        }

        //level complete screen (end screen)
        else if (screen == 5)
        {
            menuButtons();
            Engine.DrawTexture(done, Vector2.Zero);
            Engine.DrawString("High Score: " + player.highScore(), new Vector2(550, 300), Color.White, font);
            if (Engine.GetMouseButtonDown(MouseButton.Left))
            {
                screen++;
            }
        }

        //end screen
        else if (screen == 6)
        {
            menuButtons();
            Engine.DrawTexture(credits, Vector2.Zero);
            if (Engine.GetMouseButtonDown(MouseButton.Left))
            {
                screen = 0;
            }
        }

        if (menuOpen)
        {

            openMenu();

        }
    }

    //back/next buttons for switching screens
    public static void arrowButtons()
    {
        Vector2 mouse = Engine.MousePosition;

        Bounds2 bBound;
        Bounds2 nBound;

        if ((mouse.X >= 30) && (mouse.X <= 70) &&
            (mouse.Y >= 20) && (mouse.Y <= 60))
        {
            bBound = new Bounds2(40, 0, 40, 40);
            if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetMouseButtonDown(MouseButton.Right))
            {
                screen--;
            }
        }
        else
        {
            bBound = new Bounds2(0, 0, 40, 40);
        }

        if ((mouse.X >= 1205) && (mouse.X <= 1245) &&
            (mouse.Y >= 20) && (mouse.Y <= 60))
        {
            nBound = new Bounds2(40, 0, 40, 40);
            if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetMouseButtonDown(MouseButton.Right))
            {
                screen++;
            }
        }
        else
        {
            nBound = new Bounds2(0, 0, 40, 40);
        }

        Engine.DrawTexture(back, new Vector2(30, 20), source: bBound);
        Engine.DrawTexture(back, new Vector2(1205, 20), source: nBound, mirror: next);

    }

    //menu button functions
    public static void menuButtons()
    {
        Vector2 mouse = Engine.MousePosition;
        Bounds2 mBound;

        if ((mouse.X >= 80) && (mouse.X <= 120) &&
            (mouse.Y >= 20) && (mouse.Y <= 60))
        {
            mBound = new Bounds2(40, 0, 40, 40);
            if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetMouseButtonDown(MouseButton.Right))
            {
                menuOpen = true;
            }
        }
        else
        {
            mBound = new Bounds2(0, 0, 40, 40);
        }

        Engine.DrawTexture(menuButton, new Vector2(80, 20), source: mBound);

    }

    //opens the menu
    public static void openMenu()
    {
        Engine.DrawTexture(menu, Vector2.Zero);

        Engine.DrawTexture(resume, new Vector2(0, 300));
        Engine.DrawTexture(infoButton, new Vector2(0, 400));
        Engine.DrawTexture(quit, new Vector2(0, 500));

        if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetMouseButtonDown(MouseButton.Right))
        {
            Vector2 mouse = Engine.MousePosition;

            //resume button
            if ((mouse.X >= 600) && (mouse.X <= 690) &&
            (mouse.Y >= 300) && (mouse.Y <= 330))
            {
                menuOpen = false;
            }

            //instructions button
            else if ((mouse.X >= 570) && (mouse.X <= 710) &&
            (mouse.Y >= 400) && (mouse.Y <= 430))
            {
                menuOpen = false;
                screen = 1;
            }

            //quit button (goes to start screen)
            else if ((mouse.X >= 620) && (mouse.X <= 670) &&
            (mouse.Y >= 500) && (mouse.Y <= 530))
            {
                menuOpen = false;
                screen = 0;
            }
        }
    }

    public static void reload()
    {
        Engine.reload();
        StreamReader sr = new StreamReader("assets/env coords.txt");
        blocks = new Block[numBlocksLevel1];
        for (int i = 0; i < blocks.Length; i++)
        {
            string line = sr.ReadLine();
            string[] nums = line.Split(' ');

            blocks[i] = new Block(Int32.Parse(nums[0]), Int32.Parse(nums[1]), Int32.Parse(nums[2]));
        }
        player.updateBlocks(blocks);
        sr.Close();
    }
}