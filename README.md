# OlympicPulse
An AR app for mobile targeting amateur athletes visiting the Olympics, offering an immersive experience with the stadium and world-record holders.

## Project Overview

### Objective
OlympicPulse aims to provide an unforgettable experience for Olympic ticket holders, especially amateur athletes. The app leverages Augmented Reality (AR) to show a 3D representation of the stadium and an AR sprinting game where users can witness world record holders run side by side with them, or someone a bit slower to match their skill level. Users can either hold the phone themselves to view a 3D representation of the athlete running alongside them, or they can have a friend record the experience for later viewing.

### Key Features
Through a blend of interactive content and real-time engagement, OlympicPulse builds anticipation and excitement leading up to the event.

- **AR Interactive Map**: Upon scanning their ticket, users receive an AR interactive map of the stadium, providing valuable insights, directions, and information.
- **AR Sprinting Game**: Users can race alongside world record holders from different decades and experience their incredible speeds firsthand.
- **User Interaction**: Depending on the scenario, users can either hold the phone for a personal experience or have someone else record their race for a third-person perspective.


### Scenes Description

All scenes relevant to the app are located in /Assets/_OlympicPulse/Scenes.

- **Intro**: Features the 'IntroManager' prefab with the OP_Intro_Script. Acts as the splash screen on app startup. Directs users either to the 'TicketScan' scene (for first-time users) or the 'Main' scene (for returning users).
- **TicketScan**: Holds the 'TicketScanner' prefab with the OP_QR_Scan script. After obtaining camera permissions, it scans for a QR code, stores the relevant information, and transitions to the 'PersonalisedWelcome' scene.
- **PersonalisedWelcome**: Contains the 'WelcomeManager' prefab. Displays the information scanned from the ticket and a countdown to the event. On swiping up, it redirects users to the 'Main' scene.
- **Main**: Contains the AR functionalities. Upon detecting planes, the 'InteractiveMap' prefab with its attached script allows users to place or remove the 3D object onto the detected plane by tapping the screen. Also has interactive icons for information and mini games.
- **SprintingSelection**: The user selects which sprinter to go up against.
- **Sprinting**: The AR sprinting game.


## Project Setup

Follow these steps to set up the OlympicPulse project on your local machine:

1. **Download Unity Hub**: Visit [Unity's download page](https://unity.com/download) and install Unity Hub.
2. **Install Unity Version**: Install Unity version 2022.3.10f1 through Unity Hub. Make sure to add iOS build support and Android Build Support plus openJDK.
3. **Add Build Support Modules**: Ensure to add both Android Build Support and iOS Build Support during installation.
4. **Install Git LFS**: Before cloning the repository, install [Git LFS](https://git-lfs.com/).
5. **Optional GitHub Desktop**: Optionally, you can use [GitHub Desktop](https://desktop.github.com/) to clone the repository and manage commits.
6. **Clone the Repository**: Clone the OlympicPulse repository to your local machine.
7. **Open the Project in Unity**:
   - Open Unity Hub.
   - Click on "Projects."
   - Click "Open."
   - Locate the root folder of the OlympicPulse repository and select it.
