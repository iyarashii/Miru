# Miru
Miru is a WPF app used to display data from the jikan MyAnimeList API.  
Idea for the app is based on [senpai](http://www.senpai.moe/) a great site that shows airing anime in a neat way.  
It lets you display airing animes from the user's MAL watching list. Also it can display list of TV animes from the current season.
This app displays airing animes for each day of the week in the specified timezone.  
It uses EF6 and SQL Server's LocalDB to store the data.
# Building
Build the solution with Visual Studio 2019 or newer.
# Installation guide
### Follow these steps to install on Windows 10:

1.  **Check if you have LocalDB installed on your PC:**
    1. Open Control Panel.
    2. Click Programs > Uninstall a program.
    3. Search for `localdb`.
    4. If you see `SQL Server 2016 LocalDB` or newer go to step 3 otherwise go to step 2. 
    ![Image of SQL Server 2016 LocalDB in control panel](https://i.imgur.com/3WApAAy.png)

2.  **Download and install LocalDB:**
    1. Download the LocalDB from [here](https://www.microsoft.com/en-us/download/confirmation.aspx?id=56840).
    2. Run the downloaded `SQLServer2016-SSEI-Expr.exe`.
    3. Select `Download Media`.
    ![Step 1 of installation](https://i.imgur.com/So90kQ2.png)
    4. Select preferred language and download location, select `LocalDB` as a package.
    ![Step 2 of installation](https://i.imgur.com/ryTqeU9.png)
    5. Click `Download` and wait for the download to complete.
    6. Run downloaded `SqlLocalDB.msi` installer.

3.  **Install the app:**
    1. Download the newest `Miru.zip` from the [releases](https://github.com/iyarashii/Miru/releases).
    2. Extract the archive and run `setup.exe`.
    3. After installation is complete wait for the app window to show.
# Usage
After starting the application (or after app installation), you should see an app window that looks like image below.
Theme and time zone should be the same as your system's by the default.
![Image of the app window after 1st run](https://i.imgur.com/U4mGtym.png)
Enter your username in the textbox and click `Sync` button or hit `Enter` key.
Wait for synchronization, after it you should see something like this:
![Image of synchronized app window](https://i.imgur.com/LzlEwHo.png)
You can click on the anime image to go to the MAL site associated with that anime and you can click on the
anime name next to the image to copy it to your clipboard.  
If anime list does not fit on the screen, you can scroll it up and down using a mouse scroll on each day anime list.   
You can use the `Time zone` drop-down to change the time zone in which the anime list is displayed.


`Displayed anime list` drop-down is used to change displayed list between the current season list, user's watching anime list and the user's watching airing anime list.  
You can get a list of all TV & ONA animes in the current season by clicking `Get current season list` button:
![Image of the current season](https://i.imgur.com/IiwY3Ju.png)
