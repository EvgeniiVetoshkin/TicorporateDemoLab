Thankyou for your interest and support, ill try to be brief.

If you have any issues not covered by these instructions, google or the store page, you can request help from my site here: Digineaux.com or directly via email: Digineaux@gmail.com
We ussually answer within 24 hours and would much rather fix your issue and make it easier for people to resolve it themselves in furture, then to force our users to request a refund.

Get it working!:
Drag the prefab into the scene, ensure it your only or main camera and it's ready to go!
Wasn't that easy?

The default Controls:
Left Shift = Boost
left Control = slow down
WASD and arrow keys = Move/pan camera on the horizontal plane
+ and - (next to backspace and numpad +-) = Zoom in and out respectively
R, numpad 8 = rotate up
f, numpad 2 = rotate down
q, numpad 4 = rotate left
e, numpad 6 = rotate right
With edge panning enabled, moving your mouse to the edge of the screen will pan in that direction.

Require input manager to be set up, instructions below:
left alt and middle mouse click = rotating with mouse
With toggle mouse rotation ticked in the inspector, it is toggled on and off, rather than needing to be held.

Make a copy of the script if you intend to make any changes too it. It is recommended to alter features using the inspector options, rather than editing the prefab or script unless you really know what your doing or are following these instructions.

To change these controls ctrl+F search for "hardocded inputs", you can add or remove as many as you want, as long theres atleast one per control.
If you wish to disable one in the script, set it's keycode to "none".
Though i reccomend just unticking the feature in the inspector window instead of editing the script.'

This script also supports Unities Input Manager. There are hundreds of tutorials on how to use the input manager if your interested.
I will only explain how to enable Input manager support for this script.

step one: Simply uncomment the input manager section of the script.
do this by deleting the the / and /.
You can find this section by ctrl+f and searching "input manager inputs""

Step two: Create Input Manager Axis, with names matching the text in quotes
Or change the text in quotes to match Input Manager Axis
Remember to enable Input Manager, in the scripts inspector options

﻿

Rebuilding the prefab
I recommend just using the prefab, there's nothing to gain from setting it up manually. But if for some reason someone does want to, the steps are below;

Recreating the prefab and how it works (note: the names don't really matter, it's just for consistency with tutorial, tooltip and commented terms):

Step one: Create an empty object in the scene heirachy called "RTSCamRig"

Step two: Create another empty object called RotRig and make it a child of cameraRig

Step three: Add a camera object as a child of rotRig

Step four: Set the cameras X rotation to 90 and it's Y position to 10. Note if you don't do this rotation may not work properly. If you don't like the default position change it in the scripts inspector, unless you really know what your doing. It is safe to move the cameraRig however you want though.

Step five: Attach the script to the cameraRig

Step six: In the scripts inspector assign the rotRig to the Rot Rig variable and the camera to the "Camera Transform" Variable. They are located at the bottom of the inspector options.

The rig acts like a sort of gyroscope, asigining different rotational axis to the different objects, so they don't interfere with eachothers local and global coordinates. The cameraRig gets panned and rotated horizontally, the rotRig handles vertical rotation and the camera moves toward and away from the cameraRig for zooming.

