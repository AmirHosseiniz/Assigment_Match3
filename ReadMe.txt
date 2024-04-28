**** Match 3 Assigment ****

================================================================================

In order to test the levels : 

1) In project window, go to "Assets/_Scriptables/Levels", choose the level data you want.

2) In game scene, find the "LevelLoader" gameobject under "Match3Group" and drag and drop the level data onto the "EditorLevel" field in "LevelLoader" script.

3) play.

================================================================================

In order to create a level data : 

1) Right click in project window, goto "Create,GamePlay,ScriptableLevelData" and create a data.

2) In level data inspector set the "colorcount" (color count means how many diffrent colors you want in your level), "Width" and "Height" of the grid. 

3) In the first selectionGrid below , you can choose the grid cell type **(types are displayed in this order : Empty,Hole,Color,Block)**.

4) If you choose color type , you should choose the color itself too. (black color means RANDOM)

5) Click on the desired grid cell to assign the type.

..................................................................................

**: Empty : Empty cell without color.
    Hole : That grid cell will not exist and displayed like there is a hole there.
    color : match 3 object color.
    Block : A beakable block of wood that will break if a neighbor cell match. (this might be buggy due to lack of tests and lack of time.)

================================================================================

**  Some of the used sprites are downloaded from google and pinterest.

================================================================================

**  In this project Scripts are mainly devided to 3 major category : 1) Datas 2) Logics 3) Utilities

================================================================================

** Input system is capable of both "Dragging" and "Click-Clcik" behaviour for Swapping.

================================================================================