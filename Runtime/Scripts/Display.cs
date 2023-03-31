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
        public static PlayerData[] GetPlayerDisplays(int displays, int players)
        {
            PlayerData[] playerDisplays = new PlayerData[players];

            List<List<int>> targetDisplays = AssignDisplays(displays, players); /// targetDisplays[displayIndex][playerIndex]
            for(int i = 0; i < targetDisplays.Count; i++)
            {
                Rect[] viewPortRects = CalculateViewportRects(targetDisplays[i].Count);

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
        private static Rect[] CalculateViewportRects(int viewports)
        {
            // If only 1 viewport give back entire screen
            if(viewports == 1) return new Rect[] { new Rect(0, 0, 1, 1) };

            // Viewrects are distributed evenly
            float topSpace, bottomSpace;
            bool isEven = viewports % 2 == 0; // even amount of screens
            if(isEven)
            {
                topSpace = 1f / (viewports / 2);
                bottomSpace = topSpace;
            }
            else
            {
                topSpace = 1f / Mathf.Ceil(viewports / 2f);
                bottomSpace = 1f / Mathf.FloorToInt(viewports / 2);
            }

            // The viewport rect x/y position starts at bottom left corner of the screen

            float botX = 0;
            Rect[] rects = new Rect[viewports];
            for(int i = 0; i < viewports; i++)
            {
                // Check if the rect is placed top or bot
                if(i < viewports / 2f)
                {
                    // Top
                    rects[i].x = topSpace * i;
                    rects[i].y = 0.5f;
                    rects[i].width = topSpace;
                    rects[i].height = 0.5f;
                }
                else
                {
                    // Bot
                    rects[i].x = bottomSpace * botX;
                    rects[i].y = 0;
                    rects[i].width = bottomSpace;
                    rects[i].height = 0.5f;
                    botX++;
                }
            }

            return rects;
        }
    }
}
