# CLAHE CS implementation
This repo is a simple translation of the ANSI C code from the article ***"Contrast Limited Adaptive Histogram Equalization"*** by Karel Zuiderveld, karel@cv.ruu.nl in "Graphics Gems IV", Academic Press, 1994
There is a (probably much better) implementation in OpenCV but I needed the code to run on its own without any library dependency.

The image hereunder shows the application of the algorithm to one of my image.
<a href="url"><img src="http://christianjaques.ch/wp-content/uploads/2017/11/clahe_gui.jpg" height="400" width="600" ></a>

## To-dos
This code is minimalist (if not bad), here are some things I'll do when/if I find the time: 
* Remove hard-coded path to image
* Put all parameters of the CLAHE algorithm in the GUI
* Allow to load any image
* Extend algo to color images
* ...

## Dependencies
None, that was the goal.

## Ackowledgment

## License
You are free to use and modify the code here, just know that it comes as is and there's NO WARRANTY and I'd appreciate that you mention me if you do, but there's no obligation.
