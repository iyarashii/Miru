# Miru
Miru is a WPF app used to display data from the [jikan](https://github.com/Ervie/jikan.net) MyAnimeList API.
![App main view](https://user-images.githubusercontent.com/38395954/180601939-adcd1279-217e-4fe0-a47f-0b1b808a6b69.png)


Idea for the app is based on [senpai](http://www.senpai.moe/) a great site that shows airing anime in a neat way.  
It lets you display airing animes from the user's MAL watching list. Also it can display list of TV animes from the current season.
This app displays airing animes for each day of the week in the specified timezone.  
It uses EF6 and SQL Server's LocalDB to store the data.

SonarQube quality gate:
![SonarQube result](https://github.com/iyarashii/Miru/assets/38395954/22de7f95-d45e-4656-af96-f7d2c75e4801)


# Usage
After starting the application, you should see an app window that looks like image below.
Theme and time zone should be the same as your system's by the default.
![Image of the app window after 1st run](https://user-images.githubusercontent.com/38395954/180602243-25871e49-3edf-4f7f-8c44-d3eadfc87280.png)
Enter any MAL username in the `MAL Username` textbox (you can use <kbd>CTRL</kbd> + <kbd>M</kbd> shortcut to instantly focus this field and start typing) and click `Sync` button.
Wait for synchronization, after it you should see shows from the MAL list of the typed in user if their list is `public` for example:
![Image of synchronized app window](https://user-images.githubusercontent.com/38395954/180602370-bb742a14-7d60-43dd-91fe-25bc2e28b39a.png)


You can click on the username hyperlink (`iyarashii777's` on the image above) to go to the user's animelist page on MAL.

`Get Shows From The Current Season` button works like `Sync` button but it also gets all the animes from the current season even if the user is not watching them.  
`List Type`, `Broadcast Type` and `Time Zone` drop-downs can be used to customize anime list display.  
`Clear Cache` button clears data from database and local cache (local cache is located on the desktop in `MiruCache` folder).  
`Update Senpai Data` gets data from the senpai.moe site and stores it as JSON file in the local cache.  
`Filter Titles` lets you filter animes by name. It takes effect immediately as you are typing, you can use <kbd>CTRL</kbd> + <kbd>F</kbd> shortcut to instantly focus this field and start typing.  
You can set size of the images using `Art Size` field, size you select will be saved if you close the app and loaded on the next app launch.  
You can set opacity of the green/red highlight using `Dropped / Watching Highlight Opacity` field, value you select will be saved if you close the app and loaded on the next app launch.  
To reset to default you can delete your value and focus other element.  
You can click on the anime image to go to the MAL page associated with that anime and you can click on the
anime name next to the image to copy it to your clipboard.  
If anime list does not fit on the screen, you can scroll it up and down using a mouse scroll on each day anime list.  
`Get Dropped Anime Data` checkbox gets dropped anime info from user list during sync - check it if you want to have green/red highlight of dropped and watching animes when you set `List Type` to `Senpai - Current Season` or `Current Season`.


You can get a list of all animes in the current season by clicking `Get Shows From The Current Season` button:
![Image of the current season](https://user-images.githubusercontent.com/38395954/180602427-26ec53d7-9a3d-4b0a-88b8-e1b49e577135.png)

# Building
Build the solution with Visual Studio 2022.
# Installation guide for SQL Server LocalDB
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

3.  **Run the app using Miru.exe.**
# Changelog
For older changes go to the releases page and browse older releases.
### Miru 2.4
* Added progress bar that displays download progress of anime data: ![progress bar img](https://user-images.githubusercontent.com/38395954/180602002-8a5c0b4b-5f8c-4b1b-b75a-81a25e3c8442.png)
![season sync progress bar img](https://user-images.githubusercontent.com/38395954/180602324-79d5386f-9b66-422d-88d2-0fec62c01911.png)

* After using <kbd>CTRL</kbd> + <kbd>F</kbd> or <kbd>CTRL</kbd> + <kbd>M</kbd> to focus text boxes select all text of the text box.
* **Get Dropped Anime Data** checkbox default value changed to `true`.
