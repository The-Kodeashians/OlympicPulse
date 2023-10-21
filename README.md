# ![OlympicPulse](https://github.com/The-Kodeashians/OlympicPulse/assets/22523084/3d798d59-847a-4fd5-bd38-3f3b73107257)
An AR app for mobile targeting amateur athletes visiting the Olympics, offering an immersive experience with an interactive AR stadium and and AR games. This app was developed for capstone UQ Course [DECO3801](https://my.uq.edu.au/programs-courses/course.html?course_code=DECO3801).


![image](https://github.com/The-Kodeashians/OlympicPulse/assets/22523084/d3d27246-c981-4fd9-be48-5afb7fe9cadb)
![OlympicPulse Kickstarter Video](https://www.youtube.com/watch?v=8Sh9KiPCjKw)

### Objective
OlympicPulse aims to create an engaging and continuous experience for Olympic ticket holders, with a special focus on amateur athletes who aspire to compete against the best. The app offers a seamless journey from the moment the ticket is purchased to the day of the event. Through a blend of gamification, daily quests, and a streak reward system, users are encouraged to interact with the app regularly. A token system also rewards user engagement, allowing them to purchase exclusive athletes and skins, thereby personalising their Olympic experience.

### Current Key Features
Through a blend of interactive content and real-time engagement, OlympicPulse builds anticipation and excitement leading up to the event.

- **QR Ticket scan**: Users must scan their valid Olympic ticket's QR code to proceed through to the rest of the app.
- **Homepage**: Displays the user's ticket information, their profile and a countdown to when their event starts.
- **AR Interactive Map**: Upon scanning their ticket, users receive an AR interactive map of the stadium, providing valuable insights, directions, and information.
- **AR Sprinting Game**: Users can race alongside AR sprinters and world record holders.
- **Community Engagement**: The user can save a recording of their runs and share them on various social media platforms if they wish.

#### Core Technologies
Core Technologies
- Unity: OlympicPulse is built on the Unity Engine, specifically version 2022.3.10f1, providing a powerful framework for AR experiences.
- C#: All scripts are written in C#, leveraging Unity's scripting API for game mechanics and user interactions. The one exception to this is the OBjective-C plugin for iOS screen recording, preview and saving.

#### APIs and Libraries
- Unity AR Foundation: Used for building the AR functionalities, including AR plane detection and 3D object placement.
- ZXing (Zebra Crossing): Utilized for QR code scanning functionalities in the TicketScan scene.
- Unity's TextMeshPro: Used for advanced text rendering and manipulation.
- Unity's UI System: Employed for crafting the user interface across various scenes.
- Figma Unity Bridge: A custom plugin that allows for importing Figma designs directly into Unity. Used in the PersonalisedWelcome scene.

#### Deployment Platforms
- Currently only deployed and tested on iOS. Although most features should work with Android as well.

### Scenes Description

All scenes relevant to the app are located in /Assets/_OlympicPulse/Scenes.

- **Intro**: Features the 'IntroManager' prefab with the OP_Intro_Script. Acts as the splash screen on app startup. Directs users either to the 'TicketScan' scene (for first-time users) or the 'Main' scene (for returning users).
- **TicketScan**: Holds the 'TicketScanner' prefab with the OP_QR_Scan script. After obtaining camera permissions, it scans for a QR code, stores the relevant information, and transitions to the 'PersonalisedWelcome' scene.
- **PersonalisedWelcome**: Contains the 'WelcomeManager' prefab. Displays the information scanned from the ticket and a countdown to the event. On swiping up, it redirects users to the 'Main' scene.
- **Main**: Contains the AR functionalities. Upon detecting planes, the 'InteractiveMap' prefab with its attached script allows users to place or remove the 3D object onto the detected plane by tapping the screen. Also has interactive icons for information and mini games.
- **SprintingSelection**: The user selects which sprinter to go up against.
- **Sprinting**: The AR sprinting game.

### Scripts Description

All C# scripts are located in /Assets/_OlympicPulse/Scripts

- **OP_AR_Sprint_Controller**: Manages the AR sprinting game, controlling the sprinter's behaviour and interactions.
- **OP_Interactive_Map**: Handles the AR interactive map feature, including user interactions like tapping.
- **OP_Intro_Script**: Manages the splash screen and directs users to the appropriate next scene.
- **OP_Map_Canvas_Manager**: Manages the UI canvas for the AR interactive map.
- **OP_QR_Scan**: Manages the QR code scanning functionality and stores ticket information.
- **OP_Screen_Recorder**: Handles screen recording features for the app.
- **OP_Sprinter_Details**: Stores and manages the details of the sprinters available for the AR sprinting game.
- **OP_Sprinter_Map_Run**: Controls the sprinter's running behavior on the map. (not implemented)
- **OP_Sprinter_Selection_Manager**: Manages the selection process for choosing a sprinter in the AR sprinting game.
- **OP_Welcome_Script**: Displays personalized welcome messages and countdowns based on scanned ticket information.
- **/XCode/OP_Screen_Recorder/OP_Screen_Recorder.m and .h**: An Objective-C plugin for the recording, preview and save feature for use in the plugins folder.

### Attributions (Free assets used)
- [Sporty Girl by Jellymobile](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/sporty-girl-29666)
- [Sprinting Icon by Flat Icons](https://www.flaticon.com/free-icon/sprinter_3343936?term=sprint&page=1&position=3&origin=tag&related_id=3343936)
- [Seats Icon by Freepik](https://www.flaticon.com/free-icon/seats_919470?term=seats&page=1&position=11&origin=search&related_id=919470)
- [SimplePoly City - Low Poly Assets Stadium and Trees](https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899)
- [Props for Track Environment Lowpoly Free](https://assetstore.unity.com/packages/3d/props/props-for-track-environment-lowpoly-free-211494)
- [UnityFigmaBridge](https://github.com/simonoliver/UnityFigmaBridge)

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
