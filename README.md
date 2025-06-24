Setup:

This project implements a tower defence game in Unity using C#. I am using Unity version 6000.1.8f1, and developing primarily for the Windows platform.

Overview:

Upon launching, you will see at the bottom six coloured buttons. Clicking each of these will open the tower type menu for that element, allowing you to select the type of tower you want to place. Once selected, when you hover over placeable areas of the map, you will see an outline of the tower to be placed. If it is green, that means you can place it there, if it is red that means either the placement is invalid, or you don’t have enough resources to build it. The sphere surrounding the tower represents its range. When placing a tower, you can see if the tower will be strengthened by its placement by looking for the words “Boosted” or “Negated” on the preview.

Once you have placed a tower, you can hover over it and click it to select it. Once selected, you can get close to it to see the upgrade menu, or you can press M to move it. If you press escape, it will deselect the tower, cancel tower placement, and remove your currently placeable tower.

Use the camera controls WASDQER to navigate around the map and plan accordingly. For resources, the upgrade menu will show how many resources the upgrade will need, and the tower element menu will also show how many resources each option needs.

When you click ready, you will join the fight. The initial spawning position can be seen on the map as a blue portal. Subsequent rounds will spawn you in your final position on the previous round. By using number keys, or scrolling with your mouse, you can choose an element to fire, and you will then hold down the left click button to charge an element to shoot that will be shot when the mouse is released. The damage done will depend on the charge, and the element selected.

The next round’s enemies can be seen by clicking the dropdown in the top right, the currently available resources can be seen on the top left, and your health can be seen in the top right.

Controls:

Movement: 
W – Move forward in FPS and tower defence view
A – Move left in FPS and tower defence view
S – Move backward in FPS and tower defence view
D – Move right in FPS and tower defence view
Space – Jump in FPS view
Q – Move down in tower defence view
E – Move up in tower defence view
R – Reset camera in tower defence view
Right Click – Drag to move camera orientation in tower defence view
Shift – Spring in FPS view and move faster in tower defence view

Interactions:
Numbers 1-6 – Set element to shoot in FPS view
Mouse Scroll Wheel – Cycle element to shoot in FPS view
Left click – Charge and shoot in FPS view or place/select tower in tower defence view
M – Move tower in tower defence view
