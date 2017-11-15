# CLAHE CS implementation
This repo is a simple C# translation of the ANSI C code from the article ***"Contrast Limited Adaptive Histogram Equalization"*** by Karel Zuiderveld, in "Graphics Gems IV", Academic Press, 1994
There is a (probably much better) implementation in OpenCV but I needed the code to run on its own without any library dependency.

The image hereunder shows the application of the algorithm to one of my images (the GUI has slightly evolved).
<a href="url"><img src="http://christianjaques.ch/wp-content/uploads/2017/11/clahe_gui.jpg" height="400" width="600" ></a>

The code should work on all .NET versions (it makes use of winforms).

## To-dos
This code is minimalist (if not bad), here are some things I'll do when/if I find the time: 
* Allow to load any image
* Debug cases of image sizes that arent powers of 2
* Work on any type of data, not only 8 bits
* Extend algo to color images
* ...

## Dependencies
None, that was the goal.

## Ackowledgment

## License
You are free to use and modify the code here, just know that it comes as is and there's NO WARRANTY and I'd appreciate that you mention me if you do, but there's no obligation.

## Contact
Do not hesitate to contact me if you need some help, you may find my contact [here](http://christianjaques.ch/contact/). 
