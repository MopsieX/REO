# RealmEye Overlay

This is an early version. There may be bugs that I haven't encountered. Please report all bugs here.



### Features:
- Customizable hotkey to toggle the overlay
- Customizable hotkey to reset realmeye password by either copying “/tell MrEyeBall password” to your clipboard or by typing it in game
- Custom CSS style sheets to change how RealmEye looks
- Message button on each offer to either copy "/tell {username} " to your clipboard or it will use PostMessage(Windows only) to type "/tell {username} " in game
- Auto bump your offers every 60 seconds(Only bumps offers if you're on your edit offers page)


### How to build:
Download the source code and extract it somewhere
Install Node.JS
If you're on Windows run the file "Setup-win.bat"
If you're on Mac run the file "Setup-Mac.sh"
The script should install the required 3rd party libraries.
Once it finishes installing the libraries you should be able to open the project in Visual Studio and build it.