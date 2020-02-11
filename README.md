# Miru
Miru is a WPF app used to display data from the jikan.net MyAnimeList API.  
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
    1. Download the app [zip].()
    2. Extract archive and run `setup.exe`.
# Usage
After successful installation run the app.
