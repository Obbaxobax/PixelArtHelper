# PixelArtHelper
A simple Terraria Mod with tools to allow for an easier process when making in-game pixel art.
## Reporting bugs
If you want to report a bug, open an issue and include your client.log file (Or copy the error text from it). 
The logs can be found by going to Documents\My Games\Terraria\tModLoader and finding the link to the folder.
It may take me a while to see the issue, so don't expect a response immediately.
## Guide
### Terminology
I will be referring to the building outline as "the hologram" for this guide.
### Usage
With this new update, the mod has become much simpler to use. Taking a page out of UltraSonic's book, the mod now has a UI which controls its entire functionality and can be dragged anywhere on the screen.
The menu can be opened by pressing a key bind, which by default is "P". If nothing happens when you press P, you might need to manually set a key in the controls. The menu can be toggled on and off, and can be closed while the hologram is active
The menu starts on the main page, which may seem rather overwhelming at first, but hovering over everything will give you a basic description on what it does. 
However, if that is still not enough I will write more detailed descriptions here.

* The first two boxes are the width and height respectively. These are the dimensions, in tiles, of the pixel art. These values do not need to be set, and they default to using one pixel per tile.\
It is important to note that a larger image will take longer to process and will require considerably more blocks, so I strongly recommend you do not go above 100x100.\
![image showing width and height boxes](https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/4e7b25f9-ca3a-4258-b1b0-a42049ae58ce)
* The next text box is the name of which the image will be saved as. Avoid duplicate names (I should test/prevent this) and long names. This text field must be filled out, otherwise, the image will not be saved.
  * The button next to it is a simple toggle for whether or not you want the build to require paints. By default, it is turned off, but using paints may give you a more accurate result.\
  
  ![image showing save name box and paint toggle button](https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/fe149076-085d-45d9-8efb-a37633fed26c)
* The below button will take you to a page of the menu which will allow you to set the blocks which you do not want to use. For some, this task may be tedious, and I have gone through the effort of pre-setting some values which I believe should be sufficient. You may want to change these values if you want to use blocks which are easily obtainable for your stage in progression. To return, press the red button in the top right.\
  ![image showing exceptions button](https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/1c9f93df-bc72-4be1-b66d-4b21684a2ab9)
  <img src="https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/b39dbae0-788b-42b2-999b-1563edf128c5" alt="image showing exceptions page" height="400"/>

* The last two remaining elements are the most important. The text field on the right is the location in which you will specify your link or file path. If the field is empty, the button can be used to paste the text which is currently copied. However, if the field is not empty, the button will attempt to grab the image from the specified location, adding it to the list if it is successful.\
![image showing add button and link text field](https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/360ade90-e3f9-4a1e-9221-0b3b8078c7bf)
* Finally, at the bottom of the main page is where the list of images will be displayed. By default it is empty, but after you input your first image, a box should appear with the name of the image. Clicking this box will start processing the image, and after a short while it should say "Click to place the hologram!". Once this appears, you can click anywhere on your screen to place the hologram. The transparent black box outlines the space which the art will take up. Upon clicking, the menu should change tabs and you should see pixels representing the image. You can return to the previous menu and disable the hologram by pressing the red button in the upper right.\

  ![image showing list with one element](https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/b7cefcc7-f8ef-43e4-bb7d-1a075abfebcf)


At this point, you know the basics and are ready to start, but you may have noticed the other button present after you load a hologram. Clicking that button will toggle between modes, with the default mode showing the whole hologram. The other mode will only show pixels if they match the held block. I.E. if my pixel art required Diamond Gemspark, holding that block would display all the places I need to place it. Naturally, if you aren't holding a correct block, this will make the hologram completely invisible.\

<p align="center">
  <img src="https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/17418840-7e20-4349-8e62-9a39521162ba" alt="image showing hologram in normal mode" width="200"/>
  <img src="https://github.com/Obbaxobax/PixelArtHelper/assets/70305616/0096145a-8fc0-4d92-8f43-61440dedc2c0" alt="image showing hologram in alternate mode" width="200"/>
</p>
<p align="center">normal mode vs alternate mode</p>


## Credits
Mod created by [Obbax](https://github.com/Obbaxobax)\
Box.png and and main draw box function from [UltraSonic](https://github.com/OliHeamon/UltraSonic) (Really cool mod you should use)
