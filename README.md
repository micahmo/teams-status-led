# teams-status-led
Light up an LED with an Arduino board to reflect your Microsoft Teams status

# Arduino Setup
First, download and install the [Arduino IDE](https://www.arduino.cc/en/main/software).

Download the latest version of [arduino-status.ino](https://github.com/micahmo/teams-status-led/blob/main/arduino-status/arduino-status.ino). Load the .ino file into the Arduino IDE. Choose "Verify", and then with your Arduino plugged in, choose "Upload".

![image](https://user-images.githubusercontent.com/7417301/125371437-b9daa380-e34e-11eb-8d58-1bbe9d9e58eb.png)

**Note: The code assumes that the the LED is plugged into terminal 13 and GRD. You can adjust the Arduino code if your LED is plugged in differently by changing the LED constant at the beginning.**

# TeamsStatusLed

Download the [latest portable release](https://github.com/micahmo/teams-status-led/releases/tag/latest). Extract the .zip. Run `TeamsStatusLed.exe`. An icon will appear in the system tray. From there you can see the current status and modify the settings from the context menu.

![image](https://user-images.githubusercontent.com/7417301/125371694-4e450600-e34f-11eb-8ac9-4009de6b877f.png)

# Known Issues

In some rare cases, TeamsStatusLed can cause Teams to be unable to start. In that case, exit TeamsStatusLed, start Teams, then restart TeamsStatusLed.
