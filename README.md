# ARRR PIRATES

ARRR PIRATES is an AR mobile game developed in Unity 3D using ARCore.

## How to play

Firstly, the player needs to move their phone around, giving ARCore a chance to detect surfaces.
They can then click anywhere to place the gameboard, consisting of a cutout of the sea with a pirate ship on top.

The gameboard may be relocated at any time by clicking the replace button in the upper-right corner of the screen.

The goal of the game is to collect all 10 **Treasures** located on the sea floor.
These can be located by following the needle of the **Treasure Compass**, located in the front of the ship.
The **Treasures** are below sea level which requires the player to bring their phone under water (virtually) to see them.

This is because the virtual camera's position corresponds to the phones real-world position, resulting in the m navigating the currently visible space (see below) by moving their phone.

A **Diver** can than ordered to collect a located **Treasure** by first selecting the **Diver** by clicking them followed by targeting the **Treasure** by clicking it.
This will make the **Diver** climb down from the ship and swim to the **Treasure** before returning to the ship and collecting it.

Objects in the water are invisible when outside the sea cutout.
Therefore, the player needs to move the ship to reveal any **Treasures** outside of view by selecting the **Helmsman**.
This will grant the player control of the ship, allowing it to be rotated by tilting their phone as it sails forward.
The ship will stop when the **Helmsman** is deselected.

A **Helmsman** or **Diver**, is deselected when they are clicked again, when another **Helmsman** or **Diver** is selected or when the player clicks anywhere else.
Effectivelly, deselction is done by clicking anywhere at all.

When all 10 treasures have been collected, a victory screen is shown giving the player the option to restart.

## Feedback

A selected **Helmsman** and **Diver** is enclosed by two square brackets "[ ]" and a targeted treasure are enclosed by a reticle.

When the ship is moving, it its positition is static while everything in the sea moves relative to it instead.
To still give the feeling of the ship moving, a wake effekt is produced behind it, which turs as the ship turns.
There are also planks spread out on the water surface and sea floor for this purpose.

Planks stop appearing when the ship has sailed too far away from where treasures can be found, resulting in a empty environment indicating that the ship is "outside" the level.

As **Treasures** are collected, a gold pile on the ship deck increases in size.

There are also feedback in the UI including a label "x/10" where x is the number of collected **Treasures** as well as a label indicating what is currently selected (a **Helmsman**/a **Diver**/nothing).

When all 10 treasures have been collected, a victory screen is shown.

## In-Game Instructions

Before the first surface is found, an instruction message asking the player to move their phone in circles is displayed accompanied by an icon illustrating this.

When a surface has been found, this message is replaced by message requesting the player to click on a surface to place the board.

The board will always rotated with the ship facing rightwards allowing the **Helmsman**, all **Divers** and the **Treasure Compass** to be visible.
The labels "Helmsman", "Divers" and "Treasure Compass" are displayed above them, with matching colors (green, red and gold), making it clear what is what.

The **Helmsman** and **Divers** also have a "CLICK ME!" label following them.

When the **Helmsman** is clicked/selected for the first time, a message describing that the ship now is steered by tilting the phone is displayed.

When a **Diver** is clicked/selected for the first time, a message requesting the player to click a **Treasure** chest is displayed, prompting them to search for chests.
If their initial camera postition after placing the board was under water, which is often the case, they might even remeber having seen chests previously, helping them in their search.

## AR-Specific Details

The clickable hitbox of the **Helmsman**, **Divers** and **Treasures** scale linearly by the distance to the camera, making it easier to lick them without moving the phone.

The replace board feature allows the player to change location for more comfort and varied real-world height of operation, at their own discretion.
