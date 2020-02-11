# Miru
Miru is a WPF app used to display data from the jikan MyAnimeList API.  
Idea for the app is based on [senpai](http://www.senpai.moe/) a great site that shows airing anime in a neat way.  
For now, it lets you display airing animes from user's MAL watching list.
This app displays airing animes for each day of the week in the GMT+1 timezone (more time zone options coming soon).  
It uses EF6 and SQL Server's LocalDB to store the data.
# Building
Build the solution with Visual Studio 2019 or newer.
# Installation guide
### Follow these steps to install on Windows:

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
    1. Download the app [zip](https://github.com/iyarashii/Miru/releases/download/v1.0/Miru.zip).
    2. Extract the archive and run `setup.exe`.
# Usage
After successful installation run the app. You should see an app window that looks like image below.
![Image of the app window after 1st run](https://i.imgur.com/u1fb1wI.png)
Enter your username in the textbox and click `Sync` button or hit `Enter` key.
Wait for synchronization, after it you should see something like this:
![Image of synchronized app window](https://i.imgur.com/zCubMDf.png)
You can click on the anime image to go to the MAL site associated with this anime and you can click on the
anime name next to the image to copy it to your clipboard.
