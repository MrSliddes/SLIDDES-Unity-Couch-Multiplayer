using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Handles player display + camera
    /// </summary>
    public static class Display
    {
        /// <summary>
        /// Get the display values for the players
        /// </summary>
        /// <param name="amount">The amount of players</param>
        /// <returns>PlayerDisplay[]</returns>
        public static PlayerData[] GetPlayerDisplays(int displays, int players, SplitScreenMode splitScreenMode = SplitScreenMode.Horizontal)
        {
            PlayerData[] playerDisplays = new PlayerData[players];

            List<List<int>> targetDisplays = AssignDisplays(displays, players); /// targetDisplays[displayIndex][playerIndex]
            for(int i = 0; i < targetDisplays.Count; i++)
            {
                Rect[] viewPortRects = CalculateViewportRects(targetDisplays[i].Count, splitScreenMode);

                for(int j = 0; j < targetDisplays[i].Count; j++)
                {
                    int playerIndex = targetDisplays[i][j];
                    playerDisplays[playerIndex] = new PlayerData()
                    {
                        playerIndex = playerIndex,
                        cameraTargetDisplay = i,
                        cameraViewPortRect = viewPortRects[j]
                    };
                }
            }

            return playerDisplays;
        }


        /// <summary>
        /// Assign the displays to the players
        /// </summary>
        /// <param name="displays">The amount of displays</param>
        /// <param name="players">The amount of players</param>
        /// <returns>[displayIndex][playerIndex], the target display and the player that belongs to that display. Need to do it like this for calculate viewport rect cause we need to know how many players are on 1 display</returns>
        private static List<List<int>> AssignDisplays(int displays, int players)
        {
            // How many players on 1 screen
            int displayPopulation = Mathf.CeilToInt((float)players / displays);
            List<List<int>> targetDisplays = new List<List<int>>();

            int playerIndex = 0;
            for(int i = 0; i < displays; i++)
            {
                targetDisplays.Add(new List<int>());

                if(i > 0 && i <= UnityEngine.Display.displays.Length - 1) UnityEngine.Display.displays[i].Activate(); // display 0 always active /&& prevent editor warning when testing

                for(int j = 0; j < displayPopulation; j++)
                {
                    targetDisplays[i].Add(playerIndex);
                    playerIndex++;
                    if(playerIndex == players) break; // finished
                }
                if(playerIndex == players) break; // finished
            }
            return targetDisplays;
        }

        /// <summary>
        /// Calculate the viewport rect for a camera
        /// </summary>
        /// <param name="viewports">The amount of viewports used on 1 display</param>
        /// <returns>Rect[]</returns>
        private static Rect[] CalculateViewportRects(int viewports, SplitScreenMode splitScreenMode = SplitScreenMode.Horizontal)
        {
            // If only 1 viewport give back entire screen
            if(viewports == 1) return new Rect[] { new Rect(0, 0, 1, 1) };

            // Viewrects are distributed evenly
            Rect[] rects = new Rect[viewports];
            bool isEven = viewports % 2 == 0; // even amount of screens

            switch(splitScreenMode)
            {
                case SplitScreenMode.Horizontal:
                    float viewRectAmountTop, viewRectAmountBottom;
                    if(isEven)
                    {
                        viewRectAmountTop = 1f / (viewports / 2);
                        viewRectAmountBottom = viewRectAmountTop;
                    }
                    else
                    {
                        viewRectAmountTop = 1f / Mathf.Ceil(viewports / 2f);
                        viewRectAmountBottom = 1f / Mathf.FloorToInt(viewports / 2);
                    }

                    // The viewport rect x/y position starts at bottom left corner of the screen

                    float botX = 0;
                    for(int i = 0; i < viewports; i++)
                    {
                        // Check if the rect is placed top or bot
                        if(i < viewports / 2f)
                        {
                            // Top
                            rects[i].x = viewRectAmountTop * i;
                            rects[i].y = 0.5f;
                            rects[i].width = viewRectAmountTop;
                            rects[i].height = 0.5f;
                        }
                        else
                        {
                            // Bot
                            rects[i].x = viewRectAmountBottom * botX;
                            rects[i].y = 0;
                            rects[i].width = viewRectAmountBottom;
                            rects[i].height = 0.5f;
                            botX++;
                        }
                    }
                    break;
                case SplitScreenMode.Vertical:
                    float viewRectAmountLeft, viewRectAmountRight;
                    if(isEven)
                    {
                        viewRectAmountLeft = 1f / (viewports / 2);
                        viewRectAmountRight = viewRectAmountLeft;
                    }
                    else
                    {
                        viewRectAmountLeft = 1f / Mathf.Ceil(viewports / 2f);
                        viewRectAmountRight = 1f / Mathf.FloorToInt(viewports / 2);
                    }

                    // The viewport rect x/y position starts at bottom left corner of the screen

                    float rightY = 0;
                    for(int i = 0; i < viewports; i++)
                    {
                        // Check if the rect is placed left or right
                        if(i < viewports / 2f)
                        {
                            // Left
                            rects[i].x = 0;
                            rects[i].y = viewRectAmountLeft * i;
                            rects[i].width = 0.5f;
                            rects[i].height = viewRectAmountLeft;
                        }
                        else
                        {
                            // Right
                            rects[i].x = 0.5f;
                            rects[i].y = viewRectAmountRight * rightY;
                            rects[i].width = 0.5f;
                            rects[i].height = viewRectAmountRight;
                            rightY++;
                        }
                    }
                    break;
                default:
                    break;
            }

            return rects;
        }
    }
}
