# MilliGolf

The Hollow Knight golf mod.

## Custom Courses

You can create your own courses in any existing room through the *CustomCourses.json* file in your Mods folder. These courses do not track your high scores and will not be included in rando integrations.
Two sample courses have been provided. You can reference them for information on how to format the course data.
*Disclaimer - You cannot have more than one course use the same room*

__Json Guide__
The following fields are required:
- **name**: The name that appears over the door in the Hall of Golf
- **scene**: The name of the room where you want to golf
- **startTransition**: The name of the transition where you enter the room
- **millibelleSpawn**: The x and y coordinates where Millibelle will start
- **knightSpawn**: The x and y coordinates you will return to if you use the quick-reset feature (recommended to be at the entrance transition)
- **doorColor**: The RGB values of the door glow in the Hall of Golf
- **flagData**: Which flag image to use, as well as the x and y coordinates for placing it
	- The **filename** field can be "*flagSignE*", "*flagSignSE*", "*flagSignSW*", "*flagSignW*", or "*flagSignNW*" since these exist in the original courses
- **holeTarget\***: The name of the game object that Millibelle will treat as the goal (usually another transition)
	- \**This field is not required if you set a customHolePosition and will be overwritten in that case*

The following fields are optional:
- **secondaryDoorColor**: The RGB values of the right-side glow in the Hall's door, defaults to **doorColor** if unassigned
- **customHolePosition**: This allows you to define a new hitbox to be used as the target hole (*overwrites **holeTarget***)
	- Centered at (**x**,**y**) with the width/height of (**sx**,**sy**)
- **objectsToDisable**: A list of game objects that exist in the vanilla room but will not appear in your custom course
- **childrenToDisable**: If you want to disable certain parts of an object but not the entire object, and if the names of the child objects are not unique, you can use this field to clarify
	- Defined as a dictionary that can specify multiple parent objects and multiple child objects
	- *This will rarely be needed, the sample course example is inefficient but was provided to demonstrate formatting*

## Integrations

### Randomizer 4

The MilliGolf content can be used as a Randomizer 4 connection, which allows players to:

- Randomize access to each course, making you require one obtain to enter each course.
- Randomize course completion, which will make you obtain an item the first time you clear each course, and prevent you from viewing your score on the board until the completion check is obtained.
- Randomize global goals. MilliGolf does have special achievements for clearing all courses in less than N hits. The obtention of said marks can be randomized, making you obtain an item when reaching the course thresholds and checking the scoreboard instead.
- Randomize course transitions, which will add the tent door as well as all the individual course doors to the transition pool for room or door randomizers.

Warning: enabling Door Rando (from TrandoPlus), Course Access and Course Transitions will likely cause generation to take much longer than usual.